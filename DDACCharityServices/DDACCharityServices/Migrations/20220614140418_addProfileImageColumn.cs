using Microsoft.EntityFrameworkCore.Migrations;

namespace DDACCharityServices.Migrations
{
    public partial class addProfileImageColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "profileImageUrl",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "profileImageUrl",
                table: "AspNetUsers");
        }
    }
}
