using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeafUpload.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAdvisoryAlertsJson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AlertsJson",
                table: "Advisories",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlertsJson",
                table: "Advisories");
        }
    }
}
