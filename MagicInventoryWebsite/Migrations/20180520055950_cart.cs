using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MagicInventoryWebsite.Migrations
{
    public partial class cart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Carts_StoreID",
                table: "Carts",
                column: "StoreID");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Stores_StoreID",
                table: "Carts",
                column: "StoreID",
                principalTable: "Stores",
                principalColumn: "StoreID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Stores_StoreID",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_Carts_StoreID",
                table: "Carts");
        }
    }
}
