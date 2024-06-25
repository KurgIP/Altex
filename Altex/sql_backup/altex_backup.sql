--
-- PostgreSQL database dump
--

-- Dumped from database version 14.7
-- Dumped by pg_dump version 14.7

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: altex; Type: DATABASE; Schema: -; Owner: postgres
--

CREATE DATABASE altex WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE = 'English_United States.1252';


ALTER DATABASE altex OWNER TO postgres;

\connect altex

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: Fields; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Fields" (
    id integer NOT NULL,
    name character varying(32) NOT NULL,
    "order" integer NOT NULL,
    type character varying(32) NOT NULL,
    property character varying(512),
    description character varying(256) NOT NULL,
    html character varying(32) NOT NULL,
    place character varying(32) NOT NULL,
    skip_prm character varying(16) NOT NULL,
    filter character varying(64),
    sub character varying(16),
    roles character varying(512),
    show_in_list character varying(64)
);


ALTER TABLE public."Fields" OWNER TO postgres;

--
-- Name: fields_props_select_fields_by_place(character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fields_props_select_fields_by_place(plc character varying) RETURNS SETOF public."Fields"
    LANGUAGE plpgsql ROWS 50
    AS $$
DECLARE
BEGIN
	RETURN QUERY
		SELECT * FROM "Fields" WHERE "place" = plc ORDER BY "order";
RETURN;
END;    
$$;


ALTER FUNCTION public.fields_props_select_fields_by_place(plc character varying) OWNER TO postgres;

--
-- Name: fields_props_select_places(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fields_props_select_places() RETURNS SETOF record
    LANGUAGE plpgsql ROWS 50
    AS $$
DECLARE
BEGIN	
	RETURN QUERY
			SELECT DISTINCT "place" FROM "Fields" ORDER BY "place";			  
RETURN;
END;    
$$;


ALTER FUNCTION public.fields_props_select_places() OWNER TO postgres;

--
-- Name: filters_collaps_save_collaps(text, integer, text); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.filters_collaps_save_collaps(idusr text, iplc integer, val text) RETURNS void
    LANGUAGE plpgsql
    AS $$
DECLARE
BEGIN
	-- проверка, есть ли строка с настройками фильтра по колонкам, скрытых колонок и пагинации
	PERFORM  FROM "FiltersAndCollaps" WHERE "id_user"=idusr AND "place"=iplc;

	IF NOT FOUND THEN
		-- если нет такой строки
		INSERT INTO "FiltersAndCollaps" ( "id_user", "place", "filter", "collaps", "pagination" )
			                     VALUES ( idusr,     iplc,    '',        val,      ''           );

	ELSE
		-- обновляем фильтр
		UPDATE "FiltersAndCollaps" SET "collaps"=val WHERE "id_user"=idusr AND "place"=iplc;

	END IF;
  
RETURN;
END;    
$$;


ALTER FUNCTION public.filters_collaps_save_collaps(idusr text, iplc integer, val text) OWNER TO postgres;

--
-- Name: filters_collaps_save_filters_by_columns(text, integer, text); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.filters_collaps_save_filters_by_columns(idusr text, iplc integer, val text) RETURNS void
    LANGUAGE plpgsql
    AS $$
DECLARE
BEGIN
	-- проверка, есть ли строка с настройками фильтра по колонкам, скрытых колонок и пагинации
	PERFORM  FROM "FiltersAndCollaps" WHERE "id_user"=idusr AND "place"=iplc;

	IF NOT FOUND THEN
		-- если нет такой строки
		INSERT INTO "FiltersAndCollaps" ( "id_user", "place", "filter", "collaps", "pagination" )
			                     VALUES ( idusr,     iplc,    val,      '',         ''        );

	ELSE
		-- обновляем фильтр
		UPDATE "FiltersAndCollaps" SET "filter"=val WHERE "id_user"=idusr AND "place"=iplc;

	END IF;
  
RETURN;
END;    
$$;


ALTER FUNCTION public.filters_collaps_save_filters_by_columns(idusr text, iplc integer, val text) OWNER TO postgres;

--
-- Name: filters_collaps_save_pagination(text, integer, text); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.filters_collaps_save_pagination(idusr text, iplc integer, val text) RETURNS void
    LANGUAGE plpgsql
    AS $$
DECLARE
BEGIN
	-- проверка, есть ли строка с настройками фильтра по колонкам, скрытых колонок и пагинации
	PERFORM  FROM "FiltersAndCollaps" WHERE "id_user"=idusr AND "place"=iplc;

	IF NOT FOUND THEN
		-- если нет такой строки
		INSERT INTO "FiltersAndCollaps" ( "id_user", "place", "filter", "collaps", "pagination" )
			                     VALUES ( idusr,     iplc,    '',       '',        val          );

	ELSE
		-- обновляем фильтр
		UPDATE  "FiltersAndCollaps" SET "pagination"=val WHERE "id_user"=idusr AND "place"=iplc;

	END IF;
  
RETURN;
END;    
$$;


ALTER FUNCTION public.filters_collaps_save_pagination(idusr text, iplc integer, val text) OWNER TO postgres;

--
-- Name: filters_collaps_select_by_id_user(text, integer); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.filters_collaps_select_by_id_user(iid text, iplc integer) RETURNS SETOF record
    LANGUAGE plpgsql ROWS 1
    AS $$
DECLARE
BEGIN	
	RETURN QUERY
		SELECT "filter", "collaps", "pagination" FROM "FiltersAndCollaps" WHERE "id_user"=iid AND "place"=iplc;		  
RETURN;
END;    
$$;


ALTER FUNCTION public.filters_collaps_select_by_id_user(iid text, iplc integer) OWNER TO postgres;

--
-- Name: scan_result_delete_by_id(integer); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.scan_result_delete_by_id(id_del integer) RETURNS void
    LANGUAGE plpgsql
    AS $$
DECLARE
BEGIN
	-- Удаляем строки с указанным IP
	DELETE FROM "IPs" WHERE "id" = id_del;  
RETURN;
END;    
$$;


ALTER FUNCTION public.scan_result_delete_by_id(id_del integer) OWNER TO postgres;

--
-- Name: scan_result_delete_by_ip(text); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.scan_result_delete_by_ip(ip text) RETURNS void
    LANGUAGE plpgsql
    AS $$
DECLARE
BEGIN
	-- Удаляем строки с указанным IP
	DELETE FROM "IPs" WHERE "ip" = ip;  
RETURN;
END;    
$$;


ALTER FUNCTION public.scan_result_delete_by_ip(ip text) OWNER TO postgres;

--
-- Name: scan_result_save(text, text, text, text, text, real, text, timestamp without time zone, text, text, text, timestamp without time zone, text, text, integer[], text[], text[], text[], text[], text[]); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.scan_result_save(ip text, mac text, host text, host_type text, vendor text, finished_elapsed real, finished_exit text, finished_time timestamp without time zone, response_status text, runstats_down text, runstats_up text, start_scaning timestamp without time zone, status_reason text, status_state text, port_arr_number integer[], port_arr_method text[], port_arr_protocol text[], port_arr_reason text[], port_arr_service text[], port_arr_state text[]) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
ret_id_ip    integer;
ports_length integer;
BEGIN

		-- Вставляем параметры IP нового сканирования
		INSERT INTO "IPs" ( "ip", "mac", "host", "host_type", "vendor", "finished_elapsed", "finished_exit", "finished_time", "response_status", "runstats_down", "runstats_up", "start_scaning", "status_reason", "status_state" )
			      VALUES  ( ip,    mac,   host,   host_type,   vendor,   finished_elapsed,   finished_exit,   finished_time,   response_status,   runstats_down,   runstats_up,   start_scaning,   status_reason,   status_state  ) 
				      RETURNING "id" INTO ret_id_ip;

		-- Количество портов
		ports_length := array_length( port_arr_number, 1 );

		IF ports_length IS NOT NULL THEN					  
			-- Вставляем просканированные порты
			FOR r IN 1 .. ports_length LOOP
				INSERT INTO "Ports"( "ip_id",   "number",           "method",           "protocol",           "reason",           "service",           "state") 
							VALUES (  ret_id_ip, port_arr_number[r], port_arr_method[r], port_arr_protocol[r], port_arr_reason[r], port_arr_service[r], port_arr_state[r] );
			END LOOP;
		END IF;		

  
RETURN ret_id_ip;
END;    
$$;


ALTER FUNCTION public.scan_result_save(ip text, mac text, host text, host_type text, vendor text, finished_elapsed real, finished_exit text, finished_time timestamp without time zone, response_status text, runstats_down text, runstats_up text, start_scaning timestamp without time zone, status_reason text, status_state text, port_arr_number integer[], port_arr_method text[], port_arr_protocol text[], port_arr_reason text[], port_arr_service text[], port_arr_state text[]) OWNER TO postgres;

--
-- Name: IPs; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."IPs" (
    id integer NOT NULL,
    ip text NOT NULL,
    mac text,
    host text,
    host_type text,
    vendor text,
    finished_elapsed real NOT NULL,
    finished_exit text NOT NULL,
    finished_time timestamp without time zone NOT NULL,
    response_status text NOT NULL,
    runstats_down text,
    runstats_up text,
    start_scaning timestamp without time zone NOT NULL,
    status_reason text,
    status_state text
);


ALTER TABLE public."IPs" OWNER TO postgres;

--
-- Name: scan_result_select_by_columns_filters(integer, integer, text, text); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.scan_result_select_by_columns_filters(ilim integer, iofs integer, iwhr text, iord text) RETURNS SETOF public."IPs"
    LANGUAGE plpgsql ROWS 100
    AS $$
DECLARE
sql_text text;
BEGIN

RAISE NOTICE 'iwhr!!!!  [%]', iwhr;
RAISE NOTICE 'iord!!!!  [%]', iord;

	-- Указываем какие колонки врзвращаем
	sql_text := 'SELECT * ' ||
	
	-- Указываем таблицы используемые в запросе
	' FROM "IPs" as tb ';

	-- WHERE может и не быть, поэтому обрабатываем два варианта с ним и без него
	IF iwhr = '' THEN
		sql_text :=  sql_text || ' WHERE ';
	
	ELSE
		sql_text :=  sql_text || iwhr || ' AND ';
	  
	END IF;
	RAISE NOTICE '1!!!!  [%]', sql_text;
	
	-- Добавляем выбор принадлежности юзеру и компании
	 sql_text := sql_text   || 
				 iord       ||                       
				 ' LIMIT '  || ilim  ||
				 ' OFFSET ' || iofs;
	
	RAISE NOTICE '2!!!!  [%]', sql_text;
	
	RETURN QUERY
		EXECUTE sql_text;

RETURN;
END;    
$$;


ALTER FUNCTION public.scan_result_select_by_columns_filters(ilim integer, iofs integer, iwhr text, iord text) OWNER TO postgres;

--
-- Name: scan_result_select_ips_by_columns_filters(integer, integer, text, text); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.scan_result_select_ips_by_columns_filters(ilim integer, iofs integer, iwhr text, iord text) RETURNS SETOF public."IPs"
    LANGUAGE plpgsql ROWS 100
    AS $$
DECLARE
sql_text text;
BEGIN

--RAISE NOTICE 'iwhr!!!!  [%]', iwhr;
--RAISE NOTICE 'iord!!!!  [%]', iord;

	-- Указываем какие колонки врзвращаем
	sql_text := 'SELECT * ' ||
	
	-- Указываем таблицы используемые в запросе
	' FROM "IPs" as tb ';

	-- WHERE может и не быть, поэтому обрабатываем два варианта с ним и без него
	-- имя операнда WHERE, ORDER BY передаются в строке фильтра и сортировки
	IF iwhr = '' THEN
		--sql_text :=  sql_text || ' WHERE ';
	
	ELSE
		sql_text :=  sql_text || iwhr;
	  
	END IF;
	--RAISE NOTICE '1!!!!  [%]', sql_text;
	
	-- Добавляем выбор принадлежности юзеру и компании
	 sql_text := sql_text   || 
				 iord       ||                       
				 ' LIMIT '  || ilim  ||
				 ' OFFSET ' || iofs;
	
	--RAISE NOTICE '2!!!!  [%]', sql_text;
	
	RETURN QUERY
		EXECUTE sql_text;

RETURN;
END;    
$$;


ALTER FUNCTION public.scan_result_select_ips_by_columns_filters(ilim integer, iofs integer, iwhr text, iord text) OWNER TO postgres;

--
-- Name: scan_result_select_ips_quantity_by_columns_filters(text); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.scan_result_select_ips_quantity_by_columns_filters(iwhr text) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
sql_text text;
ret_qnt integer;
BEGIN

	-- Указываем какие колонки врзвращаем
	sql_text := 'SELECT COUNT(tb."id") ' ||
	
	-- Указываем таблицы используемые в запросе
	' FROM "IPs" as tb ';

	-- WHERE может и не быть, поэтому обрабатываем два варианта с ним и без него
	IF iwhr = '' THEN
		--sql_text :=  sql_text || ' WHERE ';
	
	ELSE
		sql_text :=  sql_text || iwhr;
	  
	END IF;

    --RAISE NOTICE 'sql_text  [%]', sql_text;
	EXECUTE sql_text  INTO ret_qnt;
RETURN ret_qnt;
END;    
$$;


ALTER FUNCTION public.scan_result_select_ips_quantity_by_columns_filters(iwhr text) OWNER TO postgres;

--
-- Name: Ports; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Ports" (
    id integer NOT NULL,
    number integer NOT NULL,
    ip_id integer NOT NULL,
    method text,
    protocol text,
    reason text,
    service text,
    state text
);


ALTER TABLE public."Ports" OWNER TO postgres;

--
-- Name: scan_result_select_ports_by_arr_id_ips(integer[]); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.scan_result_select_ports_by_arr_id_ips(arr_id integer[]) RETURNS SETOF public."Ports"
    LANGUAGE plpgsql ROWS 100
    AS $$
DECLARE
BEGIN

	RETURN QUERY
		SELECT * FROM "Ports" WHERE "ip_id"=ANY(arr_id) ORDER BY "ip_id", "number";

RETURN;
END;    
$$;


ALTER FUNCTION public.scan_result_select_ports_by_arr_id_ips(arr_id integer[]) OWNER TO postgres;

--
-- Name: AspNetRoleClaims; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."AspNetRoleClaims" (
    "Id" integer NOT NULL,
    "RoleId" text NOT NULL,
    "ClaimType" text,
    "ClaimValue" text
);


ALTER TABLE public."AspNetRoleClaims" OWNER TO postgres;

--
-- Name: AspNetRoleClaims_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public."AspNetRoleClaims" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."AspNetRoleClaims_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: AspNetRoles; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."AspNetRoles" (
    "Id" text NOT NULL,
    "Name" character varying(256),
    "NormalizedName" character varying(256),
    "ConcurrencyStamp" text
);


