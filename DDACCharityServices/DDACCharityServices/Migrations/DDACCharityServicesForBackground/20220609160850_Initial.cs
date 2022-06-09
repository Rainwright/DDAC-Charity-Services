using Microsoft.EntityFrameworkCore.Migrations;

namespace DDACCharityServices.Migrations.DDACCharityServicesForBackground
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Background",
                columns: table => new
                {
                    BackgroundID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BackgroundName = table.Column<string>(maxLength: 255, nullable: false),
                    BackgroundDescription = table.Column<string>(maxLength: 1000, nullable: false),
                    BackgroundAmount = table.Column<int>(nullable: false),
                    CustomUserModelEmail = table.Column<string>(nullable: true),
                    BackgroundStatus = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Background", x => x.BackgroundID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Background");
        }
    }
}
