using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNewInventoryItemFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemBatch_InventoryItem",
                table: "ItemBatches");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemBatch_Vendor",
                table: "ItemBatches");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransaction_ItemBatch",
                table: "StockTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransaction_Patient",
                table: "StockTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransaction_StaffDetail",
                table: "StockTransactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vendors",
                table: "Vendors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StockTransactions",
                table: "StockTransactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemBatches",
                table: "ItemBatches");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InventoryItems",
                table: "InventoryItems");

            migrationBuilder.RenameColumn(
                name: "VendorId",
                table: "Vendors",
                newName: "VendorID");

            migrationBuilder.RenameColumn(
                name: "BatchId",
                table: "StockTransactions",
                newName: "BatchID");

            migrationBuilder.RenameColumn(
                name: "TransactionId",
                table: "StockTransactions",
                newName: "TransactionID");

            migrationBuilder.RenameIndex(
                name: "IX_StockTransactions_BatchId",
                table: "StockTransactions",
                newName: "IX_StockTransactions_BatchID");

            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "ItemBatches",
                newName: "ItemID");

            migrationBuilder.RenameColumn(
                name: "BatchId",
                table: "ItemBatches",
                newName: "BatchID");

            migrationBuilder.RenameIndex(
                name: "IX_ItemBatches_ItemId",
                table: "ItemBatches",
                newName: "IX_ItemBatches_ItemID");

            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "InventoryItems",
                newName: "ItemID");

            migrationBuilder.AlterColumn<string>(
                name: "VendorName",
                table: "Vendors",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "Vendors",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Vendors",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPerson",
                table: "Vendors",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "StockTransactions",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValueSql: "(getdate())");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "StaffDetails",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Patients",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "MedicalRecords",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<decimal>(
                name: "SalePrice",
                table: "ItemBatches",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "UnitOfMeasure",
                table: "InventoryItems",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ItemName",
                table: "InventoryItems",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<int>(
                name: "VendorID",
                table: "InventoryItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK__Vendors__FC8653AD1D3DE96A",
                table: "Vendors",
                column: "VendorID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__StockTra__55433B6A7F2D065C",
                table: "StockTransactions",
                column: "TransactionID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__ItemBatc__B28D3F372095039A",
                table: "ItemBatches",
                column: "BatchID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Inventory__7BF7D5B66736C561",
                table: "InventoryItems",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_VendorID",
                table: "InventoryItems",
                column: "VendorID");

            migrationBuilder.AddForeignKey(
                name: "FK__Inventory__Vendo__6EF57B66",
                table: "InventoryItems",
                column: "VendorID",
                principalTable: "Vendors",
                principalColumn: "VendorID");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemBatches_Vendors_VendorId",
                table: "ItemBatches",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "VendorID");

            migrationBuilder.AddForeignKey(
                name: "FK__ItemBatch__ItemI__72C60C4A",
                table: "ItemBatches",
                column: "ItemID",
                principalTable: "InventoryItems",
                principalColumn: "ItemID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransactions_Patients_PatientId",
                table: "StockTransactions",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientID");

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransactions_StaffDetails_StaffId",
                table: "StockTransactions",
                column: "StaffId",
                principalTable: "StaffDetails",
                principalColumn: "StaffID");

            migrationBuilder.AddForeignKey(
                name: "FK__StockTran__Batch__75A278FBC",
                table: "StockTransactions",
                column: "BatchID",
                principalTable: "ItemBatches",
                principalColumn: "BatchID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Inventory__Vendo__6EF57B66",
                table: "InventoryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemBatches_Vendors_VendorId",
                table: "ItemBatches");

            migrationBuilder.DropForeignKey(
                name: "FK__ItemBatch__ItemI__72C60C4A",
                table: "ItemBatches");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransactions_Patients_PatientId",
                table: "StockTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransactions_StaffDetails_StaffId",
                table: "StockTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK__StockTran__Batch__75A278FBC",
                table: "StockTransactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Vendors__FC8653AD1D3DE96A",
                table: "Vendors");

            migrationBuilder.DropPrimaryKey(
                name: "PK__StockTra__55433B6A7F2D065C",
                table: "StockTransactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK__ItemBatc__B28D3F372095039A",
                table: "ItemBatches");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Inventory__7BF7D5B66736C561",
                table: "InventoryItems");

            migrationBuilder.DropIndex(
                name: "IX_InventoryItems_VendorID",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "ContactPerson",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "SalePrice",
                table: "ItemBatches");

            migrationBuilder.DropColumn(
                name: "VendorID",
                table: "InventoryItems");

            migrationBuilder.RenameColumn(
                name: "VendorID",
                table: "Vendors",
                newName: "VendorId");

            migrationBuilder.RenameColumn(
                name: "BatchID",
                table: "StockTransactions",
                newName: "BatchId");

            migrationBuilder.RenameColumn(
                name: "TransactionID",
                table: "StockTransactions",
                newName: "TransactionId");

            migrationBuilder.RenameIndex(
                name: "IX_StockTransactions_BatchID",
                table: "StockTransactions",
                newName: "IX_StockTransactions_BatchId");

            migrationBuilder.RenameColumn(
                name: "ItemID",
                table: "ItemBatches",
                newName: "ItemId");

            migrationBuilder.RenameColumn(
                name: "BatchID",
                table: "ItemBatches",
                newName: "BatchId");

            migrationBuilder.RenameIndex(
                name: "IX_ItemBatches_ItemID",
                table: "ItemBatches",
                newName: "IX_ItemBatches_ItemId");

            migrationBuilder.RenameColumn(
                name: "ItemID",
                table: "InventoryItems",
                newName: "ItemId");

            migrationBuilder.AlterColumn<string>(
                name: "VendorName",
                table: "Vendors",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "Vendors",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Vendors",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "StockTransactions",
                type: "datetime",
                nullable: false,
                defaultValueSql: "(getdate())",
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "StaffDetails",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Patients",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "MedicalRecords",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "UnitOfMeasure",
                table: "InventoryItems",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "ItemName",
                table: "InventoryItems",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vendors",
                table: "Vendors",
                column: "VendorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StockTransactions",
                table: "StockTransactions",
                column: "TransactionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemBatches",
                table: "ItemBatches",
                column: "BatchId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InventoryItems",
                table: "InventoryItems",
                column: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemBatch_InventoryItem",
                table: "ItemBatches",
                column: "ItemId",
                principalTable: "InventoryItems",
                principalColumn: "ItemId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemBatch_Vendor",
                table: "ItemBatches",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "VendorId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransaction_ItemBatch",
                table: "StockTransactions",
                column: "BatchId",
                principalTable: "ItemBatches",
                principalColumn: "BatchId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransaction_Patient",
                table: "StockTransactions",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransaction_StaffDetail",
                table: "StockTransactions",
                column: "StaffId",
                principalTable: "StaffDetails",
                principalColumn: "StaffID",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
