using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Redis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "aidemo");

            migrationBuilder.CreateTable(
                name: "styles",
                schema: "aidemo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArticleType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BaseColour = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MasterCategory = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProductDisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Season = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SubCategory = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Usage = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_styles", x => x.Id);
                });
            
            migrationBuilder.Sql(@"
                    ALTER DATABASE RedisAiDb
                    SET CHANGE_TRACKING = ON
                    (CHANGE_RETENTION = 2 DAYS, AUTO_CLEANUP = ON);
            ", suppressTransaction:true);
            
            migrationBuilder.Sql(@"
                    ALTER TABLE aidemo.styles
                    ENABLE CHANGE_TRACKING
                    WITH (TRACK_COLUMNS_UPDATED = ON);
            ", suppressTransaction:true);
          
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Disable Change Tracking on table
            migrationBuilder.Sql(@"
                ALTER TABLE aidemo.styles
                DISABLE CHANGE_TRACKING;
            ", suppressTransaction:true);
            
            migrationBuilder.Sql(@"
                ALTER DATABASE RedisAiDb
                SET CHANGE_TRACKING = OFF;
            ", suppressTransaction:true);
            
            migrationBuilder.DropTable(
                name: "styles",
                schema: "aidemo");
        }
    }
}