ALTER TABLE public."AspNetRoles" OWNER TO postgres;

--
-- Name: AspNetUserClaims; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."AspNetUserClaims" (
    "Id" integer NOT NULL,
    "UserId" text NOT NULL,
    "ClaimType" text,
    "ClaimValue" text
);


ALTER TABLE public."AspNetUserClaims" OWNER TO postgres;

--
-- Name: AspNetUserClaims_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public."AspNetUserClaims" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."AspNetUserClaims_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: AspNetUserLogins; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."AspNetUserLogins" (
    "LoginProvider" character varying(128) NOT NULL,
    "ProviderKey" character varying(128) NOT NULL,
    "ProviderDisplayName" text,
    "UserId" text NOT NULL
);


ALTER TABLE public."AspNetUserLogins" OWNER TO postgres;

--
-- Name: AspNetUserRoles; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."AspNetUserRoles" (
    "UserId" text NOT NULL,
    "RoleId" text NOT NULL
);


ALTER TABLE public."AspNetUserRoles" OWNER TO postgres;

--
-- Name: AspNetUserTokens; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."AspNetUserTokens" (
    "UserId" text NOT NULL,
    "LoginProvider" character varying(128) NOT NULL,
    "Name" character varying(128) NOT NULL,
    "Value" text
);


