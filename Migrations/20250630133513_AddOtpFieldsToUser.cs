﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentalDana.Migrations
{
    /// <inheritdoc />
    public partial class AddOtpFieldsToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OtpCode",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "OtpExpiresAt",
                table: "Users",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OtpCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OtpExpiresAt",
                table: "Users");
        }
    }
}
