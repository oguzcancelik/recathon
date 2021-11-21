create table if not exists worker
(
    id                  varchar(50)           not null
        constraint worker_pk
            primary key,
    name                varchar(50),
    is_working          boolean default false not null,
    is_enabled          boolean default false not null,
    trigger_immediately boolean default false not null,
    run_on_startup      boolean default false not null,
    creation_time       timestamp(2)          not null,
    update_time         timestamp(2)          not null
);

alter table worker
    owner to postgres;

create unique index if not exists worker_id_uindex
    on worker (id);
