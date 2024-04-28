using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library_APIs.Migrations
{
    /// <inheritdoc />
    public partial class Encryptedfieldsincategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EncDescription",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EncName",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

           
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.DropColumn(
                name: "EncDescription",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "EncName",
                table: "Categories");
        }
    }
}
