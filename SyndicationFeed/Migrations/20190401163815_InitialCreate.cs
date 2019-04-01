using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SyndicationFeed.Server.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Collections",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FeedWithDownloadTime",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<int>(nullable: false),
                    LastDownloadTime = table.Column<DateTime>(nullable: false),
                    ValidTill = table.Column<DateTime>(nullable: false),
                    SourceAddressString = table.Column<string>(nullable: true),
                    CollectionWithFeedsId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedWithDownloadTime", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeedWithDownloadTime_Collections_CollectionWithFeedsId",
                        column: x => x.CollectionWithFeedsId,
                        principalTable: "Collections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FeedWithDownloadTime_CollectionWithFeedsId",
                table: "FeedWithDownloadTime",
                column: "CollectionWithFeedsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FeedWithDownloadTime");

            migrationBuilder.DropTable(
                name: "Collections");
        }
    }
}
