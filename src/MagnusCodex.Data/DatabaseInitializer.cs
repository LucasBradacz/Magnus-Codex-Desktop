using MagnusCodex.Data.Seed;
using Microsoft.EntityFrameworkCore;

namespace MagnusCodex.Data;

/// <summary>
/// Ponto de entrada para inicialização do banco de dados.
/// Cria o diretório, aplica migrations e executa o seed.
/// </summary>
public static class DatabaseInitializer
{
    public record InicializacaoResultado(
        string CaminhoDb,
        IReadOnlyList<string> MigracoesAplicadas,
        bool SeedRodou,
        int TotalCartas
    );

    /// <summary>
    /// Retorna o caminho completo do arquivo .db conforme a convenção do SO:
    /// Windows → %APPDATA%\MagnusCodex\magnus_codex.db
    /// Linux   → ~/.config/MagnusCodex/magnus_codex.db
    /// </summary>
    public static string ResolverCaminhoDb()
    {
        var appData = Environment.GetFolderPath(
            Environment.SpecialFolder.ApplicationData);
        var pasta = Path.Combine(appData, "MagnusCodex");
        Directory.CreateDirectory(pasta);
        return Path.Combine(pasta, "magnus_codex.db");
    }

    /// <summary>
    /// Cria o AppDbContext apontando para o banco de dados do usuário.
    /// </summary>
    public static AppDbContext CriarContexto(string? caminhoDb = null)
    {
        caminhoDb ??= ResolverCaminhoDb();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite($"Data Source={caminhoDb}")
            .Options;

        return new AppDbContext(options);
    }

    /// <summary>
    /// Aplica migrations pendentes, roda o seed e retorna um resumo do que foi feito.
    /// </summary>
    /// <param name="caminhoCsv">
    /// Caminho para magnus_codex_cartas.csv.
    /// Tipicamente: Path.Combine(AppContext.BaseDirectory, "Data", "magnus_codex_cartas.csv")
    /// </param>
    /// <param name="caminhoDb">Opcional — usa o padrão do SO se omitido.</param>
    public static async Task<InicializacaoResultado> InicializarAsync(
        string caminhoCsv, string? caminhoDb = null)
    {
        caminhoDb ??= ResolverCaminhoDb();
        await using var db = CriarContexto(caminhoDb);

        // Captura quais migrations ainda não foram aplicadas
        var pendentes = (await db.Database.GetPendingMigrationsAsync()).ToList();
        await db.Database.MigrateAsync();

        var (seedRodou, totalCartas) = await DbSeeder.SeedAsync(db, caminhoCsv);

        return new InicializacaoResultado(
            CaminhoDb: caminhoDb,
            MigracoesAplicadas: pendentes,
            SeedRodou: seedRodou,
            TotalCartas: totalCartas
        );
    }
}
