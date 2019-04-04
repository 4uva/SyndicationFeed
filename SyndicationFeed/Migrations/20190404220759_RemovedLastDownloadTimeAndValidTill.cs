using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SyndicationFeed.Server.Migrations
{
    public partial class RemovedLastDownloadTimeAndValidTill : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastDownloadTime",
                table: "FeedWithDownloadTime");

            migrationBuilder.DropColumn(
                name: "ValidTill",
                table: "FeedWithDownloadTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastDownloadTime",
                table: "FeedWithDownloadTime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidTill",
                table: "FeedWithDownloadTime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
