using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Atheneum.Migrations.Images
{
    public partial class ImagesData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImgData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Bytes = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImgData", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Imgs_ImgData_Id",
                table: "Imgs",
                column: "Id",
                principalTable: "ImgData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Imgs_ImgData_Id",
                table: "Imgs");

            migrationBuilder.DropTable(
                name: "ImgData");
        }
    }
}
