create or replace function public.get_playlist_related_artists_relation(playlistid character varying)
    returns TABLE
            (
                id   character varying,
                name character varying,
                c    integer
            )
    language sql
as
$$
select ra.related_artist_id   as id,
       ra.related_artist_name as name,
       count(*)               as c
from playlist_track as pt
         join related_artists as ra on pt.artist_id = ra.artist_id
where pt.playlist_id = playlistid
group by related_artist_id, ra.related_artist_name;
$$;

alter function public.get_playlist_related_artists_relation(varchar) owner to postgres;