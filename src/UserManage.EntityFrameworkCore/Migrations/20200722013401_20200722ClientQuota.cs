using Microsoft.EntityFrameworkCore.Migrations;

namespace UserManage.Migrations
{
    public partial class _20200722ClientQuota : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClientQuota",
                table: "AbpUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientQuota",
                table: "AbpUsers");
        }
    }
}
