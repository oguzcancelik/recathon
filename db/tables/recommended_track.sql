create table if not exists public.recommended_track
(
    id                        serial       not null
        constraint recommended_track_pk
            primary key,
    recommendation_history_id integer      not null,
    track_id                  varchar(50)  not null,
    creation_time             timestamp(2) not null
);

alter table public.recommended_track
    owner to postgres;

create unique index if not exists recommended_track_id_uindex
    on public.recommended_track (id);
