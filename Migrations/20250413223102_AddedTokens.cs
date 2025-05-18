﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP_SPR311.Migrations
{
    /// <inheritdoc />
    public partial class AddedTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccessTokens",
                schema: "ASP",
                columns: table => new
                {
                    Jti = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Sub = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Aud = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Iat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Nbf = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Exp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Iss = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessTokens", x => x.Jti);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessTokens",
                schema: "ASP");
        }
    }
}
