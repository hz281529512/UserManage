using Microsoft.EntityFrameworkCore.Migrations;

namespace UserManage.Migrations
{
    public partial class _20190423ManyTableCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyId",
                table: "AbpUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Position",
                table: "AbpUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SelectDistrict",
                table: "AbpUsers",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Sex",
                table: "AbpUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "RoleType",
                table: "AbpRoles",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TagId",
                table: "AbpRoles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AbpOrganizationUnits",
                nullable: false,
                defaultValue: "AbpOrganizationUnitExtend");

            migrationBuilder.AddColumn<int>(
                name: "WXDeptId",
                table: "AbpOrganizationUnits",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WXParentDeptId",
                table: "AbpOrganizationUnits",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "SelectDistrict",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "Sex",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "RoleType",
                table: "AbpRoles");

            migrationBuilder.DropColumn(
                name: "TagId",
                table: "AbpRoles");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AbpOrganizationUnits");

            migrationBuilder.DropColumn(
                name: "WXDeptId",
                table: "AbpOrganizationUnits");

            migrationBuilder.DropColumn(
                name: "WXParentDeptId",
                table: "AbpOrganizationUnits");
        }
    }
}
