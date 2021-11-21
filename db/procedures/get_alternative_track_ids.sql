create or replace function public.get_alternative_track_ids(playlistid character varying, userid character varying)
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
WHERE rh.is_succeed = true
  AND rh.user_id != userId
  AND rh.playlist_id = playlistId
ORDER BY random()
LIMIT 30;
$$;

alter function public.get_alternative_track_ids(varchar, varchar) owner to postgres;