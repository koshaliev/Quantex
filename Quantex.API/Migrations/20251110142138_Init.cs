using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quantex.API.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "main");

            migrationBuilder.CreateTable(
                name: "ExpenseSchemeModel",
                schema: "main",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseSchemeModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExpenseProfileModel",
                schema: "main",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ConditionJSON = table.Column<string>(type: "json", nullable: false),
                    SchemeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseProfileModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpenseProfileModel_ExpenseSchemeModel_SchemeId",
                        column: x => x.SchemeId,
                        principalSchema: "main",
                        principalTable: "ExpenseSchemeModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExpenseGroupModel",
                schema: "main",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ConditionJSON = table.Column<string>(type: "json", nullable: true),
                    ProfileId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseGroupModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpenseGroupModel_ExpenseProfileModel_ProfileId",
                        column: x => x.ProfileId,
                        principalSchema: "main",
                        principalTable: "ExpenseProfileModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExpenseUnits",
                schema: "main",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ConditionJSON = table.Column<string>(type: "json", nullable: true),
                    CalculationJSON = table.Column<string>(type: "json", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ValidFrom = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ValidTo = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpenseUnits_ExpenseGroupModel_GroupId",
                        column: x => x.GroupId,
                        principalSchema: "main",
                        principalTable: "ExpenseGroupModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseGroupModel_ProfileId",
                schema: "main",
                table: "ExpenseGroupModel",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseProfileModel_SchemeId",
                schema: "main",
                table: "ExpenseProfileModel",
                column: "SchemeId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseUnits_GroupId",
                schema: "main",
                table: "ExpenseUnits",
                column: "GroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExpenseUnits",
                schema: "main");

            migrationBuilder.DropTable(
                name: "ExpenseGroupModel",
                schema: "main");

            migrationBuilder.DropTable(
                name: "ExpenseProfileModel",
                schema: "main");

            migrationBuilder.DropTable(
                name: "ExpenseSchemeModel",
                schema: "main");
        }
    }
}
