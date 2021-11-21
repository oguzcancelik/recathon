create table if not exists public.user_session
(
    id            serial      not null
        constraint user_session_pk
            primary key,
    user_id       varchar(50) not null,
    session_guid  varchar(50) not null,
    creation_time timestamp(2) default CURRENT_TIMESTAMP,
    update_time   timestamp(2) default CURRENT_TIMESTAMP
);

alter table public.user_session
    owner to postgres;

create unique index if not exists user_session_id_uindex
    on public.user_session (id);
