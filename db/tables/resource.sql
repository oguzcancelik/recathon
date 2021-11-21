create table if not exists resource
(
    id            serial
        constraint resource_pk
            primary key,
    language      text not null,
    class     	  text not null,
    name          text not null,
    value         text not null,
	is_active     boolean  not null,
    creation_time timestamp(2) default CURRENT_TIMESTAMP,
    update_time   timestamp(2) default CURRENT_TIMESTAMP
);

alter table resource
    owner to postgres;

create unique index if not exists resource_id_uindex
    on resource (id);

