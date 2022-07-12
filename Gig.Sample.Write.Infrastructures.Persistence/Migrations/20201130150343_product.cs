using Microsoft.EntityFrameworkCore.Migrations;

namespace Gig.Sample.Write.Infrastructures.Persistence.Migrations
{
    public partial class product : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "hasAcitve",
                table: "Users",
                newName: "HasAcitve");

            migrationBuilder.RenameColumn(
                name: "code",
                table: "Users",
                newName: "Code");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HasAcitve",
                table: "Users",
                newName: "hasAcitve");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "Users",
                newName: "code");
        }
    }
}