ALTER TABLE public."AspNetUserTokens" OWNER TO postgres;

--
-- Name: AspNetUsers; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."AspNetUsers" (
    "Id" text NOT NULL,
    "UserName" character varying(256),
    "NormalizedUserName" character varying(256),
    "Email" character varying(256),
    "NormalizedEmail" character varying(256),
    "EmailConfirmed" boolean NOT NULL,
    "PasswordHash" text,
    "SecurityStamp" text,
    "ConcurrencyStamp" text,
    "PhoneNumber" text,
    "PhoneNumberConfirmed" boolean NOT NULL,
    "TwoFactorEnabled" boolean NOT NULL,
    "LockoutEnd" timestamp without time zone,
    "LockoutEnabled" boolean NOT NULL,
    "AccessFailedCount" integer NOT NULL
);


ALTER TABLE public."AspNetUsers" OWNER TO postgres;

--
-- Name: Fields_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public."Fields" ALTER COLUMN id ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."Fields_id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: FiltersAndCollaps; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."FiltersAndCollaps" (
    id_user text NOT NULL,
    place integer NOT NULL,
    filter text,
    collaps text,
    pagination text
);


ALTER TABLE public."FiltersAndCollaps" OWNER TO postgres;

--
-- Name: IPs_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public."IPs" ALTER COLUMN id ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."IPs_id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: Ports_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public."Ports" ALTER COLUMN id ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."Ports_id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE public."__EFMigrationsHistory" OWNER TO postgres;

--
-- Data for Name: AspNetRoleClaims; Type: TABLE DATA; Schema: public; Owner: postgres
--



--
-- Data for Name: AspNetRoles; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public."AspNetRoles" VALUES ('5ab5945b-8605-493c-bc78-a805d90f83e0', 'SuperAdmin', 'SUPERADMIN', '96db2f9f-89d9-41da-8955-4e4196c72a3e');
INSERT INTO public."AspNetRoles" VALUES ('a031328d-ef0d-416c-a54f-2e9464add56d', 'Admin', 'ADMIN', '84dd3b8a-eb98-4491-ae4a-e3ef0c7237e2');
INSERT INTO public."AspNetRoles" VALUES ('71a25685-44e6-4ccb-9897-c364fc705d74', 'Visitor', 'VISITOR', 'c03a04f6-dfa6-41e5-b325-e599991816ab');
INSERT INTO public."AspNetRoles" VALUES ('7edf32c1-f433-444c-ae08-85483448ec54', 'Moderator', 'MODERATOR', '9d421b19-43e2-4f96-8e32-71cac39753d5');
INSERT INTO public."AspNetRoles" VALUES ('a30928de-15d4-4322-a80a-c0f158c09cf0', 'Basic', 'BASIC', 'd71b54f5-70d1-4027-8068-41fcf34f22fa');


--
-- Data for Name: AspNetUserClaims; Type: TABLE DATA; Schema: public; Owner: postgres
--



--
-- Data for Name: AspNetUserLogins; Type: TABLE DATA; Schema: public; Owner: postgres
--



