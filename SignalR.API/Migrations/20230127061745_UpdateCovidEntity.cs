using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SignalR.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCovidEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Count",
                table: "Covids",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Count",
                table: "Covids");
        }
    }
}
