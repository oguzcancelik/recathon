create table if not exists public.related_artists
(
    id                  varchar(100) not null
        constraint related_artists_pk
            primary key,
    artist_id           varchar(50)  not null,
    artist_name         varchar(200) not null,
    related_artist_id   varchar(50),
    related_artist_name varchar(200)
);

alter table public.related_artists
    owner to postgres;

create unique index if not exists related_artists_id_uindex
    on public.related_artists (id);

