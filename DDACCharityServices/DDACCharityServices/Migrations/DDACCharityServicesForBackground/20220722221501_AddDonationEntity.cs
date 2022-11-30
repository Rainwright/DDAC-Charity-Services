using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DDACCharityServices.Migrations.DDACCharityServicesForBackground
{
    public partial class AddDonationEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Donation",
                columns: table => new
                {
                    DonationID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BackgroundID = table.Column<int>(nullable: false),
                    CustomerEmail = table.Column<string>(nullable: true),
                    DonationAmount = table.Column<int>(nullable: false),
                    DonationDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Donation", x => x.DonationID);
                    table.ForeignKey(
                        name: "FK_Donation_Background_BackgroundID",
                        column: x => x.BackgroundID,
                        principalTable: "Background",
                        principalColumn: "BackgroundID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Donation_BackgroundID",
                table: "Donation",
                column: "BackgroundID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Donation");
        }
    }
}
