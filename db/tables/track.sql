create table if not exists public.track
(
    id               varchar(50)  not null
        constraint track_pk
            primary key,
    name             varchar(200) not null,
    album_id         varchar(50),
    album_name       varchar(200) not null,
    artist_id        varchar(50),
    artist_name      varchar(200) not null,
    duration         integer,
    key              smallint,
    mode             smallint,
    time_signature   smallint,
    acousticness     real,
    danceability     real,
    energy           real,
    instrumentalness real,
    liveness         real,
    loudness         real,
    speechiness      real,
    valence          real,
    tempo            real,
    creation_time    timestamp(2) default CURRENT_TIMESTAMP,
    update_time      timestamp(2) default CURRENT_TIMESTAMP
);

alter table public.track
    owner to postgres;

create unique index if not exists track_id_uindex
    on public.track (id);
