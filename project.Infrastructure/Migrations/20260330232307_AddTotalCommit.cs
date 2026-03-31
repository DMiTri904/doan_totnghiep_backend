using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace project.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTotalCommit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Contribution",
                table: "GroupMembers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Contribution",
                table: "GroupMembers");
        }
    }
}
