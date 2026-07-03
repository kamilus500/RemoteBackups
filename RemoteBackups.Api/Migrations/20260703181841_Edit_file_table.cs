using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemoteBackups.Api.Migrations
{
    /// <inheritdoc />
    public partial class Edit_file_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "SizeInBytes",
                table: "FileMetaDatas",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SizeInBytes",
                table: "FileMetaDatas");
        }
    }
}