--
-- Data for Name: AspNetUserRoles; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public."AspNetUserRoles" VALUES ('11dca867-37dd-439b-a6d5-461ceb15f293', '5ab5945b-8605-493c-bc78-a805d90f83e0');
INSERT INTO public."AspNetUserRoles" VALUES ('11dca867-37dd-439b-a6d5-461ceb15f293', 'a031328d-ef0d-416c-a54f-2e9464add56d');
INSERT INTO public."AspNetUserRoles" VALUES ('11dca867-37dd-439b-a6d5-461ceb15f293', '71a25685-44e6-4ccb-9897-c364fc705d74');
INSERT INTO public."AspNetUserRoles" VALUES ('11dca867-37dd-439b-a6d5-461ceb15f293', '7edf32c1-f433-444c-ae08-85483448ec54');
INSERT INTO public."AspNetUserRoles" VALUES ('11dca867-37dd-439b-a6d5-461ceb15f293', 'a30928de-15d4-4322-a80a-c0f158c09cf0');
INSERT INTO public."AspNetUserRoles" VALUES ('2003d3d0-9278-4aaf-8dd1-777e6a284ea8', '5ab5945b-8605-493c-bc78-a805d90f83e0');
INSERT INTO public."AspNetUserRoles" VALUES ('2003d3d0-9278-4aaf-8dd1-777e6a284ea8', '71a25685-44e6-4ccb-9897-c364fc705d74');
INSERT INTO public."AspNetUserRoles" VALUES ('2003d3d0-9278-4aaf-8dd1-777e6a284ea8', '7edf32c1-f433-444c-ae08-85483448ec54');
INSERT INTO public."AspNetUserRoles" VALUES ('2003d3d0-9278-4aaf-8dd1-777e6a284ea8', 'a031328d-ef0d-416c-a54f-2e9464add56d');
INSERT INTO public."AspNetUserRoles" VALUES ('2003d3d0-9278-4aaf-8dd1-777e6a284ea8', 'a30928de-15d4-4322-a80a-c0f158c09cf0');


--
-- Data for Name: AspNetUserTokens; Type: TABLE DATA; Schema: public; Owner: postgres
--



--
-- Data for Name: AspNetUsers; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public."AspNetUsers" VALUES ('11dca867-37dd-439b-a6d5-461ceb15f293', 'regkip@mail.ru', 'REGKIP@MAIL.RU', 'regkip@mail.ru', 'REGKIP@MAIL.RU', true, 'AQAAAAEAACcQAAAAEIcLWg5R/mq6G/Wncxd6Ecwp6Zn4TEI57gkDFsoSZ5kX402/acPLR7tvxerraB3eXA==', 'AFN2N5LMXEIWXWXM4CC36LPFUOHZG7VJ', '10c12c26-a7ce-4f60-83dd-08997661a85c', '8(906) 742-4387', true, false, NULL, true, 0);
INSERT INTO public."AspNetUsers" VALUES ('2003d3d0-9278-4aaf-8dd1-777e6a284ea8', 'tester@mail.ru', 'TESTER@MAIL.RU', 'tester@mail.ru', 'TESTER@MAIL.RU', true, 'AQAAAAEAACcQAAAAEM9XOQTFlZfDhg0XLoIEJFX9No+FJirJnZEldYOkBXmooIiK4kK/FqSYWYWKbJpoWQ==', '2CBRMT6YVLPBYN3IT4NYAI67R3OI57SE', '254b9737-e8b8-4280-909e-a0cb97f4758d', NULL, false, false, NULL, true, 0);


--
-- Data for Name: Fields; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public."Fields" VALUES (1, 'id', 0, 'integer', '', 'ID', 'skip', 'IPs', 'skip', '', '', '', '');
INSERT INTO public."Fields" VALUES (2, 'ip', 1, 'string', '', 'IP', 'text', 'IPs', 'skip', '', '', '', '');
INSERT INTO public."Fields" VALUES (3, 'mac', 4, 'string', '', 'MAC', 'text', 'IPs', 'skip', '', '', '', '');
INSERT INTO public."Fields" VALUES (4, 'host', 2, 'string', '', 'Host', 'text', 'IPs', 'skip', '', '', '', '');
INSERT INTO public."Fields" VALUES (5, 'host_type', 3, 'string', '', 'Host_type', 'text', 'IPs', 'skip', '', '', '', '');
INSERT INTO public."Fields" VALUES (6, 'vendor', 5, 'string', '', 'Vendor', 'text', 'IPs', 'skip', '', '', '', '');
INSERT INTO public."Fields" VALUES (7, 'finished_elapsed', 9, 'float', '', 'Finished elapsed', 'text', 'IPs', 'skip', '', '', '', '');
INSERT INTO public."Fields" VALUES (10, 'response_status', 6, 'string', '', 'Response status', 'text', 'IPs', 'skip', '', '', '', '');
INSERT INTO public."Fields" VALUES (12, 'runstats_up', 10, 'string', '', 'Runstats up', 'text', 'IPs', 'skip', '', '', '', '');
INSERT INTO public."Fields" VALUES (13, 'start_scaning', 8, 'datetime', '', 'Start scaning', 'text', 'IPs', 'skip', '', '', '', '');
INSERT INTO public."Fields" VALUES (16, 'ports', 7, 'ports', ' ', 'Ports', 'ports', 'IPs', 'skip', ' ', ' ', ' ', ' ');
INSERT INTO public."Fields" VALUES (11, 'runstats_down', 11, 'string', '', 'Runstats down', 'text', 'IPs', 'skip', '', '', '', '');
INSERT INTO public."Fields" VALUES (14, 'status_reason', 12, 'string', '', 'Status reason', 'text', 'IPs', 'skip', '', '', '', '');
INSERT INTO public."Fields" VALUES (15, 'status_state', 13, 'string', '', 'Status state', 'text', 'IPs', 'skip', '', '', '', '');
INSERT INTO public."Fields" VALUES (8, 'finished_exit', 14, 'string', '', 'Finished exit', 'text', 'IPs', 'skip', '', '', '', '');
INSERT INTO public."Fields" VALUES (9, 'finished_time', 15, 'datetime', '', 'Finished time', 'text', 'IPs', 'skip', '', '', '', '');


--
-- Data for Name: FiltersAndCollaps; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public."FiltersAndCollaps" VALUES ('11dca867-37dd-439b-a6d5-461ceb15f293', 0, '<flt><clm>start_scaning</clm><tp>datetime</tp><vl>21.06.2024 23:59:59;24.06.2024 23:59:59;all</vl><ord>order_desc</ord></flt>', '', '1;50;50;0');


