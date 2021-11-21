using System.Collections.Generic;
using SpotifyGateway.Models.Enums;

namespace SpotifyGateway.Infrastructure.Configuration.Options
{
    public class GeneralOptions
    {
        public string BaseUrl { get; set; }

        public string Name { get; set; }

        public Environment Environment { get; set; }

        public int WorkerThreadsCount { get; set; }

        public int CompletionThreadsCount { get; set; }

        public List<Language> ActiveLanguages { get; set; }

        public bool IsClientAppEnabled { get; set; }
    }
}