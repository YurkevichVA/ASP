using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP_201.Migrations
{
    /// <inheritdoc />
    public partial class AddTopicAuthor_Nav : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Topics_AuthorId",
                table: "Topics",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Themes_AuthorId",
                table: "Themes",
                column: "AuthorId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Themes_Users_AuthorId",
            //    table: "Themes",
            //    column: "AuthorId",
            //    principalTable: "Users",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Topics_Users_AuthorId",
            //    table: "Topics",
            //    column: "AuthorId",
            //    principalTable: "Users",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Themes_Users_AuthorId",
            //    table: "Themes");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_Topics_Users_AuthorId",
            //    table: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_Topics_AuthorId",
                table: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_Themes_AuthorId",
                table: "Themes");
        }
    }
}
