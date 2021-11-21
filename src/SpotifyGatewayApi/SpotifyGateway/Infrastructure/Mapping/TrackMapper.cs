using System;
using System.Collections.Generic;
using System.Linq;
using SpotifyGateway.Data.Entities;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Infrastructure.Extensions;
using SpotifyAPI.Web.Models;
using PlaylistTrack = SpotifyGateway.Data.Entities.PlaylistTrack;

namespace SpotifyGateway.Infrastructure.Mapping
{
    public static class TrackMapper
    {
        public static IEnumerable<Track> GetTracks(this FullAlbum album)
        {
            var artist = album.Artists.FirstOrDefault();

            return album.Tracks.Items.Select(x => x.ToEntity(album.Id, album.Name, artist?.Id, artist?.Name));
        }

        public static Track ToEntity(this SimpleTrack simpleTrack, string albumId, string albumName, string artistId, string artistName)
        {
            return new Track
            {
                Id = simpleTrack.Id,
                AlbumId = albumId,
                AlbumName = albumName?.Limit(DatabaseConstants.AlbumNameLimit) ?? DatabaseConstants.DefaultAlbumName,
                ArtistId = artistId,
                ArtistName = artistName?.Limit(DatabaseConstants.ArtistNameLimit) ?? DatabaseConstants.DefaultArtistName,
                Duration = simpleTrack.DurationMs,
                Name = simpleTrack.Name.Limit(DatabaseConstants.TrackNameLimit) ?? DatabaseConstants.DefaultTrackName,
                CreationTime = DateTime.UtcNow,
                UpdateTime = DateTime.UtcNow
            };
        }

        public static List<Track> ToEntity(this IEnumerable<PlayHistory> playHistories)
        {
            return playHistories
                .Select(x => x.Track.ToEntity())
                .GroupBy(x => x.Id)
                .Select(x => x.First())
                .ToList();
        }

        public static Track ToEntity(this SimpleTrack simpleTrack)
        {
            return new Track
            {
                Id = simpleTrack.Id,
                ArtistId = simpleTrack.Artists.FirstOrDefault()?.Id,
                ArtistName = simpleTrack.Artists.FirstOrDefault()?.Name.Limit(DatabaseConstants.ArtistNameLimit) ?? DatabaseConstants.DefaultArtistName,
                Duration = simpleTrack.DurationMs,
                Name = simpleTrack.Name.Limit(DatabaseConstants.TrackNameLimit) ?? DatabaseConstants.DefaultTrackName,
                CreationTime = DateTime.UtcNow,
                UpdateTime = DateTime.UtcNow
            };
        }

        public static List<Track> ToEntity(this IEnumerable<SavedTrack> savedTracks)
        {
            return savedTracks
                .Select(x => x.Track.ToEntity())
                .GroupBy(x => x.Id)
                .Select(x => x.First())
                .ToList();
        }

        public static List<Track> ToEntity(this IEnumerable<FullTrack> playlistTracks)
        {
            return playlistTracks
                .GroupBy(x => x.Id)
                .Select(x => x.First().ToEntity())
                .ToList();
        }

        public static Track ToEntity(this FullTrack fullTrack)
        {
            return new Track
            {
                Id = fullTrack.Id,
                AlbumId = fullTrack.Album.Id,
                AlbumName = fullTrack.Album.Name?.Limit(DatabaseConstants.AlbumNameLimit) ?? DatabaseConstants.DefaultAlbumName,
                ArtistId = fullTrack.Artists.FirstOrDefault()?.Id,
                ArtistName = fullTrack.Artists.FirstOrDefault()?.Name.Limit(DatabaseConstants.ArtistNameLimit) ?? DatabaseConstants.DefaultArtistName,
                Duration = fullTrack.DurationMs,
                Name = fullTrack.Name.Limit(DatabaseConstants.TrackNameLimit) ?? DatabaseConstants.DefaultTrackName,
                CreationTime = DateTime.UtcNow,
                UpdateTime = DateTime.UtcNow
            };
        }

