create or replace function public.get_related_artist_base(playlistid character varying, artist character varying)
    returns TABLE
            (
                artist_name character varying,
                name        character varying
            )
    language sql
as
$$
select pt.artist_name, pt.name
from playlist_track pt
         join related_artists ra on pt.artist_id = ra.artist_id
where pt.playlist_id = playlistid
  and (ra.related_artist_id = artist or ra.related_artist_name = artist)
order by artist_name;
$$;

alter function public.get_related_artist_base(varchar, varchar) owner to postgres;