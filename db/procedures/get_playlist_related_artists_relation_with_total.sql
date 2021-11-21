create or replace function public.get_playlist_related_artists_relation_with_total(playlistid character varying)
    returns TABLE
            (
                id    character varying,
                name  character varying,
                c     integer,
                total integer
            )
    language sql
as
$$
select *,
       (select count(*)
        from playlist_track
        where playlist_id = playlistid) as total
from get_playlist_related_artists_relation(playlistid)
$$;

alter function public.get_playlist_related_artists_relation_with_total(varchar) owner to postgres;