--
-- Data for Name: IPs; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public."IPs" VALUES (7, '10.0.0.1', '64:EE:B7:EB:56:68', 'www.netis.cc', 'www.netis.cc', 'Netis Technology', 4.21, 'success', '2024-06-19 19:37:03', 'Success', '0', '1', '2024-06-19 22:37:03.757873', 'arp-response', 'up');
INSERT INTO public."IPs" VALUES (8, '10.0.0.1', '64:EE:B7:EB:56:68', 'www.netis.cc', 'www.netis.cc', 'Netis Technology', 6.48, 'success', '2024-06-19 19:42:13', 'Success', '0', '1', '2024-06-19 22:42:13.548718', 'arp-response', 'up');
INSERT INTO public."IPs" VALUES (9, '10.0.0.1', '64:EE:B7:EB:56:68', 'www.netis.cc', 'www.netis.cc', 'Netis Technology', 4.12, 'success', '2024-06-19 19:42:31', 'Success', '0', '1', '2024-06-19 22:42:31.891733', 'arp-response', 'up');
INSERT INTO public."IPs" VALUES (10, '10.0.0.1', '64:EE:B7:EB:56:68', 'www.netis.cc', 'www.netis.cc', 'Netis Technology', 1.84, 'success', '2024-06-19 19:54:23', 'Success', '0', '1', '2024-06-19 22:54:23.879885', 'arp-response', 'up');
INSERT INTO public."IPs" VALUES (11, '10.0.0.1', '64:EE:B7:EB:56:68', 'www.netis.cc', 'www.netis.cc', 'Netis Technology', 4.3, 'success', '2024-06-19 19:54:59', 'Success', '0', '1', '2024-06-19 22:54:59.144999', 'arp-response', 'up');
INSERT INTO public."IPs" VALUES (12, '10.0.0.2', 'B0:A7:B9:CD:28:29', '', '', 'TP-Link Limited', 32.33, 'success', '2024-06-19 19:55:31', 'Success', '0', '1', '2024-06-19 22:55:31.621442', 'arp-response', 'up');
INSERT INTO public."IPs" VALUES (13, '10.0.0.1', '64:EE:B7:EB:56:68', 'www.netis.cc', 'www.netis.cc', 'Netis Technology', 1.89, 'success', '2024-06-19 20:10:26', 'Success', '0', '1', '2024-06-19 23:10:26.133602', 'arp-response', 'up');
INSERT INTO public."IPs" VALUES (14, '10.0.0.1', '64:EE:B7:EB:56:68', 'www.netis.cc', 'www.netis.cc', 'Netis Technology', 4.13, 'success', '2024-06-19 20:10:50', 'Success', '0', '1', '2024-06-19 23:10:50.290064', 'arp-response', 'up');
INSERT INTO public."IPs" VALUES (15, '10.0.0.2', 'B0:A7:B9:CD:28:29', '', '', 'TP-Link Limited', 32.15, 'success', '2024-06-19 20:11:22', 'Success', '0', '1', '2024-06-19 23:11:22.491248', 'arp-response', 'up');
INSERT INTO public."IPs" VALUES (22, '10.0.0.9', '16:8E:55:3F:6E:76', '', '', '', 9.15, 'success', '2024-06-19 20:11:59', 'Success', '0', '1', '2024-06-19 23:11:59.125141', 'arp-response', 'up');
INSERT INTO public."IPs" VALUES (16, '10.0.0.3', '', '', '', '', 1.67, 'success', '2024-06-19 20:11:24', 'Not_answer', '1', '0', '2024-06-19 23:11:24.316973', '', '');
INSERT INTO public."IPs" VALUES (17, '10.0.0.4', '', '', '', '', 1.74, 'success', '2024-06-19 20:11:26', 'Not_answer', '1', '0', '2024-06-19 23:11:26.129084', '', '');
INSERT INTO public."IPs" VALUES (19, '10.0.0.6', '', '', '', '', 1.69, 'success', '2024-06-19 20:11:46', 'Not_answer', '1', '0', '2024-06-19 23:11:46.436025', '', '');
INSERT INTO public."IPs" VALUES (20, '10.0.0.7', '', '', '', '', 1.69, 'success', '2024-06-19 20:11:48', 'Not_answer', '1', '0', '2024-06-19 23:11:48.189161', '', '');
INSERT INTO public."IPs" VALUES (21, '10.0.0.8', '', '', '', '', 1.7, 'success', '2024-06-19 20:11:49', 'Not_answer', '1', '0', '2024-06-19 23:11:49.929419', '', '');
INSERT INTO public."IPs" VALUES (26, '10.0.0.3', '', '', '', '', 1.66, 'success', '2024-06-19 20:25:19', 'Not_answer', '1', '0', '2024-06-19 23:25:19.223047', '', '');
INSERT INTO public."IPs" VALUES (27, '10.0.0.4', '', '', '', '', 1.67, 'success', '2024-06-19 20:25:20', 'Not_answer', '1', '0', '2024-06-19 23:25:21.014879', '', '');
INSERT INTO public."IPs" VALUES (29, '10.0.0.6', '', '', '', '', 1.67, 'success', '2024-06-19 20:25:46', 'Not_answer', '1', '0', '2024-06-19 23:25:46.48118', '', '');
INSERT INTO public."IPs" VALUES (30, '10.0.0.7', '', '', '', '', 1.68, 'success', '2024-06-19 20:25:48', 'Not_answer', '1', '0', '2024-06-19 23:25:48.193884', '', '');
INSERT INTO public."IPs" VALUES (31, '10.0.0.8', '', '', '', '', 1.68, 'success', '2024-06-19 20:25:49', 'Not_answer', '1', '0', '2024-06-19 23:25:49.931569', '', '');
INSERT INTO public."IPs" VALUES (34, '10.0.0.11', '', '', '', '', 1.72, 'success', '2024-06-19 20:39:07', 'Not_answer', '1', '0', '2024-06-19 23:39:07.922368', '', '');
INSERT INTO public."IPs" VALUES (36, '10.0.0.13', '', '', '', '', 1.68, 'success', '2024-06-19 20:40:17', 'Not_answer', '1', '0', '2024-06-19 23:40:17.77121', '', '');
INSERT INTO public."IPs" VALUES (38, '10.0.0.15', '', '', '', '', 1.66, 'success', '2024-06-19 20:40:27', 'Not_answer', '1', '0', '2024-06-19 23:40:27.179753', '', '');
INSERT INTO public."IPs" VALUES (39, '10.0.0.16', '', '', '', '', 1.66, 'success', '2024-06-19 20:40:28', 'Not_answer', '1', '0', '2024-06-19 23:40:28.872508', '', '');
INSERT INTO public."IPs" VALUES (40, '10.0.0.17', '', '', '', '', 1.68, 'success', '2024-06-19 20:40:30', 'Not_answer', '1', '0', '2024-06-19 23:40:30.591695', '', '');
INSERT INTO public."IPs" VALUES (41, '10.0.0.18', '', '', '', '', 1.73, 'success', '2024-06-19 21:15:21', 'Not_answer', '1', '0', '2024-06-20 00:15:21.122027', '', '');
INSERT INTO public."IPs" VALUES (43, '10.0.0.20', '', '', '', '', 1.69, 'success', '2024-06-19 21:15:29', 'Not_answer', '1', '0', '2024-06-20 00:15:29.819801', '', '');
INSERT INTO public."IPs" VALUES (45, '10.0.0.22', '', '', '', '', 1.77, 'success', '2024-06-19 21:15:38', 'Not_answer', '1', '0', '2024-06-20 00:15:38.96675', '', '');
INSERT INTO public."IPs" VALUES (46, '10.0.0.23', '', '', '', '', 1.67, 'success', '2024-06-19 21:15:40', 'Not_answer', '1', '0', '2024-06-20 00:15:40.681545', '', '');
INSERT INTO public."IPs" VALUES (47, '10.0.0.24', '', '', '', '', 1.65, 'success', '2024-06-19 21:15:42', 'Not_answer', '1', '0', '2024-06-20 00:15:42.37143', '', '');
INSERT INTO public."IPs" VALUES (48, '10.0.0.25', '', '', '', '', 1.67, 'success', '2024-06-19 21:15:44', 'Not_answer', '1', '0', '2024-06-20 00:15:44.082673', '', '');
INSERT INTO public."IPs" VALUES (53, '10.0.0.3', '', '', '', '', 1.7, 'success', '2024-06-19 21:22:01', 'Not_answer', '1', '0', '2024-06-20 00:22:01.570434', '', '');
INSERT INTO public."IPs" VALUES (18, '10.0.0.5', '00:D2:79:64:81:4E', '', '', 'Vingroup Joint Stock Company', 18.51, 'success', '2024-06-19 20:11:44', 'Success', '0', '1', '2024-06-19 23:11:44.705968', 'arp-response', 'up');
INSERT INTO public."IPs" VALUES (23, '10.0.0.10', 'D4:3D:7E:EA:7B:CF', '', '', 'Micro-Star Int&apos;l', 7.38, 'success', '2024-06-19 20:12:06', 'Success', '0', '1', '2024-06-19 23:12:06.647773', 'arp-response', 'up');
INSERT INTO public."IPs" VALUES (24, '10.0.0.1', '64:EE:B7:EB:56:68', 'www.netis.cc', 'www.netis.cc', 'Netis Technology', 4.25, 'success', '2024-06-19 20:24:44', 'Success', '0', '1', '2024-06-19 23:24:44.750237', 'arp-response', 'up');
INSERT INTO public."IPs" VALUES (25, '10.0.0.2', 'B0:A7:B9:CD:28:29', '', '', 'TP-Link Limited', 32.43, 'success', '2024-06-19 20:25:17', 'Success', '0', '1', '2024-06-19 23:25:17.257454', 'arp-response', 'up');
INSERT INTO public."IPs" VALUES (28, '10.0.0.5', '00:D2:79:64:81:4E', '', '', 'Vingroup Joint Stock Company', 23.71, 'success', '2024-06-19 20:25:44', 'Success', '0', '1', '2024-06-19 23:25:44.773404', 'arp-response', 'up');
INSERT INTO public."IPs" VALUES (32, '10.0.0.9', '16:8E:55:3F:6E:76', '', '', '', 7.99, 'success', '2024-06-19 20:25:57', 'Success', '0', '1', '2024-06-19 23:25:57.969949', 'arp-response', 'up');
INSERT INTO public."IPs" VALUES (33, '10.0.0.10', 'D4:3D:7E:EA:7B:CF', '', '', 'Micro-Star Int&apos;l', 7.38, 'success', '2024-06-19 20:26:05', 'Success', '0', '1', '2024-06-19 23:26:05.386281', 'arp-response', 'up');
INSERT INTO public."IPs" VALUES (35, '10.0.0.12', '54:6C:EB:90:39:EF', '', '', 'Intel Corporate', 67.83, 'success', '2024-06-19 20:40:15', 'Success', '0', '1', '2024-06-19 23:40:16.021339', 'arp-response', 'up');
INSERT INTO public."IPs" VALUES (37, '10.0.0.14', 'B6:CA:52:46:C6:B3', '', '', '', 7.66, 'success', '2024-06-19 20:40:25', 'Success', '0', '1', '2024-06-19 23:40:25.475772', 'arp-response', 'up');
INSERT INTO public."IPs" VALUES (42, '10.0.0.19', '', '', '', '', 6.86, 'success', '2024-06-19 21:15:28', 'Success', '0', '1', '2024-06-20 00:15:28.083659', 'localhost-response', 'up');
INSERT INTO public."IPs" VALUES (44, '10.0.0.21', '00:21:52:15:7B:83', '', '', 'General Satellite Research &amp; Development Limited', 7.3, 'success', '2024-06-19 21:15:37', 'Success', '0', '1', '2024-06-20 00:15:37.157881', 'arp-response', 'up');
INSERT INTO public."IPs" VALUES (49, '10.0.0.1', '64:EE:B7:EB:56:68', 'www.netis.cc', 'www.netis.cc', 'Netis Technology', 6.55, 'success', '2024-06-19 21:20:48', 'Success', '0', '1', '2024-06-20 00:20:49.000549', 'arp-response', 'up');
INSERT INTO public."IPs" VALUES (50, '10.0.0.1', '64:EE:B7:EB:56:68', 'www.netis.cc', 'www.netis.cc', 'Netis Technology', 1.85, 'success', '2024-06-19 21:21:03', 'Success', '0', '1', '2024-06-20 00:21:03.051697', 'arp-response', 'up');
INSERT INTO public."IPs" VALUES (51, '10.0.0.1', '64:EE:B7:EB:56:68', 'www.netis.cc', 'www.netis.cc', 'Netis Technology', 6.43, 'success', '2024-06-19 21:21:27', 'Success', '0', '1', '2024-06-20 00:21:27.501789', 'arp-response', 'up');
INSERT INTO public."IPs" VALUES (52, '10.0.0.2', 'B0:A7:B9:CD:28:29', '', '', 'TP-Link Limited', 32.2, 'success', '2024-06-19 21:21:59', 'Success', '0', '1', '2024-06-20 00:21:59.829917', 'arp-response', 'up');
INSERT INTO public."IPs" VALUES (54, '8.8.8.8', '', 'dns.google', 'dns.google', '', 10.15, 'success', '2024-06-22 23:52:53', 'Not_answer', '0', '1', '2024-06-23 02:52:53.324632', 'echo-reply', 'up');
INSERT INTO public."IPs" VALUES (55, '8.8.8.8', '', 'dns.google', 'dns.google', '', 9.96, 'success', '2024-06-23 00:07:55', 'Not_answer', '0', '1', '2024-06-23 03:07:55.777102', 'echo-reply', 'up');
INSERT INTO public."IPs" VALUES (56, '93.158.160.119', '', 'vla-32z4-eth-trunk4.yndx.net', 'vla-32z4-eth-trunk4.yndx.net', '', 170.33, 'success', '2024-06-24 15:36:18', 'Not_answer', '0', '1', '2024-06-24 18:36:18.379118', 'reset', 'up');


