using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPokemonReference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Costumes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    NoEvolve = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Costumes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pokemon",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DexNumber = table.Column<int>(type: "integer", nullable: false),
                    FormCode = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsReleased = table.Column<bool>(type: "boolean", nullable: false),
                    ShinyAvailable = table.Column<bool>(type: "boolean", nullable: false),
                    DynamaxAvailable = table.Column<bool>(type: "boolean", nullable: false),
                    GigantamaxAvailable = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pokemon", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PokemonCostumes",
                columns: table => new
                {
                    PokemonId = table.Column<int>(type: "integer", nullable: false),
                    CostumeId = table.Column<int>(type: "integer", nullable: false),
                    SpriteCode = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokemonCostumes", x => new { x.PokemonId, x.CostumeId });
                    table.ForeignKey(
                        name: "FK_PokemonCostumes_Costumes_CostumeId",
                        column: x => x.CostumeId,
                        principalTable: "Costumes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PokemonCostumes_Pokemon_PokemonId",
                        column: x => x.PokemonId,
                        principalTable: "Pokemon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Costumes_Code",
                table: "Costumes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_DexNumber_FormCode",
                table: "Pokemon",
                columns: new[] { "DexNumber", "FormCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PokemonCostumes_CostumeId",
                table: "PokemonCostumes",
                column: "CostumeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PokemonCostumes");

            migrationBuilder.DropTable(
                name: "Costumes");

            migrationBuilder.DropTable(
                name: "Pokemon");
        }
    }
}
