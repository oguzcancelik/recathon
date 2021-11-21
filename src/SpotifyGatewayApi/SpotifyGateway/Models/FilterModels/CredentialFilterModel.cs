using SpotifyGateway.Data.Entities;
using SpotifyGateway.Models.FilterModels.Base;

namespace SpotifyGateway.Models.FilterModels
{
    public class CredentialFilterModel : BaseFilterModel<Credential>
    {
        protected override string WhereQuery()
        {
            var whereValue = Where?.Invoke(default)?.Query();
            var whereQuery = !string.IsNullOrEmpty(whereValue)
                ? $"WHERE {whereValue} AND is_active = TRUE AND is_deleted = FALSE"
                : "WHERE is_active = TRUE AND is_deleted = FALSE";

            return whereQuery;
        }
    }
}