--
-- Data for Name: Ports; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public."Ports" VALUES (1, 52869, 7, '', 'tcp', 'syn-ack', '', 'open');
INSERT INTO public."Ports" VALUES (2, 0, 7, NULL, NULL, NULL, NULL, NULL);
INSERT INTO public."Ports" VALUES (3, 0, 7, NULL, NULL, NULL, NULL, NULL);
INSERT INTO public."Ports" VALUES (4, 52869, 8, '', 'tcp', 'syn-ack', '', 'open');
INSERT INTO public."Ports" VALUES (5, 0, 8, NULL, NULL, NULL, NULL, NULL);
INSERT INTO public."Ports" VALUES (6, 0, 8, NULL, NULL, NULL, NULL, NULL);
INSERT INTO public."Ports" VALUES (7, 52869, 9, '', 'tcp', 'syn-ack', '', 'open');
INSERT INTO public."Ports" VALUES (8, 0, 9, NULL, NULL, NULL, NULL, NULL);
INSERT INTO public."Ports" VALUES (9, 0, 9, NULL, NULL, NULL, NULL, NULL);
INSERT INTO public."Ports" VALUES (10, 52869, 10, '', 'tcp', 'syn-ack', '', 'open');
INSERT INTO public."Ports" VALUES (11, 0, 10, NULL, NULL, NULL, NULL, NULL);
INSERT INTO public."Ports" VALUES (12, 0, 10, NULL, NULL, NULL, NULL, NULL);
INSERT INTO public."Ports" VALUES (13, 52869, 11, '', 'tcp', 'syn-ack', '', 'open');
INSERT INTO public."Ports" VALUES (14, 0, 11, NULL, NULL, NULL, NULL, NULL);
INSERT INTO public."Ports" VALUES (15, 0, 11, NULL, NULL, NULL, NULL, NULL);
INSERT INTO public."Ports" VALUES (16, 52869, 13, '', 'tcp', 'syn-ack', '', 'open');
INSERT INTO public."Ports" VALUES (17, 0, 13, NULL, NULL, NULL, NULL, NULL);
INSERT INTO public."Ports" VALUES (18, 0, 13, NULL, NULL, NULL, NULL, NULL);
INSERT INTO public."Ports" VALUES (19, 52869, 14, '', 'tcp', 'syn-ack', '', 'open');
INSERT INTO public."Ports" VALUES (20, 0, 14, NULL, NULL, NULL, NULL, NULL);
INSERT INTO public."Ports" VALUES (21, 0, 14, NULL, NULL, NULL, NULL, NULL);
INSERT INTO public."Ports" VALUES (22, 445, 23, 'table', 'tcp', 'syn-ack', 'microsoft-ds', 'open');
INSERT INTO public."Ports" VALUES (23, 0, 23, NULL, NULL, NULL, NULL, NULL);
INSERT INTO public."Ports" VALUES (24, 0, 23, NULL, NULL, NULL, NULL, NULL);
INSERT INTO public."Ports" VALUES (25, 0, 23, NULL, NULL, NULL, NULL, NULL);
INSERT INTO public."Ports" VALUES (26, 52869, 24, '', 'tcp', 'syn-ack', '', 'open');
INSERT INTO public."Ports" VALUES (27, 0, 24, NULL, NULL, NULL, NULL, NULL);
INSERT INTO public."Ports" VALUES (28, 0, 24, NULL, NULL, NULL, NULL, NULL);
INSERT INTO public."Ports" VALUES (29, 445, 33, 'table', 'tcp', 'syn-ack', 'microsoft-ds', 'open');
INSERT INTO public."Ports" VALUES (30, 0, 33, NULL, NULL, NULL, NULL, NULL);
INSERT INTO public."Ports" VALUES (31, 0, 33, NULL, NULL, NULL, NULL, NULL);
INSERT INTO public."Ports" VALUES (32, 0, 33, NULL, NULL, NULL, NULL, NULL);
INSERT INTO public."Ports" VALUES (33, 5432, 42, 'table', 'tcp', 'syn-ack', 'postgresql', 'open');
INSERT INTO public."Ports" VALUES (34, 0, 42, NULL, NULL, NULL, NULL, NULL);
INSERT INTO public."Ports" VALUES (35, 0, 42, NULL, NULL, NULL, NULL, NULL);
INSERT INTO public."Ports" VALUES (36, 0, 42, NULL, NULL, NULL, NULL, NULL);
INSERT INTO public."Ports" VALUES (37, 0, 42, NULL, NULL, NULL, NULL, NULL);
INSERT INTO public."Ports" VALUES (38, 0, 42, NULL, NULL, NULL, NULL, NULL);
INSERT INTO public."Ports" VALUES (39, 50000, 44, 'table', 'tcp', 'syn-ack', 'ibm-db2', 'open');
INSERT INTO public."Ports" VALUES (40, 52869, 49, '', 'tcp', 'syn-ack', '', 'open');
INSERT INTO public."Ports" VALUES (41, 0, 49, NULL, NULL, NULL, NULL, NULL);
INSERT INTO public."Ports" VALUES (42, 0, 49, NULL, NULL, NULL, NULL, NULL);
INSERT INTO public."Ports" VALUES (43, 52869, 50, '', 'tcp', 'syn-ack', '', 'open');
INSERT INTO public."Ports" VALUES (44, 0, 50, NULL, NULL, NULL, NULL, NULL);
INSERT INTO public."Ports" VALUES (45, 0, 50, NULL, NULL, NULL, NULL, NULL);
INSERT INTO public."Ports" VALUES (46, 52869, 51, '', 'tcp', 'syn-ack', '', 'open');
INSERT INTO public."Ports" VALUES (47, 0, 51, NULL, NULL, NULL, NULL, NULL);
INSERT INTO public."Ports" VALUES (48, 0, 51, NULL, NULL, NULL, NULL, NULL);
INSERT INTO public."Ports" VALUES (49, 443, 54, 'table', 'tcp', 'syn-ack', 'https', 'open');
INSERT INTO public."Ports" VALUES (50, 0, 54, NULL, NULL, NULL, NULL, NULL);
INSERT INTO public."Ports" VALUES (51, 53, 55, 'table', 'tcp', 'syn-ack', 'domain', 'open');
INSERT INTO public."Ports" VALUES (52, 443, 55, 'table', 'tcp', 'syn-ack', 'https', 'open');
INSERT INTO public."Ports" VALUES (53, 22, 56, 'table', 'tcp', 'no-response', 'ssh', 'filtered');
INSERT INTO public."Ports" VALUES (54, 135, 56, 'table', 'tcp', 'no-response', 'msrpc', 'filtered');
INSERT INTO public."Ports" VALUES (55, 139, 56, 'table', 'tcp', 'no-response', 'netbios-ssn', 'filtered');


