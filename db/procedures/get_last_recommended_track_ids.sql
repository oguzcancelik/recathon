create or replace function public.get_last_recommended_track_ids(playlistid character varying, userid character varying)
    returns TABLE
            (
                track_id character varying
            )
    language sql
as
$$
SELECT rt.track_id
FROM recommended_track AS rt
         JOIN recommendation_history AS rh ON rh.id = rt.recommendation_history_id
WHERE rh.user_id = userid
  AND rh.playlist_id = playlistId
  AND rh.is_succeed = true
ORDER BY rh.id DESC
LIMIT 250;
$$;

alter function public.get_last_recommended_track_ids(varchar, varchar) owner to postgres;