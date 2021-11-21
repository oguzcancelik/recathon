create or replace function public.get_playlist_track_features(playlistid character varying)
    returns TABLE
            (
                track_id         character varying,
                artist_id        character varying,
                duration         integer,
                key              smallint,
                mode             smallint,
                time_signature   smallint,
                acousticness     real,
                danceability     real,
                energy           real,
                instrumentalness real,
                liveness         real,
                loudness         real,
                speechiness      real,
                valence          real,
                tempo            real
            )
    language sql
as
$$
select track_id,
       artist_id,
       duration,
       key,
       mode,
       time_signature,
       acousticness,
       danceability,
       energy,
       instrumentalness,
       liveness,
       loudness,
       speechiness,
       valence,
       tempo
from playlist_track
where playlist_id = playlistid;
$$;

alter function public.get_playlist_track_features(varchar) owner to postgres;