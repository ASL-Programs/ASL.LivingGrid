using Microsoft.EntityFrameworkCore.Migrations;

namespace ASL.LivingGrid.WebAdminPanel.Migrations;

public partial class AddPendingReviewStatus : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // No schema changes required; enum value renamed to PendingReview
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // No schema changes to revert
    }
}
