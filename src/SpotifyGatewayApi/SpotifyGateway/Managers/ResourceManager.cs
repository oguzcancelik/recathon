using System.Linq;
using SpotifyGateway.Data.Repositories.Abstractions;
using SpotifyGateway.Infrastructure.Extensions;
using SpotifyGateway.Infrastructure.Factories.Abstractions;
using SpotifyGateway.Managers.Abstractions;
using SpotifyGateway.Models.FilterModels;
using SpotifyGateway.Models.Requests;

namespace SpotifyGateway.Managers
{
    public class ResourceManager : IResourceManager
    {
        private readonly IResourceFactory _resourceFactory;
        private readonly IResourceRepository _resourceRepository;

        public ResourceManager(
            IResourceFactory resourceFactory,
            IResourceRepository resourceRepository
        )
        {
            _resourceFactory = resourceFactory;
            _resourceRepository = resourceRepository;
        }

        public void UpdateResources(UpdateResourcesRequest request)
        {
            var resourceFilterModel = new ResourceFilterModel
            {
                ResourcesClass = request.ResourcesClass,
                Language = request.Language
            };

            var values = _resourceRepository.GetResources(resourceFilterModel);

            var resourcesList = _resourceFactory.GetResources(request.ResourcesClass);

            if (request.Language.HasValue)
            {
                resourcesList = resourcesList.Where(x => x.Language == request.Language.Value).ToList();
            }

            foreach (var resources in resourcesList)
            {
                var className = resources.GetType().Name;

                var currentValues = values.Where(x => className.ContainsIgnoreCase(x.Class) && x.Language == resources.Language.ToString()).ToList();

                resources.SetValues(currentValues);
            }
        }
    }
}