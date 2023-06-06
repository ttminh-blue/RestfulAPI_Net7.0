using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicVilla_VillaAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddFKIntoVilla : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "villaID",
                table: "villaNumber",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "villas",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2023, 6, 6, 22, 13, 40, 439, DateTimeKind.Local).AddTicks(7617));

            migrationBuilder.UpdateData(
                table: "villas",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2023, 6, 6, 22, 13, 40, 439, DateTimeKind.Local).AddTicks(7630));

            migrationBuilder.UpdateData(
                table: "villas",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2023, 6, 6, 22, 13, 40, 439, DateTimeKind.Local).AddTicks(7631));

            migrationBuilder.UpdateData(
                table: "villas",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2023, 6, 6, 22, 13, 40, 439, DateTimeKind.Local).AddTicks(7633));

            migrationBuilder.UpdateData(
                table: "villas",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2023, 6, 6, 22, 13, 40, 439, DateTimeKind.Local).AddTicks(7634));

            migrationBuilder.CreateIndex(
                name: "IX_villaNumber_villaID",
                table: "villaNumber",
                column: "villaID");

            migrationBuilder.AddForeignKey(
                name: "FK_villaNumber_villas_villaID",
                table: "villaNumber",
                column: "villaID",
                principalTable: "villas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_villaNumber_villas_villaID",
                table: "villaNumber");

            migrationBuilder.DropIndex(
                name: "IX_villaNumber_villaID",
                table: "villaNumber");

            migrationBuilder.DropColumn(
                name: "villaID",
                table: "villaNumber");

            migrationBuilder.UpdateData(
                table: "villas",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2023, 6, 5, 23, 50, 6, 812, DateTimeKind.Local).AddTicks(781));

            migrationBuilder.UpdateData(
                table: "villas",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2023, 6, 5, 23, 50, 6, 812, DateTimeKind.Local).AddTicks(793));

            migrationBuilder.UpdateData(
                table: "villas",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2023, 6, 5, 23, 50, 6, 812, DateTimeKind.Local).AddTicks(795));

            migrationBuilder.UpdateData(
                table: "villas",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2023, 6, 5, 23, 50, 6, 812, DateTimeKind.Local).AddTicks(796));

            migrationBuilder.UpdateData(
                table: "villas",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2023, 6, 5, 23, 50, 6, 812, DateTimeKind.Local).AddTicks(797));
        }
    }
}
