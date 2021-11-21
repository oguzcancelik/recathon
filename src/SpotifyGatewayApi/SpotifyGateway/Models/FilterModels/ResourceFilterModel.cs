using SpotifyGateway.Data.Entities;
using SpotifyGateway.Models.Enums;
using SpotifyGateway.Models.FilterModels.Base;

namespace SpotifyGateway.Models.FilterModels
{
    public class ResourceFilterModel : BaseFilterModel<Resource>
    {
        public ResourcesClass ResourcesClass { get; set; }

        public Language? Language { get; set; }
    }
}