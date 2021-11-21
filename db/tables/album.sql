create table if not exists album
(
    id               varchar(50)  not null
        constraint album_pk
            primary key,
    name             varchar(200) not null,
    artist_id        varchar(50)  not null,
    artist_name      varchar(200) not null,
    release_date     date,
    number_of_tracks smallint,
    is_completed     boolean,
    type             varchar(20),
    image_path       varchar(250),
    creation_time    timestamp(2) default CURRENT_TIMESTAMP,
    update_time      timestamp(2) default CURRENT_TIMESTAMP
);

alter table album
    owner to postgres;

create unique index if not exists album_id_uindex
    on album (id);