--
-- Data for Name: __EFMigrationsHistory; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public."__EFMigrationsHistory" VALUES ('20240609220004_InitialPersistedGrantDbMigration', '6.0.16');
INSERT INTO public."__EFMigrationsHistory" VALUES ('20240610012330_InsertDefaultUsers_InsertDefaultRolesDbMigration', '6.0.16');
INSERT INTO public."__EFMigrationsHistory" VALUES ('20240619104856_IPs_Ports_tables_create', '6.0.31');
INSERT INTO public."__EFMigrationsHistory" VALUES ('20240620140114_Fields_FilterAndCollaps_tables_create', '6.0.31');


--
-- Name: AspNetRoleClaims_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."AspNetRoleClaims_Id_seq"', 1, false);


--
-- Name: AspNetUserClaims_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."AspNetUserClaims_Id_seq"', 1, false);


--
-- Name: Fields_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Fields_id_seq"', 4, true);


--
-- Name: IPs_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."IPs_id_seq"', 56, true);


--
-- Name: Ports_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Ports_id_seq"', 55, true);


--
-- Name: Fields Fields_pk_id; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Fields"
    ADD CONSTRAINT "Fields_pk_id" PRIMARY KEY (id);


--
-- Name: FiltersAndCollaps FiltersAndCollaps_PK_iduser_idplace; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."FiltersAndCollaps"
    ADD CONSTRAINT "FiltersAndCollaps_PK_iduser_idplace" PRIMARY KEY (id_user, place);


