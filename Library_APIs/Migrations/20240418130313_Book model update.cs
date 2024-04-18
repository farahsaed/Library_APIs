using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library_APIs.Migrations
{
    /// <inheritdoc />
    public partial class Bookmodelupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookState",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookState",
                table: "Books");
        }
    }
}
