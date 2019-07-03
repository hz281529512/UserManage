using Microsoft.EntityFrameworkCore.Migrations;

namespace UserManage.Migrations
{
    public partial class _20190619BaseUserOrg3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BaseUserGuid",
                table: "BaseUserEmpOrg",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseUserGuid",
                table: "BaseUserEmpOrg");
        }
    }
}
