using Microsoft.EntityFrameworkCore.Migrations;

namespace UserManage.Migrations
{
    public partial class AlterTableThirdPartyConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AppId",
                table: "ThirdPartyConfig",
                newName: "SuiteID");

            migrationBuilder.AddColumn<string>(
                name: "EncodingAESKey",
                table: "ThirdPartyConfig",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EncodingAESKey",
                table: "ThirdPartyConfig");

            migrationBuilder.RenameColumn(
                name: "SuiteID",
                table: "ThirdPartyConfig",
                newName: "AppId");
        }
    }
}
