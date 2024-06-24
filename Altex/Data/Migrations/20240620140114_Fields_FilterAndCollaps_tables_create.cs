using Altex.Utils;
using Humanizer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using NuGet.Protocol.Plugins;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
using System.Net.NetworkInformation;
using System.Xml.Linq;

#nullable disable

namespace Altex.Data.Migrations
{
    public partial class Fields_FilterAndCollaps_tables_create : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Fields",
                columns: table => new
                {
                    id           = table.Column<int>(type: "integer", nullable: false)
                                               .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name         = table.Column<string>(type: "character varying(32)",  nullable: false),
                    order        = table.Column<int>(type:    "integer",                nullable: false),
                    type         = table.Column<string>(type: "character varying(32)",  nullable: false),
                    property     = table.Column<string>(type: "character varying(512)", nullable: true ),
                    description  = table.Column<string>(type: "character varying(256)", nullable: false),
                    html         = table.Column<string>(type: "character varying(32)",  nullable: false),
                    place        = table.Column<string>(type: "character varying(32)",  nullable: false),
                    skip_prm     = table.Column<string>(type: "character varying(16)",  nullable: false),
                    filter       = table.Column<string>(type: "character varying(64)",  nullable: true ),
                    sub          = table.Column<string>(type: "character varying(16)",  nullable: true ),
                    roles        = table.Column<string>(type: "character varying(512)", nullable: true ),
                    show_in_list = table.Column<string>(type: "character varying(64)",  nullable: true ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("Fields_pk_id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "FiltersAndCollaps",
                columns: table => new
                {
                    id_user    = table.Column<string>(type: "text", nullable: false),
                    place      = table.Column<int>(type:    "integer", nullable: false),
                    filter     = table.Column<string>(type: "text", nullable: true),
                    collaps    = table.Column<string>(type: "text", nullable: true),
                    pagination = table.Column<string>(type: "text", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("FiltersAndCollaps_PK_iduser_idplace", x => new { x.id_user, x.place });
                    table.ForeignKey(
                        name:            "FiltersAndCollaps_FK_id_user",
                        column:          x => x.id_user,
                        principalTable:  "AspNetUsers",
                        principalColumn: "Id",
                        onDelete:        ReferentialAction.Cascade);
                });


          //  Вставляем параметры полей в Fields
          //  INSERT INTO public."Fields"(id, name, "order", type, property, description, html, place, skip_prm, filter, sub, roles, show_in_list)
          //VALUES(3, 'mac', 2, 'string', '', 'MAC', 'text', 'IPs', 'skip', '', '', '', '');

          //  INSERT INTO public."Fields"(id, name, "order", type, property, description, html, place, skip_prm, filter, sub, roles, show_in_list)
          //VALUES(4, 'host', 3, 'string', '', 'Host', 'text', 'IPs', 'skip', '', '', '', '');

          //  INSERT INTO public."Fields"(id, name, "order", type, property, description, html, place, skip_prm, filter, sub, roles, show_in_list)
          //VALUES(5, 'host_type', 4, 'string', '', 'Host_type', 'text', 'IPs', 'skip', '', '', '', '');

          //  INSERT INTO public."Fields"(id, name, "order", type, property, description, html, place, skip_prm, filter, sub, roles, show_in_list)
          //VALUES(6, 'vendor', 5, 'string', '', 'Vendor', 'text', 'IPs', 'skip', '', '', '', '');

          //  INSERT INTO public."Fields"(id, name, "order", type, property, description, html, place, skip_prm, filter, sub, roles, show_in_list)
          //VALUES(7, 'finished_elapsed', 6, 'float', '', 'Finished elapsed', 'text', 'IPs', 'skip', '', '', '', '');

          //  INSERT INTO public."Fields"(id, name, "order", type, property, description, html, place, skip_prm, filter, sub, roles, show_in_list)
          //VALUES(8, 'finished_exit', 7, 'string', '', 'Finished exit', 'text', 'IPs', 'skip', '', '', '', '');

          //  INSERT INTO public."Fields"(id, name, "order", type, property, description, html, place, skip_prm, filter, sub, roles, show_in_list)
          //VALUES(9, 'finished_time', 8, 'datetime', '', 'Finished time', 'text', 'IPs', 'skip', '', '', '', '');

          //  INSERT INTO public."Fields"(id, name, "order", type, property, description, html, place, skip_prm, filter, sub, roles, show_in_list)
          //VALUES(10, 'response_status', 9, 'string', '', 'Response status', 'text', 'IPs', 'skip', '', '', '', '');

          //  INSERT INTO public."Fields"(id, name, "order", type, property, description, html, place, skip_prm, filter, sub, roles, show_in_list)
          //VALUES( 11, 'runstats_down', 10, 'string', '', 'Runstats down', 'text', 'IPs', 'skip', '', '', '', '');

          //  INSERT INTO public."Fields"(id, name, "order", type, property, description, html, place, skip_prm, filter, sub, roles, show_in_list)
          //VALUES( 12, 'runstats_up', 11, 'string', '', 'Runstats up', 'text', 'IPs', 'skip', '', '', '', '');

          //  INSERT INTO public."Fields"(id, name, "order", type, property, description, html, place, skip_prm, filter, sub, roles, show_in_list)
          //VALUES( 13, 'start_scaning', 12, 'datetime', '', 'Start scaning', 'text', 'IPs', 'skip', '', '', '', '');

          //  INSERT INTO public."Fields"(id, name, "order", type, property, description, html, place, skip_prm, filter, sub, roles, show_in_list)
          //VALUES( 14, 'status_reason', 13, 'string', '', 'Status reason', 'text', 'IPs', 'skip', '', '', '', '');

          //  INSERT INTO public."Fields"(id, name, "order", type, property, description, html, place, skip_prm, filter, sub, roles, show_in_list)
          //VALUES( 15, 'status_state', 14, 'string', '', 'Status state', 'text', 'IPs', 'skip', '', '', '', '');


        //migrationBuilder.InsertData(
        //    table: "Fields",
        //    columns:       new[] { "id", "name", "order", "type",    "property", "description", "html", "place", "skip_prm", "filter", "sub", "roles", "show_in_list" },
        //    values: new object[] {  1,   "id",    0,      "integer", "",         "ID",          "text", "IPs",   "skip",     "",       "",    "",      ""             }
        //);

        //migrationBuilder.InsertData(
        //    table: "Fields",
        //    columns:       new[] { "id", "name", "order", "type",   "property", "description", "html", "place", "skip_prm", "filter", "sub", "roles", "show_in_list" },
        //    values: new object[] {  2,   "ip",    1,      "string", "",         "IP",          "text", "IPs",   "skip",     "", "", "", ""  }
        //);

        //migrationBuilder.InsertData(
        //    table: "Fields",
        //    columns:       new[] { "id", "name", "order", "type",   "property", "description", "html", "place", "skip_prm", "filter", "sub", "roles", "show_in_list" },
        //    values: new object[] {  3,   "mac",   2,      "string", "",         "MAC",         "text", "IPs",   "skip",     "", "", "", ""  }
        //);

        //migrationBuilder.InsertData(
        //    table: "Fields",
        //    columns: new[] { "id", "name", "order", "type", "property", "description", "html", "place", "skip_prm", "filter", "sub", "roles", "show_in_list" },
        //    values: new object[] { 4, "host", 3, "string", "", "Host", "text", "IPs", "skip", "", "", "", ""  }
        //);

        //migrationBuilder.InsertData(
        //    table: "Fields",
        //    columns: new[] { "id", "name", "order", "type", "property", "description", "html", "place", "skip_prm", "filter", "sub", "roles", "show_in_list" },
        //    values: new object[] { 5, "host_type", 4, "string", "", "Host_type", "text", "IPs", "skip", "", "", "", ""  }
        //);

        //migrationBuilder.InsertData(
        //    table: "Fields",
        //    columns: new[] { "id", "name", "order", "type", "property", "description", "html", "place", "skip_prm", "filter", "sub", "roles", "show_in_list" },
        //    values: new object[] { 6, "vendor", 5, "string", "", "Vendor", "text", "IPs", "skip", "", "", "", ""  }
        //);

        //migrationBuilder.InsertData(
        //    table: "Fields",
        //    columns: new[] { "id", "name", "order", "type", "property", "description", "html", "place", "skip_prm", "filter", "sub", "roles", "show_in_list" },
        //    values: new object[] { 7, "finished_elapsed", 6, "float", "", "Finished elapsed", "text", "IPs", "skip", "", "", "", ""  }
        //);

        //migrationBuilder.InsertData(
        //    table: "Fields",
        //    columns: new[] { "id", "name", "order", "type", "property", "description", "html", "place", "skip_prm", "filter", "sub", "roles", "show_in_list" },
        //    values: new object[] { 8, "finished_exit", 7, "string", "", "Finished exit", "text", "IPs", "skip", "", "", "", ""  }
        //);

        //migrationBuilder.InsertData(
        //    table: "Fields",
        //    columns: new[] { "id", "name", "order", "type", "property", "description", "html", "place", "skip_prm", "filter", "sub", "roles", "show_in_list" },
        //    values: new object[] { 9, "finished_time", 8, "datetime", "", "Finished time", "text", "IPs", "skip", "", "", "", ""  }
        //);

        //migrationBuilder.InsertData(
        //    table: "Fields",
        //    columns: new[] { "id", "name", "order", "type", "property", "description", "html", "place", "skip_prm", "filter", "sub", "roles", "show_in_list" },
        //    values: new object[] { 10, "response_status", 9, "string", "", "Response status", "text", "IPs", "skip", "", "", "", ""  }
        //);

        //migrationBuilder.InsertData(
        //    table: "Fields",
        //    columns: new[] { "id", "name", "order", "type", "property", "description", "html", "place", "skip_prm", "filter", "sub", "roles", "show_in_list" },
        //    values: new object[] { 11, "runstats_down", 10, "string", "", "Runstats down", "text", "IPs", "skip", "", "", "", ""  }
        //);

        //migrationBuilder.InsertData(
        //    table: "Fields",
        //    columns: new[] { "id", "name", "order", "type", "property", "description", "html", "place", "skip_prm", "filter", "sub", "roles", "show_in_list" },
        //    values: new object[] { 12, "runstats_up", 11, "string", "", "Runstats up", "text", "IPs", "skip", "", "", "", ""  }
        //);

        //migrationBuilder.InsertData(
        //    table: "Fields",
        //    columns: new[] { "id", "name", "order", "type", "property", "description", "html", "place", "skip_prm", "filter", "sub", "roles", "show_in_list" },
        //    values: new object[] { 13, "start_scaning", 12, "datetime", "", "Start scaning", "text", "IPs", "skip", "", "", "", ""  }
        //);

        //migrationBuilder.InsertData(
        //    table: "Fields",
        //    columns: new[] { "id", "name", "order", "type", "property", "description", "html", "place", "skip_prm", "filter", "sub", "roles", "show_in_list" },
        //    values: new object[] { 14, "status_reason", 13, "string", "", "Status reason", "text", "IPs", "skip", "", "", "", ""  }
        //);

        //migrationBuilder.InsertData(
        //    table: "Fields",
        //    columns: new[] { "id", "name", "order", "type", "property", "description", "html", "place", "skip_prm", "filter", "sub", "roles", "show_in_list" },
        //    values: new object[] { 15, "status_state", 14, "string", "", "Status state", "text", "IPs", "skip", "", "", "", ""  }
        //);

    }


        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Fields");
            migrationBuilder.DropTable(name: "FiltersAndCollaps");
        }
    }
}
