using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MonolithAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Description", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("d1b172ba-4d15-4505-8de0-b43588da3359"), null, "Seller role", "Seller", "SELLER" },
                    { new Guid("dfb04ae0-0184-41ce-ac5d-1bee1ade19b3"), null, "Customer role", "Customer", "CUSTOMER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("d1b172ba-4d15-4505-8de0-b43588da3359"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("dfb04ae0-0184-41ce-ac5d-1bee1ade19b3"));
        }
    }
}
