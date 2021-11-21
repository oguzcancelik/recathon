create function get_artists_for_album_search(playlistid character varying, defined_limit integer)
    returns TABLE
            (
                id                      character varying,
                name                    character varying,
                album_offset            integer,
                album_count             integer,
                saved_album_count       integer,
                single_offset           integer,
                single_count            integer,
                saved_single_count      integer,
                compilation_offset      integer,
                compilation_count       integer,
                saved_compilation_count integer,
                creation_time           timestamp without time zone
            )
    language sql
as
$$
select a.id,
       a.name,
       a.album_offset,
       a.album_count,
       a.saved_album_count,
       a.single_offset,
       a.single_count,
       a.saved_single_count,
       a.compilation_offset,
       a.compilation_count,
       a.saved_compilation_count,
       a.creation_time
from artist as a
         join(select * from get_playlist_related_artists_relation(playlistid)) as ra on ra.id = a.id
where a.album_count = 0
   or a.single_count = 0
   or a.compilation_count = 0
   or (a.album_count > a.album_offset and a.saved_album_count < 100)
   or (a.single_count > a.single_offset and a.saved_single_count < 100)
   or (a.compilation_count > a.compilation_offset and a.saved_compilation_count < 100)
order by (a.album_count = 0 or a.single_count = 0) desc, ra.c * random() desc
limit defined_limit;
$$;

alter function get_artists_for_album_search(varchar, integer) owner to postgres;
