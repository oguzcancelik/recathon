using System;
using SpotifyAPI.Web.Enums;

namespace SpotifyGateway.Data.Entities
{
    public class Artist
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int SavedAlbumCount { get; set; }

        public int AlbumOffset { get; set; }

        public int AlbumCount { get; set; }

        public int SavedSingleCount { get; set; }

        public int SingleOffset { get; set; }

        public int SingleCount { get; set; }

        public int SavedCompilationCount { get; set; }

        public int CompilationOffset { get; set; }

        public int CompilationCount { get; set; }

        public string ImagePath { get; set; }
        
        public DateTime? CreationTime { get; set; }

        public DateTime? UpdateTime { get; set; }

        public AlbumType SearchAlbumType { get; set; }
    }
}