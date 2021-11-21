create or replace function public.get_albums_for_track_search(playlistid character varying, types text, defined_limit integer) returns SETOF album
    language sql
as
$$
select a.*
from album as a
         join (select * from get_playlist_related_artists_relation_with_total(playlistid)) as ra on ra.id = a.artist_id
where a.is_completed is not true
  and a.number_of_tracks < 40
  and a.type = any (types::text[])
order by (cast((ra.c) as decimal) / cast(ra.total as decimal)) * random() * random() desc
limit defined_limit;
$$;

alter function public.get_albums_for_track_search(varchar, text, integer) owner to postgres;