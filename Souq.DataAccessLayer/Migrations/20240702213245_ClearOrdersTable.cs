using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Souq.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class ClearOrdersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM OrderHeaders");
            migrationBuilder.Sql("DELETE FROM OrderDetails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
