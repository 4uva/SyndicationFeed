using Microsoft.EntityFrameworkCore.Migrations;

namespace SyndicationFeed.Server.Migrations
{
    public partial class DeclaredStringPropertiesAsRequiredAndRenamedTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FeedWithDownloadTime_Collections_CollectionWithFeedsId",
                table: "FeedWithDownloadTime");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FeedWithDownloadTime",
                table: "FeedWithDownloadTime");

            migrationBuilder.RenameTable(
                name: "FeedWithDownloadTime",
                newName: "Feeds");

            migrationBuilder.RenameIndex(
                name: "IX_FeedWithDownloadTime_CollectionWithFeedsId",
                table: "Feeds",
                newName: "IX_Feeds_CollectionWithFeedsId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Collections",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SourceAddressString",
                table: "Feeds",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Feeds",
                table: "Feeds",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Feeds_Collections_CollectionWithFeedsId",
                table: "Feeds",
                column: "CollectionWithFeedsId",
                principalTable: "Collections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feeds_Collections_CollectionWithFeedsId",
                table: "Feeds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Feeds",
                table: "Feeds");

            migrationBuilder.RenameTable(
                name: "Feeds",
                newName: "FeedWithDownloadTime");

            migrationBuilder.RenameIndex(
                name: "IX_Feeds_CollectionWithFeedsId",
                table: "FeedWithDownloadTime",
                newName: "IX_FeedWithDownloadTime_CollectionWithFeedsId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Collections",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "SourceAddressString",
                table: "FeedWithDownloadTime",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddPrimaryKey(
                name: "PK_FeedWithDownloadTime",
                table: "FeedWithDownloadTime",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FeedWithDownloadTime_Collections_CollectionWithFeedsId",
                table: "FeedWithDownloadTime",
                column: "CollectionWithFeedsId",
                principalTable: "Collections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
