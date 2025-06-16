using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixRoleSeedingDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleID",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2023, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2023, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleID",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2023, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2023, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleID",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2023, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2023, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleID",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 15, 15, 56, 39, 691, DateTimeKind.Utc).AddTicks(5230), new DateTime(2025, 6, 15, 15, 56, 39, 691, DateTimeKind.Utc).AddTicks(5450) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleID",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 15, 15, 56, 39, 691, DateTimeKind.Utc).AddTicks(5577), new DateTime(2025, 6, 15, 15, 56, 39, 691, DateTimeKind.Utc).AddTicks(5578) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleID",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 15, 15, 56, 39, 691, DateTimeKind.Utc).AddTicks(5579), new DateTime(2025, 6, 15, 15, 56, 39, 691, DateTimeKind.Utc).AddTicks(5580) });
        }
    }
}
