using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeClassPlatform.BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLegacyStorageAssets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Assets_ProfileImageAssetId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Assets_CoverAssetId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Assets_ThumbnailAssetId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Lectures_Assets_ThumbnailAssetId",
                table: "Lectures");

            migrationBuilder.DropForeignKey(
                name: "FK_Lectures_Assets_VideoAssetId",
                table: "Lectures");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentProfiles_Assets_ProfileImageAssetId",
                table: "StudentProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherProfiles_Assets_ProfileImageAssetId",
                table: "TeacherProfiles");

            migrationBuilder.DropTable(
                name: "Assets");

            migrationBuilder.CreateTable(
                name: "ExternalResources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Provider = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ResourceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OriginalUrl = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    CanonicalUrl = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ContentType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: true),
                    DurationSeconds = table.Column<int>(type: "int", nullable: true),
                    ThumbnailUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TitleEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TitleAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DescriptionAr = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    VerificationStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastVerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedByAccountId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MetadataJson = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalResources", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_ExternalResources_ProfileImageAssetId",
                table: "Accounts",
                column: "ProfileImageAssetId",
                principalTable: "ExternalResources",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_ExternalResources_CoverAssetId",
                table: "Courses",
                column: "CoverAssetId",
                principalTable: "ExternalResources",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_ExternalResources_ThumbnailAssetId",
                table: "Courses",
                column: "ThumbnailAssetId",
                principalTable: "ExternalResources",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lectures_ExternalResources_ThumbnailAssetId",
                table: "Lectures",
                column: "ThumbnailAssetId",
                principalTable: "ExternalResources",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lectures_ExternalResources_VideoAssetId",
                table: "Lectures",
                column: "VideoAssetId",
                principalTable: "ExternalResources",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentProfiles_ExternalResources_ProfileImageAssetId",
                table: "StudentProfiles",
                column: "ProfileImageAssetId",
                principalTable: "ExternalResources",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherProfiles_ExternalResources_ProfileImageAssetId",
                table: "TeacherProfiles",
                column: "ProfileImageAssetId",
                principalTable: "ExternalResources",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_ExternalResources_ProfileImageAssetId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_ExternalResources_CoverAssetId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_ExternalResources_ThumbnailAssetId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Lectures_ExternalResources_ThumbnailAssetId",
                table: "Lectures");

            migrationBuilder.DropForeignKey(
                name: "FK_Lectures_ExternalResources_VideoAssetId",
                table: "Lectures");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentProfiles_ExternalResources_ProfileImageAssetId",
                table: "StudentProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherProfiles_ExternalResources_ProfileImageAssetId",
                table: "TeacherProfiles");

            migrationBuilder.DropTable(
                name: "ExternalResources");

            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByAccountId = table.Column<int>(type: "int", nullable: true),
                    DurationSeconds = table.Column<int>(type: "int", nullable: true),
                    ExternalId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Height = table.Column<int>(type: "int", nullable: true),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    IsReady = table.Column<bool>(type: "bit", nullable: false),
                    MetadataJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ObjectKey = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OriginalFileName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Provider = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UploadStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Width = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Assets_ProfileImageAssetId",
                table: "Accounts",
                column: "ProfileImageAssetId",
                principalTable: "Assets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Assets_CoverAssetId",
                table: "Courses",
                column: "CoverAssetId",
                principalTable: "Assets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Assets_ThumbnailAssetId",
                table: "Courses",
                column: "ThumbnailAssetId",
                principalTable: "Assets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lectures_Assets_ThumbnailAssetId",
                table: "Lectures",
                column: "ThumbnailAssetId",
                principalTable: "Assets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lectures_Assets_VideoAssetId",
                table: "Lectures",
                column: "VideoAssetId",
                principalTable: "Assets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentProfiles_Assets_ProfileImageAssetId",
                table: "StudentProfiles",
                column: "ProfileImageAssetId",
                principalTable: "Assets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherProfiles_Assets_ProfileImageAssetId",
                table: "TeacherProfiles",
                column: "ProfileImageAssetId",
                principalTable: "Assets",
                principalColumn: "Id");
        }
    }
}
