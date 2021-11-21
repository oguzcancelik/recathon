create table credential
(
    client_id         varchar(50)                not null
        constraint credential_pk
            primary key,
    client_secret     varchar(50)                not null,
    access_token      varchar(500),
    type              text[],
    usage_type        text[],
    redirect_uri      varchar(100),
    redirect_deeplink varchar(100),
    token_type        varchar(50),
    usage_count       integer,
    token_update_time timestamp(2) default CURRENT_TIMESTAMP,
    email             varchar(100)               not null,
    application_name  varchar(100)               not null,
    is_active         boolean      default false not null,
    is_deleted        boolean      default false not null,
    creation_time     timestamp(2) default CURRENT_TIMESTAMP,
    update_time       timestamp(2) default CURRENT_TIMESTAMP
);

alter table credential
    owner to postgres;

create unique index credential_client_id_uindex
    on credential (client_id);
