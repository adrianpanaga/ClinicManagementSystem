using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReAddPatientAndStaffUserForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Users_UserId_New",
                table: "Patients",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_StaffDetails_Users_UserId_New",
                table: "StaffDetails",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__Users__RoleID__45F365D3_New",
                table: "Users",
                column: "RoleID",
                principalTable: "Roles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Users_UserId_New",
                table: "Patients");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffDetails_Users_UserId_New",
                table: "StaffDetails");

            migrationBuilder.DropForeignKey(
                name: "FK__Users__RoleID__45F365D3_New",
                table: "Users");

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Users_UserId",
                table: "Patients",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK__StaffDeta__UserI__4BAC3F29",
                table: "StaffDetails",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__Users__RoleID__45F365D3",
                table: "Users",
                column: "RoleID",
                principalTable: "Roles",
                principalColumn: "Id");
        }
    }
}
