using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTradeAndMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Trades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InitiatorId = table.Column<string>(type: "text", nullable: false),
                    ResponderId = table.Column<string>(type: "text", nullable: false),
                    InitiatorListingId = table.Column<int>(type: "integer", nullable: true),
                    ResponderListingId = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trades", x => x.Id);
                    table.CheckConstraint("CK_Trade_DifferentTrainers", "\"InitiatorId\" <> \"ResponderId\"");
                    table.ForeignKey(
                        name: "FK_Trades_TradeListings_InitiatorListingId",
                        column: x => x.InitiatorListingId,
                        principalTable: "TradeListings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Trades_TradeListings_ResponderListingId",
                        column: x => x.ResponderListingId,
                        principalTable: "TradeListings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Trades_Trainers_InitiatorId",
                        column: x => x.InitiatorId,
                        principalTable: "Trainers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Trades_Trainers_ResponderId",
                        column: x => x.ResponderId,
                        principalTable: "Trainers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TradeMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TradeId = table.Column<int>(type: "integer", nullable: false),
                    SenderId = table.Column<string>(type: "text", nullable: false),
                    Body = table.Column<string>(type: "text", nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TradeMessages_Trades_TradeId",
                        column: x => x.TradeId,
                        principalTable: "Trades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TradeMessages_Trainers_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Trainers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TradeMessages_SenderId",
                table: "TradeMessages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_TradeMessages_TradeId_SentAt",
                table: "TradeMessages",
                columns: new[] { "TradeId", "SentAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Trades_InitiatorId",
                table: "Trades",
                column: "InitiatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Trades_InitiatorListingId",
                table: "Trades",
                column: "InitiatorListingId");

            migrationBuilder.CreateIndex(
                name: "IX_Trades_ResponderId",
                table: "Trades",
                column: "ResponderId");

            migrationBuilder.CreateIndex(
                name: "IX_Trades_ResponderListingId",
                table: "Trades",
                column: "ResponderListingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TradeMessages");

            migrationBuilder.DropTable(
                name: "Trades");
        }
    }
}
