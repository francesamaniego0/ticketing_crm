using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Migrations
{
    /// <inheritdoc />
    public partial class AddTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientModels",
                table: "ClientModels");

            migrationBuilder.DropColumn(
                name: "Fname",
                table: "ClientModels");

            migrationBuilder.DropColumn(
                name: "Lname",
                table: "ClientModels");

            migrationBuilder.RenameTable(
                name: "ClientModels",
                newName: "TblClient");

            migrationBuilder.RenameColumn(
                name: "Suffix",
                table: "TblClient",
                newName: "Location");

            migrationBuilder.RenameColumn(
                name: "Mname",
                table: "TblClient",
                newName: "ClientName");

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "TblClient",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "TblClient",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateDeleted",
                table: "TblClient",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "TblClient",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DeleteFlag",
                table: "TblClient",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "TblClient",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "TblClient",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "TblClient",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TblClient",
                table: "TblClient",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "TblClientContactPersons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    Fullname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    DeleteFlag = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: false),
                    DateDeleted = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblClientContactPersons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TblTeam",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeamName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SuperVisorId = table.Column<int>(type: "int", nullable: false),
                    SuperVisorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    DeleteFlag = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: false),
                    DateDeleted = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblTeam", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TblClientContactPersons");

            migrationBuilder.DropTable(
                name: "TblTeam");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TblClient",
                table: "TblClient");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "TblClient");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "TblClient");

            migrationBuilder.DropColumn(
                name: "DateDeleted",
                table: "TblClient");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "TblClient");

            migrationBuilder.DropColumn(
                name: "DeleteFlag",
                table: "TblClient");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "TblClient");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "TblClient");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "TblClient");

            migrationBuilder.RenameTable(
                name: "TblClient",
                newName: "ClientModels");

            migrationBuilder.RenameColumn(
                name: "Location",
                table: "ClientModels",
                newName: "Suffix");

            migrationBuilder.RenameColumn(
                name: "ClientName",
                table: "ClientModels",
                newName: "Mname");

            migrationBuilder.AddColumn<string>(
                name: "Fname",
                table: "ClientModels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Lname",
                table: "ClientModels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientModels",
                table: "ClientModels",
                column: "Id");
        }
    }
}
