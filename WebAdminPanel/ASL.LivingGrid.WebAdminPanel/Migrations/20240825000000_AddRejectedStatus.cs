using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ASL.LivingGrid.WebAdminPanel.Migrations;

public partial class AddRejectedStatus : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "RejectedBy",
            table: "TranslationRequests",
            type: "nvarchar(100)",
            maxLength: 100,
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "RejectedAt",
            table: "TranslationRequests",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "RejectedBy",
            table: "TranslationRequests");

        migrationBuilder.DropColumn(
            name: "RejectedAt",
            table: "TranslationRequests");
    }
}
