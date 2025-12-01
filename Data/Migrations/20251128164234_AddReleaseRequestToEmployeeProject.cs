using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeePortal.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddReleaseRequestToEmployeeProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReleaseComment",
                table: "EmployeeProjects",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ReleaseRequested",
                table: "EmployeeProjects",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReleaseComment",
                table: "EmployeeProjects");

            migrationBuilder.DropColumn(
                name: "ReleaseRequested",
                table: "EmployeeProjects");
        }
    }
}
