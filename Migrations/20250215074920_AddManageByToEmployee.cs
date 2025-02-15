using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMSystemTask.Migrations
{
    /// <inheritdoc />
    public partial class AddManageByToEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ManageBy",
                table: "Employees",
                type: "int",
                nullable: true
    );
            migrationBuilder.CreateIndex(
                name: "IX_Employees_ManageBy",
                table: "Employees",
                column: "ManageBy");
            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Employees_ManageBy",
                table: "Employees",
                column: "ManageBy",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.NoAction
                );
        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Employees_ManageBy",
                table: "Employees"
                );
            migrationBuilder.DropIndex(
                name: "IX_Employees_ManageBy",
                table: "Employees"
                );
            migrationBuilder.DropColumn(
                name: "ManageBy",
                table: "Employees"
                );


        }
    }
}
