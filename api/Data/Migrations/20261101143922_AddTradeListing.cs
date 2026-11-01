using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTradeListing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TradeListings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TrainerId = table.Column<string>(type: "text", nullable: false),
                    PokemonId = table.Column<int>(type: "integer", nullable: false),
                    Kind = table.Column<string>(type: "text", nullable: false),
                    IsShiny = table.Column<bool>(type: "boolean", nullable: true),
                    HasCostume = table.Column<bool>(type: "boolean", nullable: true),
                    CostumeId = table.Column<int>(type: "integer", nullable: true),
                    MaxType = table.Column<string>(type: "text", nullable: true),
                    HasBackground = table.Column<bool>(type: "boolean", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeListings", x => x.Id);
                    table.CheckConstraint("CK_TradeListing_CostumeRequiresHasCostume", "\"CostumeId\" IS NULL OR \"HasCostume\" = true");
                    table.CheckConstraint("CK_TradeListing_HaveAttributesSet", "\"Kind\" <> 'Have' OR (\"IsShiny\" IS NOT NULL AND \"HasCostume\" IS NOT NULL AND \"MaxType\" IS NOT NULL AND \"HasBackground\" IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_TradeListings_PokemonCostumes_PokemonId_CostumeId",
                        columns: x => new { x.PokemonId, x.CostumeId },
                        principalTable: "PokemonCostumes",
                        principalColumns: new[] { "PokemonId", "CostumeId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TradeListings_Pokemon_PokemonId",
                        column: x => x.PokemonId,
                        principalTable: "Pokemon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TradeListings_Trainers_TrainerId",
                        column: x => x.TrainerId,
                        principalTable: "Trainers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TradeListings_PokemonId_CostumeId",
                table: "TradeListings",
                columns: new[] { "PokemonId", "CostumeId" });

            migrationBuilder.CreateIndex(
                name: "IX_TradeListings_PokemonId_Kind_Status",
                table: "TradeListings",
                columns: new[] { "PokemonId", "Kind", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_TradeListings_TrainerId",
                table: "TradeListings",
                column: "TrainerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TradeListings");
        }
    }
}
