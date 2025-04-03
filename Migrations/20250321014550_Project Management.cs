using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Migrations
{
    /// <inheritdoc />
    public partial class ProjectManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "TblTeamMembers");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "TblTeamMembers");

            migrationBuilder.DropColumn(
                name: "DateDeleted",
                table: "TblTeamMembers");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "TblTeamMembers");

            migrationBuilder.DropColumn(
                name: "DeleteFlag",
                table: "TblTeamMembers");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "TblTeamMembers");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "TblTeamMembers");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "TblClientContactPersons");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "TblClientContactPersons");

            migrationBuilder.DropColumn(
                name: "DateDeleted",
                table: "TblClientContactPersons");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "TblClientContactPersons");

            migrationBuilder.DropColumn(
                name: "DeleteFlag",
                table: "TblClientContactPersons");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "TblClientContactPersons");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "TblClientContactPersons");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "TblClientContactPersons");

            migrationBuilder.AddColumn<int>(
                name: "ClientId",
                table: "TblTeam",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TblProjectContactPersons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: true),
                    ContactPersonId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblProjectContactPersons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TblProjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Client = table.Column<int>(type: "int", nullable: true),
                    Team = table.Column<int>(type: "int", nullable: true),
                    DeleteFlag = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true),
                    DateDeleted = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblProjects", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TblProjectContactPersons");

            migrationBuilder.DropTable(
                name: "TblProjects");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "TblTeam");

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "TblTeamMembers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "TblTeamMembers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateDeleted",
                table: "TblTeamMembers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "TblTeamMembers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DeleteFlag",
                table: "TblTeamMembers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "TblTeamMembers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "TblTeamMembers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "TblClientContactPersons",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "TblClientContactPersons",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateDeleted",
                table: "TblClientContactPersons",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "TblClientContactPersons",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DeleteFlag",
                table: "TblClientContactPersons",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "TblClientContactPersons",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "TblClientContactPersons",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "TblClientContactPersons",
                type: "int",
                nullable: true);
        }
    }
}
