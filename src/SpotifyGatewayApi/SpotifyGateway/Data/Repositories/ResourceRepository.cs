using System.Collections.Generic;
using Microsoft.Extensions.Options;
using SpotifyGateway.Data.Entities;
using SpotifyGateway.Data.Repositories.Abstractions;
using SpotifyGateway.Data.Repositories.Base;
using SpotifyGateway.Infrastructure.Configuration.Options;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Infrastructure.Extensions;
using SpotifyGateway.Models.Enums;
using SpotifyGateway.Models.FilterModels;

namespace SpotifyGateway.Data.Repositories
{
    public class ResourceRepository : Repository, IResourceRepository
    {
        public ResourceRepository(IOptions<DatabaseOptions> databaseOptions) : base(databaseOptions)
        {
        }

        public List<Resource> GetResources(ResourceFilterModel resourceFilterModel = null)
        {
            var condition = string.Empty;

            if (resourceFilterModel != null && resourceFilterModel.ResourcesClass != ResourcesClass.All)
            {
                condition += " AND position(@Class in class) > 0";
            }

            if (resourceFilterModel?.Language != null)
            {
                condition += " AND language = @Language";
            }

            var query = QueryConstants.GetResourcesQuery.Format(condition);

            var parameters = new
            {
                Class = resourceFilterModel?.ResourcesClass.ToString(),
                Language = resourceFilterModel?.Language?.ToString()
            };

            return Query<Resource>(query, parameters);
        }
    }
}