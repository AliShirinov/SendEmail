using Microsoft.EntityFrameworkCore.Migrations;

namespace SendEmailApp.Migrations
{
    public partial class RemoveId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "Shares");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Shares",
                nullable: false,
                defaultValue: 0);
        }
    }
}
