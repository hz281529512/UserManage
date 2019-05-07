using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace UserManage.Migrations
{
    public partial class CreateTableAbpCompany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AbpCompany",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    CompanyNo = table.Column<string>(nullable: true),
                    CompanyName = table.Column<string>(nullable: true),
                    CompanyType = table.Column<string>(nullable: true),
                    CompanyTel = table.Column<string>(nullable: true),
                    CompanyAddress = table.Column<string>(nullable: true),
                    CompanyMail = table.Column<string>(nullable: true),
                    LegalPerson = table.Column<string>(nullable: true),
                    LegalPersonTel = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpCompany", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AbpCompany");
        }
    }
}
