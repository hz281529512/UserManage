﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace UserManage.Migrations
{
    public partial class _20190605ServiceConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AbpServiceConfig",
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
                    LocalName = table.Column<string>(nullable: true),
                    AppName = table.Column<string>(nullable: true),
                    LoginProvider = table.Column<string>(nullable: true),
                    CropId = table.Column<string>(nullable: true),
                    AppId = table.Column<string>(nullable: true),
                    Secret = table.Column<string>(nullable: true),
                    IsEnabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpServiceConfig", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AbpServiceConfig");
        }
    }
}
