using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservasApi.Migrations
{
    // La clase hereda de Migration, lo que la identifica como una migración de EF Core.
    public partial class InicialDocker : Migration
    {

        // El método UP se ejecuta cuando aplicas la migración (crear/actualizar la DB).
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Creación de la tabla 'Espacios'
            migrationBuilder.CreateTable(
                name: "Espacios",
                columns: table => new
                {
                    // Define la columna 'Id' como clave primaria (PK) y con auto-incremento (Identity)
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    // Columna 'Nombre', tipo string (nvarchar en SQL), longitud máxima de 100 caracteres, y NO NULO.
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),

                    // Columna 'Descripcion', tipo string, longitud máxima de 250 caracteres, y NO NULO.
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),

                    // Columna 'Capacidad', tipo entero (int), NO NULO.
                    Capacidad = table.Column<int>(type: "int", nullable: false),

                    // Columna 'Disponible', tipo booleano (bit en SQL), NO NULO.
                    Disponible = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    // Establece la columna 'Id' como la clave primaria de la tabla 'Espacios'.
                    table.PrimaryKey("PK_Espacios", x => x.Id);
                });

            // 2. Creación de la tabla 'Usuarios'
            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    // Define la columna 'Id' como clave primaria (PK) y con auto-incremento (Identity)
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    // Columna 'Nombre', tipo string, longitud máxima de 100 caracteres, y NO NULO.
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),

                    // Columna 'Email', tipo string, NO NULO.
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),

                    // Columna 'Rol', tipo entero (int), NO NULO (probablemente un Enum mapeado a int).
                    Rol = table.Column<int>(type: "int", nullable: false),

                    // Columna 'FechaRegistro', tipo fecha y hora (datetime2 en SQL), NO NULO.
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    // Establece la columna 'Id' como la clave primaria de la tabla 'Usuarios'.
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            // 3. Creación de la tabla 'Reservas'
            migrationBuilder.CreateTable(
                name: "Reservas",
                columns: table => new
                {
                    // Define la columna 'Id' como clave primaria (PK) y con auto-incremento (Identity)
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    // Clave Foránea a la tabla 'Usuarios'
                    UsuarioId = table.Column<int>(type: "int", nullable: false),

                    // Clave Foránea a la tabla 'Espacios'
                    EspacioId = table.Column<int>(type: "int", nullable: false),

                    // Columna 'FechaInicio', tipo fecha y hora, NO NULO.
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),

                    // Columna 'FechaFin', tipo fecha y hora, NO NULO.
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false),

                    // Columna 'Estado', tipo entero, NO NULO (probablemente un Enum mapeado a int).
                    Estado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    // Establece la columna 'Id' como la clave primaria de la tabla 'Reservas'.
                    table.PrimaryKey("PK_Reservas", x => x.Id);

                    // Define la relación de clave foránea (FK) con la tabla 'Espacios'.
                    // onDelete: ReferentialAction.Cascade: Si se elimina un Espacio, se eliminan todas sus Reservas asociadas.
                    table.ForeignKey(
                        name: "FK_Reservas_Espacios_EspacioId",
                        column: x => x.EspacioId,
                        principalTable: "Espacios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);

                    // Define la relación de clave foránea (FK) con la tabla 'Usuarios'.
                    // onDelete: ReferentialAction.Cascade: Si se elimina un Usuario, se eliminan todas sus Reservas asociadas.
                    table.ForeignKey(
                        name: "FK_Reservas_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Creación de índices para optimizar búsquedas.

            // Crea un índice en la columna 'EspacioId' de la tabla 'Reservas'.
            migrationBuilder.CreateIndex(
                name: "IX_Reservas_EspacioId",
                table: "Reservas",
                column: "EspacioId");

            // Crea un índice en la columna 'UsuarioId' de la tabla 'Reservas'.
            migrationBuilder.CreateIndex(
                name: "IX_Reservas_UsuarioId",
                table: "Reservas",
                column: "UsuarioId");
        }

        // El método DOWN se ejecuta cuando reviertes (deshaces) la migración.
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Elimina la tabla 'Reservas' (primero, ya que depende de 'Espacios' y 'Usuarios').
            migrationBuilder.DropTable(
                name: "Reservas");

            // Elimina la tabla 'Espacios'.
            migrationBuilder.DropTable(
                name: "Espacios");

            // Elimina la tabla 'Usuarios'.
            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
