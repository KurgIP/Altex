using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Altex.Data.Migrations
{
    public partial class IPs_Ports_tables_create : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "IPs",
                columns: table => new
                {
                    id                  = table.Column<int>(type: "integer", nullable: false)
                                               .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ip                  = table.Column<string>(type: "text", nullable: false),
                    mac                 = table.Column<string>(type: "text", nullable: true),
                    host                = table.Column<string>(type: "text", nullable: true),
                    host_type           = table.Column<string>(type: "text", nullable: true),
                    vendor              = table.Column<string>(type: "text", nullable: true),
                    finished_elapsed    = table.Column<string>(type: "Real", nullable: false),
                    finished_exit       = table.Column<string>(type: "text", nullable: false),
                    finished_time       = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    response_status     = table.Column<string>(type: "text", nullable: false),
                    runstats_down       = table.Column<string>(type: "text", nullable: true),
                    runstats_up         = table.Column<string>(type: "text", nullable: true),
                    start_scaning       = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    status_reason       = table.Column<string>(type: "text", nullable: true),
                    status_state        = table.Column<string>(type: "text", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("IPs_PK_id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Ports",
                columns: table => new
                {
                    id          = table.Column<int>(type: "integer", nullable: false)
                                       .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    number      = table.Column<int>(type: "integer", nullable: false),
                    ip_id       = table.Column<int>(type: "integer", nullable: false),
                    method      = table.Column<string>(type: "text", nullable: true),
                    protocol    = table.Column<string>(type: "text", nullable: true),
                    reason      = table.Column<string>(type: "text", nullable: true),
                    service     = table.Column<string>(type: "text", nullable: true),
                    state       = table.Column<string>(type: "text", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("Ports_PK_id", x => x.id);
                    table.ForeignKey(
                        name:            "FK_Ports_ip_id_IPs_id",
                        column:          x => x.ip_id,
                        principalTable:  "IPs",
                        principalColumn: "id",
                        onDelete:        ReferentialAction.Cascade);
                });

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "IPs"   );
            migrationBuilder.DropTable(name: "Ports" );
        }
    }
}
