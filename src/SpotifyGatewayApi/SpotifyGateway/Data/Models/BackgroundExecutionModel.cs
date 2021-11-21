using System.Collections.Generic;

namespace SpotifyGateway.Data.Models
{
    public class BackgroundExecutionModel
    {
        public string Query { get; set; }

        public Dictionary<string, object> Parameters { get; set; }

        public string PageParameterName { get; set; }

        public List<int> Ids { get; set; }
    }
}