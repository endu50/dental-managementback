using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentalDana.Migrations
{
    /// <inheritdoc />
    public partial class Appointment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Appoints",
                columns: table => new
                {
                    appointId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    patientName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    appointmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    treatmentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    dentistName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appoints", x => x.appointId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Appoints");
        }
    }
}
