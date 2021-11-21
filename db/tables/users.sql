create table if not exists public.users
(
    id            varchar(50)                            not null
        constraint users_pk
            primary key,
    display_name  varchar(50),
    access_token  varchar(500)                           not null,
    refresh_token varchar(500)                           not null,
    client_id     varchar(50)                            not null,
    token_type    varchar(50)                            not null,
    expires_in    integer                                not null,
    creation_time timestamp(2) default CURRENT_TIMESTAMP not null,
    update_time   timestamp(2) default CURRENT_TIMESTAMP
);

alter table public.users
    owner to postgres;

create unique index if not exists users_id_uindex
    on public.users (id);
