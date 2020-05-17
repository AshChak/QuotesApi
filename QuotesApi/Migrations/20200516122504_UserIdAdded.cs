using Microsoft.EntityFrameworkCore.Migrations;

namespace QuotesApi.Migrations
{
    public partial class UserIdAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Quotes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Quotes");
        }
    }
}