        public static List<PlaylistTrack> ToEntity(this IEnumerable<Track> tracks, string playlistId)
        {
            return tracks.Select(x => x.ToEntity(playlistId)).ToList();
        }

        public static PlaylistTrack ToEntity(this Track track, string playlistId)
        {
            return new PlaylistTrack
            {
                Id = $"{playlistId}{track.Id}",
                Acousticness = track.Acousticness,
                ArtistId = track.ArtistId,
                ArtistName = track.ArtistName.Limit(DatabaseConstants.ArtistNameLimit),
                Danceability = track.Danceability,
                Duration = track.Duration,
                Energy = track.Energy,
                Instrumentalness = track.Instrumentalness,
                Key = track.Key,
                Liveness = track.Liveness,
                Loudness = track.Loudness,
                Mode = track.Mode,
                Name = track.Name.Limit(DatabaseConstants.TrackNameLimit),
                PlaylistId = playlistId,
                Speechiness = track.Speechiness,
                Tempo = track.Tempo,
                TimeSignature = track.TimeSignature,
                TrackId = track.Id,
                Valence = track.Valence
            };
        }

        public static List<Track> ToEntity(this IEnumerable<PlaylistTrack> playlistTracks)
        {
            return playlistTracks.Select(x => x.ToEntity()).ToList();
        }

        public static Track ToEntity(this PlaylistTrack playlistTrack)
        {
            return new Track
            {
                Id = playlistTrack.TrackId,
                Acousticness = playlistTrack.Acousticness,
                ArtistId = playlistTrack.ArtistId,
                ArtistName = playlistTrack.ArtistName,
                Danceability = playlistTrack.Danceability,
                Duration = playlistTrack.Duration,
                Energy = playlistTrack.Energy,
                Instrumentalness = playlistTrack.Instrumentalness,
                Key = playlistTrack.Key,
                Liveness = playlistTrack.Liveness,
                Loudness = playlistTrack.Loudness,
                Mode = playlistTrack.Mode,
                Name = playlistTrack.Name,
                Speechiness = playlistTrack.Speechiness,
                Tempo = playlistTrack.Tempo,
                TimeSignature = playlistTrack.TimeSignature,
                Valence = playlistTrack.Valence
            };
        }

        public static List<Track> MapAudioFeatures(this List<Track> tracks, List<AudioFeatures> audioFeaturesList)
        {
            using var e1 = tracks.GetEnumerator();
            using var e2 = audioFeaturesList.GetEnumerator();

            var invalidTrackIds = new List<string>();

            while (e1.MoveNext() && e2.MoveNext())
            {
                var track = e1.Current;
                var audioFeatures = e2.Current;

                if (track == null)
                {
                    continue;
                }

                if (audioFeatures?.Id == null)
                {
                    invalidTrackIds.Add(track.Id);
                    continue;
                }

                if (track.Id == audioFeatures.Id)
                {
                    track.MapAudioFeatures(audioFeatures);
                }
                else
                {
                    var features = audioFeaturesList.FirstOrDefault(y => y.Id == track.Id);

                    if (features != null)
                    {
                        track.MapAudioFeatures(features);
                    }
                    else
                    {
                        invalidTrackIds.Add(track.Id);
                    }
                }
            }

            if (invalidTrackIds.Count > 0)
            {
                tracks.RemoveAll(x => invalidTrackIds.Contains(x.Id));
            }

            return tracks;
        }

        public static void MapAudioFeatures(this Track track, AudioFeatures audioFeatures)
        {
            track.Acousticness = audioFeatures.Acousticness;
            track.Danceability = audioFeatures.Danceability;
            track.Energy = audioFeatures.Energy;
            track.Instrumentalness = audioFeatures.Instrumentalness;
            track.Key = audioFeatures.Key;
            track.Liveness = audioFeatures.Liveness;
            track.Loudness = audioFeatures.Loudness;
            track.Mode = audioFeatures.Mode;
            track.Speechiness = audioFeatures.Speechiness;
            track.Tempo = audioFeatures.Tempo;
            track.TimeSignature = audioFeatures.TimeSignature;
            track.Valence = audioFeatures.Valence;
        }
    }
}