--
-- Name: IPs IPs_PK_id; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."IPs"
    ADD CONSTRAINT "IPs_PK_id" PRIMARY KEY (id);


--
-- Name: AspNetRoleClaims PK_AspNetRoleClaims; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetRoleClaims"
    ADD CONSTRAINT "PK_AspNetRoleClaims" PRIMARY KEY ("Id");


--
-- Name: AspNetRoles PK_AspNetRoles; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetRoles"
    ADD CONSTRAINT "PK_AspNetRoles" PRIMARY KEY ("Id");


--
-- Name: AspNetUserClaims PK_AspNetUserClaims; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetUserClaims"
    ADD CONSTRAINT "PK_AspNetUserClaims" PRIMARY KEY ("Id");


--
-- Name: AspNetUserLogins PK_AspNetUserLogins; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetUserLogins"
    ADD CONSTRAINT "PK_AspNetUserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey");


--
-- Name: AspNetUserRoles PK_AspNetUserRoles; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetUserRoles"
    ADD CONSTRAINT "PK_AspNetUserRoles" PRIMARY KEY ("UserId", "RoleId");


--
-- Name: AspNetUserTokens PK_AspNetUserTokens; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetUserTokens"
    ADD CONSTRAINT "PK_AspNetUserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name");


--
-- Name: AspNetUsers PK_AspNetUsers; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetUsers"
    ADD CONSTRAINT "PK_AspNetUsers" PRIMARY KEY ("Id");


--
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- Name: Ports Ports_PK_id; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Ports"
    ADD CONSTRAINT "Ports_PK_id" PRIMARY KEY (id);


--
-- Name: EmailIndex; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "EmailIndex" ON public."AspNetUsers" USING btree ("NormalizedEmail");


--
-- Name: IX_AspNetRoleClaims_RoleId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AspNetRoleClaims_RoleId" ON public."AspNetRoleClaims" USING btree ("RoleId");


--
-- Name: IX_AspNetUserClaims_UserId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AspNetUserClaims_UserId" ON public."AspNetUserClaims" USING btree ("UserId");


--
-- Name: IX_AspNetUserLogins_UserId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AspNetUserLogins_UserId" ON public."AspNetUserLogins" USING btree ("UserId");


--
-- Name: IX_AspNetUserRoles_RoleId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AspNetUserRoles_RoleId" ON public."AspNetUserRoles" USING btree ("RoleId");


--
-- Name: RoleNameIndex; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX "RoleNameIndex" ON public."AspNetRoles" USING btree ("NormalizedName");


--
-- Name: UserNameIndex; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX "UserNameIndex" ON public."AspNetUsers" USING btree ("NormalizedUserName");


--
-- Name: AspNetRoleClaims FK_AspNetRoleClaims_AspNetRoles_RoleId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetRoleClaims"
    ADD CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES public."AspNetRoles"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUserClaims FK_AspNetUserClaims_AspNetUsers_UserId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetUserClaims"
    ADD CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUserLogins FK_AspNetUserLogins_AspNetUsers_UserId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetUserLogins"
    ADD CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUserRoles FK_AspNetUserRoles_AspNetRoles_RoleId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetUserRoles"
    ADD CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES public."AspNetRoles"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUserRoles FK_AspNetUserRoles_AspNetUsers_UserId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetUserRoles"
    ADD CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUserTokens FK_AspNetUserTokens_AspNetUsers_UserId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetUserTokens"
    ADD CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- Name: Ports FK_Ports_ip_id_IPs_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Ports"
    ADD CONSTRAINT "FK_Ports_ip_id_IPs_id" FOREIGN KEY (ip_id) REFERENCES public."IPs"(id) ON DELETE CASCADE;


--
-- Name: FiltersAndCollaps FiltersAndCollaps_FK_id_user; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."FiltersAndCollaps"
    ADD CONSTRAINT "FiltersAndCollaps_FK_id_user" FOREIGN KEY (id_user) REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- PostgreSQL database dump complete
--

