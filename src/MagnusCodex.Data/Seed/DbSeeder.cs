using MagnusCodex.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MagnusCodex.Data.Seed;

/// <summary>
/// Popula o banco na primeira execução.
/// Chame <see cref="SeedAsync"/> logo após aplicar as migrations.
/// </summary>
public static class DbSeeder
{
    /// <summary>
    /// Roda o seed apenas se ainda não houver cartas no banco.
    /// Retorna (seedRodou, totalCartas).
    /// </summary>
    /// <param name="db">Contexto já aberto.</param>
    /// <param name="caminhoCsv">Caminho completo para magnus_codex_cartas.csv.</param>
    public static async Task<(bool SeedRodou, int TotalCartas)> SeedAsync(
        AppDbContext db, string caminhoCsv)
    {
        // Idempotente: não re-importa se já tiver dados
        if (await db.Formas.AnyAsync())
        {
            int total = await db.Formas.CountAsync()
                      + await db.Transmutacaos.CountAsync()
                      + await db.Modificadors.CountAsync();
            return (false, total);
        }

        if (!File.Exists(caminhoCsv))
            throw new FileNotFoundException(
                $"CSV de cartas não encontrado: {caminhoCsv}");

        var cartas = CartasCsvReader.Ler(caminhoCsv);

        // Pendência do documento: Seletor não aceita transmutação
        var seletor = cartas.Formas.FirstOrDefault(f =>
            f.Nome.Equals("Seletor", StringComparison.OrdinalIgnoreCase));
        if (seletor != null)
            seletor.AceitaTransmutacao = false;

        // Pendência do documento: Crítico Recursivo usa soma_progressiva
        var criticoRecursivo = cartas.Modificadors.FirstOrDefault(m =>
            m.Nome.Equals("Crítico Recursivo", StringComparison.OrdinalIgnoreCase));
        if (criticoRecursivo != null)
        {
            criticoRecursivo.OperacaoCusto = "soma_progressiva";
            criticoRecursivo.ValorCusto    = 1;
        }

        db.Formas.AddRange(cartas.Formas);
        db.Transmutacaos.AddRange(cartas.Transmutacaos);
        db.Modificadors.AddRange(cartas.Modificadors);

        await db.SaveChangesAsync();

        int totalImportado = cartas.Formas.Count
                           + cartas.Transmutacaos.Count
                           + cartas.Modificadors.Count;
        return (true, totalImportado);
    }
}
