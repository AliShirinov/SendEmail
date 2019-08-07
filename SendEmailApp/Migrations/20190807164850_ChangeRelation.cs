using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SendEmailApp.Migrations
{
    public partial class ChangeRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shares_IdentityUser<string>_AppUserId",
                table: "Shares");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Shares",
                table: "Shares");

            migrationBuilder.AlterColumn<string>(
                name: "AppUserId",
                table: "Shares",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Shares",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Shares",
                table: "Shares",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Shares_AppUserId",
                table: "Shares",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shares_IdentityUser<string>_AppUserId",
                table: "Shares",
                column: "AppUserId",
                principalTable: "IdentityUser<string>",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shares_IdentityUser<string>_AppUserId",
                table: "Shares");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Shares",
                table: "Shares");

            migrationBuilder.DropIndex(
                name: "IX_Shares_AppUserId",
                table: "Shares");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Shares");

            migrationBuilder.AlterColumn<string>(
                name: "AppUserId",
                table: "Shares",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Shares",
                table: "Shares",
                columns: new[] { "AppUserId", "UserTaskId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Shares_IdentityUser<string>_AppUserId",
                table: "Shares",
                column: "AppUserId",
                principalTable: "IdentityUser<string>",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
