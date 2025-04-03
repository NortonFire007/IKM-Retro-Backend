using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IKM_Retro.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRefreshTokenTableUserNameColToUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens");

            migrationBuilder.RenameTable(
                name: "RefreshTokens",
                newName: "AspNetUserRefreshTokens");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "AspNetUserRefreshTokens",
                newName: "UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserRefreshTokens",
                table: "AspNetUserRefreshTokens",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserRefreshTokens",
                table: "AspNetUserRefreshTokens");

            migrationBuilder.RenameTable(
                name: "AspNetUserRefreshTokens",
                newName: "RefreshTokens");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "RefreshTokens",
                newName: "UserName");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens",
                column: "Id");
        }
    }
}
