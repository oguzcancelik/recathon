create table configuration
(
    id            serial
        constraint configuration_pk
            primary key,
    application   varchar(50)   not null,
    class         varchar(50)   not null,
    name          varchar(50)   not null,
    value         varchar(1200) not null,
    creation_time timestamp(2) default CURRENT_TIMESTAMP,
    update_time   timestamp(2) default CURRENT_TIMESTAMP
);

alter table configuration
    owner to postgres;

create unique index configuration_id_uindex
    on configuration (id);