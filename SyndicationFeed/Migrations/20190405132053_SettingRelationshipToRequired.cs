using Microsoft.EntityFrameworkCore.Migrations;

namespace SyndicationFeed.Server.Migrations
{
    public partial class SettingRelationshipToRequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FeedWithDownloadTime_Collections_CollectionWithFeedsId",
                table: "FeedWithDownloadTime");

            migrationBuilder.AlterColumn<long>(
                name: "CollectionWithFeedsId",
                table: "FeedWithDownloadTime",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FeedWithDownloadTime_Collections_CollectionWithFeedsId",
                table: "FeedWithDownloadTime",
                column: "CollectionWithFeedsId",
                principalTable: "Collections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FeedWithDownloadTime_Collections_CollectionWithFeedsId",
                table: "FeedWithDownloadTime");

            migrationBuilder.AlterColumn<long>(
                name: "CollectionWithFeedsId",
                table: "FeedWithDownloadTime",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddForeignKey(
                name: "FK_FeedWithDownloadTime_Collections_CollectionWithFeedsId",
                table: "FeedWithDownloadTime",
                column: "CollectionWithFeedsId",
                principalTable: "Collections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
