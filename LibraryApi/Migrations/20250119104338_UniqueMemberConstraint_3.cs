using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryApi.Migrations
{
    /// <inheritdoc />
    public partial class UniqueMemberConstraint_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Members_CardNumber",
                table: "Members");

            migrationBuilder.AlterColumn<string>(
                name: "CardNumber",
                table: "Members",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Members_CardNumber",
                table: "Members",
                column: "CardNumber",
                unique: true,
                filter: "[CardNumber] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Members_CardNumber",
                table: "Members");

            migrationBuilder.AlterColumn<string>(
                name: "CardNumber",
                table: "Members",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Members_CardNumber",
                table: "Members",
                column: "CardNumber",
                unique: true);
        }
    }
}
