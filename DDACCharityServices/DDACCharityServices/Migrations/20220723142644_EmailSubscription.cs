using Microsoft.EntityFrameworkCore.Migrations;

namespace DDACCharityServices.Migrations
{
    public partial class EmailSubscription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SubscriptionArn",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubscriptionArn",
                table: "AspNetUsers");
        }
    }
}
