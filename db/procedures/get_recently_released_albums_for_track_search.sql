create or replace function public.get_recently_released_albums_for_track_search(playlistid character varying, defined_limit integer) returns SETOF album
    language sql
as
$$
select distinct a.*
from album as a
         join related_artists as ra on a.artist_id = ra.related_artist_id
         join playlist_track as pt on ra.artist_id = pt.artist_id
where a.is_completed is false
  and pt.playlist_id = playlistid
  and current_date - a.release_date < 100
order by a.number_of_tracks desc, a.release_date desc
limit defined_limit;
$$;

alter function public.get_recently_released_albums_for_track_search(varchar, integer) owner to postgres;