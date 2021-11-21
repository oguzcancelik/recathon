using System;
using Dapper.Contrib.Extensions;

namespace SpotifyGateway.Data.Entities
{
    [Table("user_session")]
    public class UserSession
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string SessionGuid { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime UpdateTime { get; set; }
    }
}