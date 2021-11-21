create table if not exists recommendation_history
(
    id                      serial      not null
        constraint recommendation_history_pk
            primary key,
    user_id                 varchar(50) not null,
    playlist_id             varchar(50) not null,
    playlist_name           varchar(200),
    generated_playlist_id   varchar(50),
    generated_playlist_name varchar(200),
    recommended_track_count integer     not null,
    is_succeed              boolean     not null,
    error_message           varchar(200),
    playlist_type           integer     not null,
    playlist_source         integer     not null,
    generate_type           integer     not null,
    start_time              timestamp(2),
    end_time                timestamp(2),
    creation_time           timestamp(2) default CURRENT_TIMESTAMP,
    update_time             timestamp(2) default CURRENT_TIMESTAMP
);

alter table recommendation_history
    owner to postgres;

create unique index if not exists recommendation_history_id_uindex
    on recommendation_history (id);
