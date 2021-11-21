create table if not exists public.playlist
(
    id                   varchar(50)                            not null
        constraint playlist_pk
            primary key,
    name                 varchar(200)                           not null,
    owner_id             varchar(50),
    owner_name           varchar(200),
    is_public            boolean      default false             not null,
    is_collaborative     boolean      default false             not null,
    recommendation_count integer      default 0,
    playlist_type        integer                                not null,
    last_updated         timestamp(2) default CURRENT_TIMESTAMP not null,
    is_search_reduced    boolean      default false             not null,
    creation_time        timestamp(2) default CURRENT_TIMESTAMP not null
);

alter table public.playlist
    owner to postgres;

create unique index if not exists playlist_id_uindex
    on public.playlist (id);
