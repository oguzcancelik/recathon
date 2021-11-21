create function get_saved_album_information(playlistid character varying)
    returns TABLE
            (
                artist_count              integer,
                album_exists_artist_count integer
            )
    language sql
as
$$
select count(distinct id) filter ( where album_count != 0), count(distinct id) filter ( where album_count > 0)
from artist
where id in (select * from get_most_common_related_artists_in_playlist(playlistid));
$$;

alter function get_saved_album_information(varchar) owner to postgres;