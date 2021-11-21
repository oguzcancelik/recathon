using System;

namespace SpotifyGateway.Data.Entities
{
    public class Worker
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public bool IsWorking { get; set; }

        public bool IsEnabled { get; set; }

        public bool TriggerImmediately { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime UpdateTime { get; set; }
    }
}