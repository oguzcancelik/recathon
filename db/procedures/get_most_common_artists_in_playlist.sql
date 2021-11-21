create or replace function public.get_most_common_artists_in_playlist(playlistid character varying)
    returns TABLE
            (
                artist_id character varying
            )
    language sql
as
$$
select pt.artist_id
from playlist_track as pt
where playlist_id = playlistid
group by artist_id
order by count(*) desc
limit 50;
$$;

alter function public.get_most_common_artists_in_playlist(varchar) owner to postgres;