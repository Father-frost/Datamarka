using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Datamarka_DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddBatchColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrderCode",
                table: "Order",
                newName: "Batch");

            migrationBuilder.AlterColumn<DateTime>(
                name: "WarrantDate",
                table: "Order",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ProdDate",
                table: "Order",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Batch",
                table: "Order",
                newName: "OrderCode");

            migrationBuilder.AlterColumn<DateTime>(
                name: "WarrantDate",
                table: "Order",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ProdDate",
                table: "Order",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");
        }
    }
}
