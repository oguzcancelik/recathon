using System.Collections.Generic;
using System.Threading.Tasks;
using SpotifyGateway.Data.Entities;
using SpotifyGateway.Models.Enums;
using SpotifyGateway.Models.FilterModels;

namespace SpotifyGateway.Services.SpotifyServices.Abstractions
{
    public interface ICredentialService
    {
        Task<Credential> GetByUsageTypeAsync(CredentialUsageType credentialUsageType = CredentialUsageType.Default);

        Task<Credential> GetByClientIdAsync(string clientId);

        Task<Credential> GetByUsageCountAsync();

        Task UpdateUsageCountAsync(string clientId, UsageCount usageCount);
        Task<List<Credential>> GetAsync(CredentialFilterModel filterModel);
    }
}