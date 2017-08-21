using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication1.Data.Migrations
{
    public partial class Fund : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FundId1",
                table: "OrderDetails",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_FundId1",
                table: "OrderDetails",
                column: "FundId1");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Funds_FundId1",
                table: "OrderDetails",
                column: "FundId1",
                principalTable: "Funds",
                principalColumn: "FundId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Funds_FundId1",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_FundId1",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "FundId1",
                table: "OrderDetails");
        }
    }
}
