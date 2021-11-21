create table if not exists public.playlist_track
(
    id               varchar(50) not null
        constraint playlist_track_pk
            primary key,
    playlist_id      varchar(50) not null,
    track_id         varchar(50),
    name             varchar(200),
    artist_id        varchar(50),
    artist_name      varchar(200),
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
    tempo            real
);

alter table public.playlist_track
    owner to postgres;

create unique index if not exists playlist_track_id_uindex
    on public.playlist_track (id);
