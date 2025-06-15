using Microsoft.EntityFrameworkCore.Migrations;

namespace ASL.LivingGrid.WebAdminPanel.Migrations;

public partial class AddMachineApprovedStatus : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Enum extended to include MachineApproved and HumanApproved
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // No schema changes to revert
    }
}
