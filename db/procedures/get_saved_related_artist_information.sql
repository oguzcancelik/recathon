create or replace function public.get_saved_related_artist_information(playlistid character varying)
    returns TABLE
            (
                related_artist_count integer
            )
    language sql
as
$$
select count(distinct artist_id)
from related_artists
where artist_id in (select * from get_most_common_artists_in_playlist(playlistid));
$$;

alter function public.get_saved_related_artist_information(varchar) owner to postgres;