using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Gig.Sample.Write.Infrastructures.Persistence.Migrations
{
    public partial class product_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecGuid = table.Column<Guid>(nullable: false),
                    CompanyId = table.Column<long>(nullable: false),
                    BranchId = table.Column<long>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    OwnerId = table.Column<long>(nullable: false),
                    ModifiedBy = table.Column<long>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    ModifiedOn = table.Column<DateTime>(nullable: false),
                    StateCode = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    RowVersion = table.Column<byte[]>(nullable: true),
                    UserId = table.Column<long>(nullable: false),
                    Count = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Price = table.Column<decimal>(nullable: false),
                    HasUsed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
