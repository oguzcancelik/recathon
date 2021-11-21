using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpotifyGateway.Data.Entities;
using SpotifyGateway.Data.Repositories.Base.Abstractions;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Models.Enums;
using SpotifyGateway.Models.FilterModels;
using SpotifyGateway.Models.FilterModels.Base;
using SpotifyGateway.Services.SpotifyServices.Abstractions;

namespace SpotifyGateway.Services.SpotifyServices
{
    public class CredentialService : ICredentialService
    {
        private readonly IRepository _repository;

        public CredentialService(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Credential>> GetAsync(CredentialFilterModel filterModel)
        {
            var credentials = await _repository.QueryAsync<Credential>(filterModel.Query(), filterModel.Parameters);

            return credentials.ToList();
        }

        public async Task<Credential> GetByUsageTypeAsync(CredentialUsageType credentialUsageType = CredentialUsageType.Default)
        {
            var parameters = new
            {
                Type = CredentialType.App.ToString(),
                UsageType = credentialUsageType.ToString(),
                CurrentTime = DateTime.UtcNow,
                TimeoutLimit = SpotifyApiConstants.TokenTimeoutLimit
            };

            var crendetial = await _repository.QueryFirstOrDefaultAsync<Credential>(QueryConstants.GetCredentialQuery, parameters);

            return crendetial;
        }

        public async Task<Credential> GetByClientIdAsync(string clientId)
        {
            var filterModel = new CredentialFilterModel
            {
                Where = x => new Where((nameof(x.ClientId), clientId, Operation.Equal)),
                Limit = 1
            };

            var crendentials = await GetAsync(filterModel);

            return crendentials.FirstOrDefault();
        }

        public async Task<Credential> GetByUsageCountAsync()
        {
            var filterModel = new CredentialFilterModel
            {
                Where = x => new Where((nameof(x.Type), CredentialType.Auth.ToString(), Operation.In)),
                OrderBy = x => new OrderBy((nameof(x.UsageCount), SortType.Asc), (string.Empty, SortType.Random)),
                Limit = 1
            };

            var crendentials = await GetAsync(filterModel);

            return crendentials.FirstOrDefault();
        }

        public async Task UpdateUsageCountAsync(string clientId, UsageCount usageCount)
        {
            var parameters = new
            {
                ClientId = clientId,
                Count = (int)usageCount
            };

            await _repository.ExecuteAsync(QueryConstants.UpdateCredentialsUsageCountQuery, parameters);
        }
    }
}