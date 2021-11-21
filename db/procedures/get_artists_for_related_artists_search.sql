create or replace function public.get_artists_for_related_artists_search(playlistid character varying, defined_limit integer)
    returns TABLE
            (
                id   character varying,
                name character varying
            )
    language sql
as
$$
select pt.artist_id, pt.artist_name
from playlist_track as pt
where pt.playlist_id = playlistId
  and not exists(select 1 from related_artists as ra where pt.artist_id = ra.artist_id)
group by pt.artist_id, pt.artist_name
order by count(*) desc
limit defined_limit;
$$;

alter function public.get_artists_for_related_artists_search(varchar, integer) owner to postgres;