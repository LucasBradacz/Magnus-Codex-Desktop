using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagnusCodex.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Formas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", nullable: false),
                    Alcance = table.Column<string>(type: "TEXT", nullable: true),
                    Tamanho = table.Column<string>(type: "TEXT", nullable: true),
                    Duracao = table.Column<string>(type: "TEXT", nullable: true),
                    CustoBase = table.Column<int>(type: "INTEGER", nullable: false),
                    PoderBase = table.Column<int>(type: "INTEGER", nullable: false),
                    Basica = table.Column<bool>(type: "INTEGER", nullable: false),
                    Disciplina = table.Column<string>(type: "TEXT", nullable: true),
                    NivelMinimo = table.Column<int>(type: "INTEGER", nullable: false),
                    Imagem = table.Column<string>(type: "TEXT", nullable: true),
                    AceitaTransmutacao = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Formas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Modificadors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", nullable: false),
                    OperacaoCusto = table.Column<string>(type: "TEXT", nullable: false),
                    ValorCusto = table.Column<double>(type: "REAL", nullable: false),
                    OperacaoPoder = table.Column<string>(type: "TEXT", nullable: true),
                    ValorPoder = table.Column<double>(type: "REAL", nullable: true),
                    Efeito = table.Column<string>(type: "TEXT", nullable: true),
                    Basica = table.Column<bool>(type: "INTEGER", nullable: false),
                    Disciplina = table.Column<string>(type: "TEXT", nullable: true),
                    NivelMinimo = table.Column<int>(type: "INTEGER", nullable: false),
                    Imagem = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modificadors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Personagens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    Player = table.Column<string>(type: "TEXT", nullable: false),
                    Nivel = table.Column<int>(type: "INTEGER", nullable: false),
                    Superior = table.Column<int>(type: "INTEGER", nullable: false),
                    EsquerdaSuperior = table.Column<int>(type: "INTEGER", nullable: false),
                    DireitaSuperior = table.Column<int>(type: "INTEGER", nullable: false),
                    EsquerdaInferior = table.Column<int>(type: "INTEGER", nullable: false),
                    DireitaInferior = table.Column<int>(type: "INTEGER", nullable: false),
                    Agilidade = table.Column<int>(type: "INTEGER", nullable: false),
                    Dominio = table.Column<int>(type: "INTEGER", nullable: false),
                    Percepcao = table.Column<int>(type: "INTEGER", nullable: false),
                    Potencia = table.Column<int>(type: "INTEGER", nullable: false),
                    Resistencia = table.Column<int>(type: "INTEGER", nullable: false),
                    VidaAtual = table.Column<int>(type: "INTEGER", nullable: false),
                    ManaAtual = table.Column<int>(type: "INTEGER", nullable: false),
                    EstabilidadeAtual = table.Column<int>(type: "INTEGER", nullable: false),
                    Dinheiro = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personagens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Transmutacaos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", nullable: false),
                    DadoPoder = table.Column<string>(type: "TEXT", nullable: true),
                    CustoMultiplicador = table.Column<double>(type: "REAL", nullable: false),
                    Basica = table.Column<bool>(type: "INTEGER", nullable: false),
                    Disciplina = table.Column<string>(type: "TEXT", nullable: true),
                    NivelMinimo = table.Column<int>(type: "INTEGER", nullable: false),
                    Imagem = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transmutacaos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Encantamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    PersonagemId = table.Column<int>(type: "INTEGER", nullable: true),
                    FormaId = table.Column<int>(type: "INTEGER", nullable: false),
                    CustoFinal = table.Column<int>(type: "INTEGER", nullable: false),
                    PoderFinal = table.Column<string>(type: "TEXT", nullable: false),
                    Alcance = table.Column<string>(type: "TEXT", nullable: true),
                    Tamanho = table.Column<string>(type: "TEXT", nullable: true),
                    Duracao = table.Column<string>(type: "TEXT", nullable: true),
                    Observacao = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Encantamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Encantamentos_Formas_FormaId",
                        column: x => x.FormaId,
                        principalTable: "Formas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Encantamentos_Personagens_PersonagemId",
                        column: x => x.PersonagemId,
                        principalTable: "Personagens",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Itens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PersonagemId = table.Column<int>(type: "INTEGER", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    Quantidade = table.Column<int>(type: "INTEGER", nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", nullable: true),
                    Acoes = table.Column<string>(type: "TEXT", nullable: true),
                    Municao = table.Column<string>(type: "TEXT", nullable: true),
                    Alcance = table.Column<string>(type: "TEXT", nullable: true),
                    Dano = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Itens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Itens_Personagens_PersonagemId",
                        column: x => x.PersonagemId,
                        principalTable: "Personagens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonagemFormas",
                columns: table => new
                {
                    PersonagemId = table.Column<int>(type: "INTEGER", nullable: false),
                    FormaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonagemFormas", x => new { x.PersonagemId, x.FormaId });
                    table.ForeignKey(
                        name: "FK_PersonagemFormas_Formas_FormaId",
                        column: x => x.FormaId,
                        principalTable: "Formas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonagemFormas_Personagens_PersonagemId",
                        column: x => x.PersonagemId,
                        principalTable: "Personagens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonagemModificadors",
                columns: table => new
                {
                    PersonagemId = table.Column<int>(type: "INTEGER", nullable: false),
                    ModificadorId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonagemModificadors", x => new { x.PersonagemId, x.ModificadorId });
                    table.ForeignKey(
                        name: "FK_PersonagemModificadors_Modificadors_ModificadorId",
                        column: x => x.ModificadorId,
                        principalTable: "Modificadors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonagemModificadors_Personagens_PersonagemId",
                        column: x => x.PersonagemId,
                        principalTable: "Personagens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonagemTransmutacaos",
                columns: table => new
                {
                    PersonagemId = table.Column<int>(type: "INTEGER", nullable: false),
                    TransmutacaoId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonagemTransmutacaos", x => new { x.PersonagemId, x.TransmutacaoId });
                    table.ForeignKey(
                        name: "FK_PersonagemTransmutacaos_Personagens_PersonagemId",
                        column: x => x.PersonagemId,
                        principalTable: "Personagens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonagemTransmutacaos_Transmutacaos_TransmutacaoId",
                        column: x => x.TransmutacaoId,
                        principalTable: "Transmutacaos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EncantamentoModificadors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EncantamentoId = table.Column<int>(type: "INTEGER", nullable: false),
                    ModificadorId = table.Column<int>(type: "INTEGER", nullable: false),
                    Ordem = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EncantamentoModificadors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EncantamentoModificadors_Encantamentos_EncantamentoId",
                        column: x => x.EncantamentoId,
                        principalTable: "Encantamentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EncantamentoModificadors_Modificadors_ModificadorId",
                        column: x => x.ModificadorId,
                        principalTable: "Modificadors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EncantamentoTransmutacaos",
                columns: table => new
                {
                    EncantamentoId = table.Column<int>(type: "INTEGER", nullable: false),
                    TransmutacaoId = table.Column<int>(type: "INTEGER", nullable: false),
                    Ordem = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EncantamentoTransmutacaos", x => new { x.EncantamentoId, x.TransmutacaoId });
                    table.ForeignKey(
                        name: "FK_EncantamentoTransmutacaos_Encantamentos_EncantamentoId",
                        column: x => x.EncantamentoId,
                        principalTable: "Encantamentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EncantamentoTransmutacaos_Transmutacaos_TransmutacaoId",
                        column: x => x.TransmutacaoId,
                        principalTable: "Transmutacaos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EncantamentoModificadors_EncantamentoId",
                table: "EncantamentoModificadors",
                column: "EncantamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_EncantamentoModificadors_ModificadorId",
                table: "EncantamentoModificadors",
                column: "ModificadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Encantamentos_FormaId",
                table: "Encantamentos",
                column: "FormaId");

            migrationBuilder.CreateIndex(
                name: "IX_Encantamentos_PersonagemId",
                table: "Encantamentos",
                column: "PersonagemId");

            migrationBuilder.CreateIndex(
                name: "IX_EncantamentoTransmutacaos_TransmutacaoId",
                table: "EncantamentoTransmutacaos",
                column: "TransmutacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Itens_PersonagemId",
                table: "Itens",
                column: "PersonagemId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonagemFormas_FormaId",
                table: "PersonagemFormas",
                column: "FormaId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonagemModificadors_ModificadorId",
                table: "PersonagemModificadors",
                column: "ModificadorId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonagemTransmutacaos_TransmutacaoId",
                table: "PersonagemTransmutacaos",
                column: "TransmutacaoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EncantamentoModificadors");

            migrationBuilder.DropTable(
                name: "EncantamentoTransmutacaos");

            migrationBuilder.DropTable(
                name: "Itens");

            migrationBuilder.DropTable(
                name: "PersonagemFormas");

            migrationBuilder.DropTable(
                name: "PersonagemModificadors");

            migrationBuilder.DropTable(
                name: "PersonagemTransmutacaos");

            migrationBuilder.DropTable(
                name: "Encantamentos");

            migrationBuilder.DropTable(
                name: "Modificadors");

            migrationBuilder.DropTable(
                name: "Transmutacaos");

            migrationBuilder.DropTable(
                name: "Formas");

            migrationBuilder.DropTable(
                name: "Personagens");
        }
    }
}
