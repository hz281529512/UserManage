using Microsoft.EntityFrameworkCore.Migrations;

namespace UserManage.Migrations
{
    public partial class TryModifyRole20190424A : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "WxTagId",
                table: "AbpRoles",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "WxTagId",
                table: "AbpRoles",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
