using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ClinicManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleID", "CreatedAt", "RoleName", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 6, 15, 15, 56, 39, 691, DateTimeKind.Utc).AddTicks(5230), "Admin", new DateTime(2025, 6, 15, 15, 56, 39, 691, DateTimeKind.Utc).AddTicks(5450) },
                    { 2, new DateTime(2025, 6, 15, 15, 56, 39, 691, DateTimeKind.Utc).AddTicks(5577), "Receptionist", new DateTime(2025, 6, 15, 15, 56, 39, 691, DateTimeKind.Utc).AddTicks(5578) },
                    { 3, new DateTime(2025, 6, 15, 15, 56, 39, 691, DateTimeKind.Utc).AddTicks(5579), "HR", new DateTime(2025, 6, 15, 15, 56, 39, 691, DateTimeKind.Utc).AddTicks(5580) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleID",
                keyValue: 3);
        }
    }
}
