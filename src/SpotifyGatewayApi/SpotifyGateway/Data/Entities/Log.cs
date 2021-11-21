using System.Collections.Generic;
using Newtonsoft.Json;
using SpotifyGateway.Models.Enums;

namespace SpotifyGateway.Data.Entities
{
    public class Log : BaseMongoDbEntity
    {
        public int? Code { get; set; }
        
        public string ErrorMessage { get; set; }

        public string UserFriendlyMessage { get; set; }

        public string ClassName { get; set; }

        public string MethodName { get; set; }

        [JsonIgnore]
        public LogLevel LogLevel { get; set; }

        public Dictionary<string, object> Values { get; set; }

        [JsonIgnore]
        public string StackTrace { get; set; }

        public string RequestId { get; set; }
    }
}