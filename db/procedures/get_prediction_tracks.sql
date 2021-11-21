create function get_prediction_tracks(playlistid character varying, userid character varying,
                                      defined_limit integer DEFAULT 1000)
    returns TABLE
            (
                id               character varying,
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
select t.id               as id,
       t.artist_id        as artist_id,
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
         join (select * from get_playlist_related_artists_relation_with_total(playlistid)) as ra on ra.id = t.artist_id
where t.id not in (select * from get_last_recommended_track_ids(playlistid, userid))
  and not exists(select 1
                 from playlist_track as pt
                 where pt.artist_id = t.artist_id
                   and pt.name = t.name
                   and playlist_id = playlistid)
order by (cast((ra.c) as decimal) / cast(ra.total as decimal)) * random() * random() * random() desc
limit defined_limit;
$$;

alter function get_prediction_tracks(varchar, varchar, integer) owner to postgres;

