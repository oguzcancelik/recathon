create function get_users_recommendation_history(userid character varying)
    returns TABLE
            (
                generated_playlist_id   character varying,
                generated_playlist_name character varying,
                playlist_id             character varying,
                track_id                character varying,
                track_name              character varying,
                artist_name             character varying,
                image_path              character varying,
                creation_time           timestamp without time zone
            )
    language sql
as
$$
select rh.generated_playlist_id,
       rh.generated_playlist_name,
       rh.playlist_id,
       t.id as TrackId,
       t.name,
       t.artist_name,
       a.image_path,
       rh.creation_time
from recommended_track as rt
         join (select distinct on (playlist_id) id,
                                                generated_playlist_id,
                                                generated_playlist_name,
                                                playlist_id,
                                                creation_time
               from recommendation_history
               where user_id = userid
                 and is_succeed is true
               order by playlist_id, creation_time desc) as rh on rh.id = rt.recommendation_history_id
         join track t on t.id = rt.track_id
         join album a on a.id = t.album_id
order by rh.creation_time desc;
$$;

alter function get_users_recommendation_history(varchar) owner to postgres;
