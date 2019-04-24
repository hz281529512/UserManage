using Microsoft.EntityFrameworkCore.Migrations;

namespace UserManage.Migrations
{
    public partial class TryModifyRole20190424 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TagId",
                table: "AbpRoles");

            migrationBuilder.AddColumn<string>(
                name: "WxTagId",
                table: "AbpRoles",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WxTagId",
                table: "AbpRoles");

            migrationBuilder.AddColumn<int>(
                name: "TagId",
                table: "AbpRoles",
                nullable: true);
        }
    }
}
