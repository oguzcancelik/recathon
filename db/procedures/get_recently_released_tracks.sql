create or replace function public.get_recently_released_tracks(playlistid character varying)
    returns TABLE
            (
                id               character varying,
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
select t.id               as id,
       t.duration         as duration,
       t.key              as key,
       t.mode             as mode,
       t.time_signature   as time_signature,
       t.acousticness     as acousticness,
       t.danceability     as danceability,
       t.energy           as energy,
       t.instrumentalness as instrumentalness,
       t.liveness         as liveness,
       t.loudness         as loudness,
       t.speechiness      as speechiness,
       t.valence          as valence,
       t.tempo            as tempo
from track as t
         join album a on t.album_id = a.id
         join (select * from get_playlist_related_artists_relation(playlistid)) as ra on a.artist_id = ra.id
where current_date - release_date < 200;
$$;

alter function public.get_recently_released_tracks(varchar) owner to postgres;