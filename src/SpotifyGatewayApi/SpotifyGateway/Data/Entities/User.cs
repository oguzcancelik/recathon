using System;
using Dapper.Contrib.Extensions;

namespace SpotifyGateway.Data.Entities
{
    [Table("users")]
    public class User
    {
        [System.ComponentModel.DataAnnotations.Key]
        public string Id { get; set; }

        public string DisplayName { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public string ClientId { get; set; }

        public string TokenType { get; set; }

        public double ExpiresIn { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime UpdateTime { get; set; }
    }
}