using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quantex.API.Migrations
{
    /// <inheritdoc />
    public partial class AddGroups_Profiles_Schemes_Tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseGroupModel_ExpenseProfileModel_ProfileId",
                schema: "main",
                table: "ExpenseGroupModel");

            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseProfileModel_ExpenseSchemeModel_SchemeId",
                schema: "main",
                table: "ExpenseProfileModel");

            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseUnits_ExpenseGroupModel_GroupId",
                schema: "main",
                table: "ExpenseUnits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExpenseSchemeModel",
                schema: "main",
                table: "ExpenseSchemeModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExpenseProfileModel",
                schema: "main",
                table: "ExpenseProfileModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExpenseGroupModel",
                schema: "main",
                table: "ExpenseGroupModel");

            migrationBuilder.RenameTable(
                name: "ExpenseSchemeModel",
                schema: "main",
                newName: "ExpenseSchemes",
                newSchema: "main");

            migrationBuilder.RenameTable(
                name: "ExpenseProfileModel",
                schema: "main",
                newName: "ExpenseProfiles",
                newSchema: "main");

            migrationBuilder.RenameTable(
                name: "ExpenseGroupModel",
                schema: "main",
                newName: "ExpenseGroups",
                newSchema: "main");

            migrationBuilder.RenameIndex(
                name: "IX_ExpenseProfileModel_SchemeId",
                schema: "main",
                table: "ExpenseProfiles",
                newName: "IX_ExpenseProfiles_SchemeId");

            migrationBuilder.RenameIndex(
                name: "IX_ExpenseGroupModel_ProfileId",
                schema: "main",
                table: "ExpenseGroups",
                newName: "IX_ExpenseGroups_ProfileId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExpenseSchemes",
                schema: "main",
                table: "ExpenseSchemes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExpenseProfiles",
                schema: "main",
                table: "ExpenseProfiles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExpenseGroups",
                schema: "main",
                table: "ExpenseGroups",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseGroups_ExpenseProfiles_ProfileId",
                schema: "main",
                table: "ExpenseGroups",
                column: "ProfileId",
                principalSchema: "main",
                principalTable: "ExpenseProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseProfiles_ExpenseSchemes_SchemeId",
                schema: "main",
                table: "ExpenseProfiles",
                column: "SchemeId",
                principalSchema: "main",
                principalTable: "ExpenseSchemes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseUnits_ExpenseGroups_GroupId",
                schema: "main",
                table: "ExpenseUnits",
                column: "GroupId",
                principalSchema: "main",
                principalTable: "ExpenseGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseGroups_ExpenseProfiles_ProfileId",
                schema: "main",
                table: "ExpenseGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseProfiles_ExpenseSchemes_SchemeId",
                schema: "main",
                table: "ExpenseProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseUnits_ExpenseGroups_GroupId",
                schema: "main",
                table: "ExpenseUnits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExpenseSchemes",
                schema: "main",
                table: "ExpenseSchemes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExpenseProfiles",
                schema: "main",
                table: "ExpenseProfiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExpenseGroups",
                schema: "main",
                table: "ExpenseGroups");

            migrationBuilder.RenameTable(
                name: "ExpenseSchemes",
                schema: "main",
                newName: "ExpenseSchemeModel",
                newSchema: "main");

            migrationBuilder.RenameTable(
                name: "ExpenseProfiles",
                schema: "main",
                newName: "ExpenseProfileModel",
                newSchema: "main");

            migrationBuilder.RenameTable(
                name: "ExpenseGroups",
                schema: "main",
                newName: "ExpenseGroupModel",
                newSchema: "main");

            migrationBuilder.RenameIndex(
                name: "IX_ExpenseProfiles_SchemeId",
                schema: "main",
                table: "ExpenseProfileModel",
                newName: "IX_ExpenseProfileModel_SchemeId");

            migrationBuilder.RenameIndex(
                name: "IX_ExpenseGroups_ProfileId",
                schema: "main",
                table: "ExpenseGroupModel",
                newName: "IX_ExpenseGroupModel_ProfileId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExpenseSchemeModel",
                schema: "main",
                table: "ExpenseSchemeModel",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExpenseProfileModel",
                schema: "main",
                table: "ExpenseProfileModel",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExpenseGroupModel",
                schema: "main",
                table: "ExpenseGroupModel",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseGroupModel_ExpenseProfileModel_ProfileId",
                schema: "main",
                table: "ExpenseGroupModel",
                column: "ProfileId",
                principalSchema: "main",
                principalTable: "ExpenseProfileModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseProfileModel_ExpenseSchemeModel_SchemeId",
                schema: "main",
                table: "ExpenseProfileModel",
                column: "SchemeId",
                principalSchema: "main",
                principalTable: "ExpenseSchemeModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseUnits_ExpenseGroupModel_GroupId",
                schema: "main",
                table: "ExpenseUnits",
                column: "GroupId",
                principalSchema: "main",
                principalTable: "ExpenseGroupModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
