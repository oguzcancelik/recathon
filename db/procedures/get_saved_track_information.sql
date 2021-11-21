create or replace function public.get_saved_track_information(playlistid character varying)
    returns TABLE
            (
                artist_count integer,
                track_count  integer
            )
    language sql
as
$$
select count(distinct artist_id), sum(number_of_tracks) filter ( where is_completed )
from album
where artist_id in (select * from get_most_common_related_artists_in_playlist(playlistid));
$$;

alter function public.get_saved_track_information(varchar) owner to postgres;
