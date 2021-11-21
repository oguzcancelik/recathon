create or replace function public.get_most_common_related_artists_in_playlist(playlistid character varying)
    returns TABLE
            (
                artist_id character varying
            )
    language sql
as
$$
select id
from get_playlist_related_artists_relation(playlistid)
order by c desc
limit 50;
$$;

alter function public.get_most_common_related_artists_in_playlist(varchar) owner to postgres;