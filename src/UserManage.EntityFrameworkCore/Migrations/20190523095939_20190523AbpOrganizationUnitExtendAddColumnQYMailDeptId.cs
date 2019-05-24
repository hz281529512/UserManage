using Microsoft.EntityFrameworkCore.Migrations;

namespace UserManage.Migrations
{
    public partial class _20190523AbpOrganizationUnitExtendAddColumnQYMailDeptId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "QYMailDeptId",
                table: "AbpOrganizationUnits",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QYMailDeptId",
                table: "AbpOrganizationUnits");
        }
    }
}
