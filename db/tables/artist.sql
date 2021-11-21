create table if not exists artist
(
    id                      varchar(50)  not null
        constraint artist_pk
            primary key,
    name                    varchar(200) not null,
    saved_album_count       integer,
    album_offset            integer,
    album_count             integer,
    saved_single_count      integer,
    single_offset           integer,
    single_count            integer,
    saved_compilation_count integer,
    compilation_offset      integer,
    compilation_count       integer,
    image_path              varchar(250),
    creation_time           timestamp(2) default CURRENT_TIMESTAMP,
    update_time             timestamp(2) default CURRENT_TIMESTAMP
);

alter table artist
    owner to postgres;

create unique index if not exists artist_id_uindex
    on artist (id);
