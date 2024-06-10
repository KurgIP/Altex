using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Altex.Data.Migrations
{
    public partial class InsertDefaultUsers_InsertDefaultRolesDbMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //  Вставляем главного юзера-админа
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail", "EmailConfirmed", "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "PhoneNumber", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnd", "LockoutEnabled", "AccessFailedCount" },
                values: new object[] { "11dca867-37dd-439b-a6d5-461ceb15f293", "regkip@mail.ru", "REGKIP@MAIL.RU", "regkip@mail.ru", "REGKIP@MAIL.RU", true, "AQAAAAEAACcQAAAAEIcLWg5R/mq6G/Wncxd6Ecwp6Zn4TEI57gkDFsoSZ5kX402/acPLR7tvxerraB3eXA==", "AFN2N5LMXEIWXWXM4CC36LPFUOHZG7VJ", "10c12c26-a7ce-4f60-83dd-08997661a85c", "8(906) 742-4387", true, false, null, true, 0 }
            );

            //  Вставляем роли по умолчанию
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[] { "5ab5945b-8605-493c-bc78-a805d90f83e0", "SuperAdmin", "SUPERADMIN", "96db2f9f-89d9-41da-8955-4e4196c72a3e" }
            );
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[] { "a031328d-ef0d-416c-a54f-2e9464add56d", "Admin", "ADMIN", "84dd3b8a-eb98-4491-ae4a-e3ef0c7237e2" }
            );
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[] { "71a25685-44e6-4ccb-9897-c364fc705d74", "Visitor", "VISITOR", "c03a04f6-dfa6-41e5-b325-e599991816ab" }
            );
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[] { "7edf32c1-f433-444c-ae08-85483448ec54", "Moderator", "MODERATOR", "9d421b19-43e2-4f96-8e32-71cac39753d5" }
            );
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[] { "a30928de-15d4-4322-a80a-c0f158c09cf0", "Basic", "BASIC", "d71b54f5-70d1-4027-8068-41fcf34f22fa" }
            );

            //  Присватваем дефолтному юзеру роли по умолчанию

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[] { "11dca867-37dd-439b-a6d5-461ceb15f293", "5ab5945b-8605-493c-bc78-a805d90f83e0" }
            );
            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[] { "11dca867-37dd-439b-a6d5-461ceb15f293", "a031328d-ef0d-416c-a54f-2e9464add56d" }
            );
            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[] { "11dca867-37dd-439b-a6d5-461ceb15f293", "71a25685-44e6-4ccb-9897-c364fc705d74" }
            );
            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[] { "11dca867-37dd-439b-a6d5-461ceb15f293", "7edf32c1-f433-444c-ae08-85483448ec54" }
            );
            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[] { "11dca867-37dd-439b-a6d5-461ceb15f293", "a30928de-15d4-4322-a80a-c0f158c09cf0" }
            );

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DELETE FROM AspNetUsers", true);
            migrationBuilder.Sql(@"DELETE FROM AspNetRoles", true);
            migrationBuilder.Sql(@"DELETE FROM AspNetUserRoles", true);
        }
    }
}
