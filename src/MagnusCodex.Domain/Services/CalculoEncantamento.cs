using MagnusCodex.Domain.Entities;

namespace MagnusCodex.Domain.Services;

/// <summary>
/// Calcula custo e poder de um encantamento a partir de suas peças.
/// Recebe os dados já carregados — sem acesso a banco aqui.
/// </summary>
public static class CalculoEncantamento
{
    /// <summary>
    /// Retorna o custo final do encantamento (mínimo 1).
    /// </summary>
    /// <param name="forma">A forma base.</param>
    /// <param name="transmutacaoPrincipal">Primeira transmutação (ou null).</param>
    /// <param name="modificadoresOrdenados">Modificadores na ordem de aplicação, com repetição permitida.</param>
    public static int CalcularCusto(
        Forma forma,
        Transmutacao? transmutacaoPrincipal,
        IEnumerable<Modificador> modificadoresOrdenados)
    {
        double custo = forma.CustoBase * (transmutacaoPrincipal?.CustoMultiplicador ?? 1.0);

        // Rastreia quantas vezes cada modificador já apareceu (para soma_progressiva)
        var contagem = new Dictionary<int, int>();

        foreach (var mod in modificadoresOrdenados)
        {
            contagem.TryGetValue(mod.Id, out int vezes);
            vezes++;
            contagem[mod.Id] = vezes;

            custo = AplicarOperacao(custo, mod.OperacaoCusto, mod.ValorCusto, vezes);
        }

        return Math.Max(1, (int)Math.Floor(custo));
    }

    /// <summary>
    /// Retorna o poder formatado do encantamento (ex: "4d6" ou "-").
    /// </summary>
    public static string CalcularPoderFormatado(
        Forma forma,
        Transmutacao? transmutacaoPrincipal,
        IEnumerable<Modificador> modificadoresOrdenados)
    {
        double poder = forma.PoderBase;

        foreach (var mod in modificadoresOrdenados)
        {
            // Divisão de poder é aplicada após a rolagem — ignorar aqui
            if (mod.OperacaoPoder != null && mod.OperacaoPoder != "divisao")
                poder = AplicarOperacao(poder, mod.OperacaoPoder, mod.ValorPoder ?? 0, 1);
        }

        int poderFinal = Math.Max(1, (int)Math.Floor(poder));

        return transmutacaoPrincipal != null
            ? $"{poderFinal}{transmutacaoPrincipal.DadoPoder}"
            : "-";
    }

    /// <summary>
    /// Número de ações necessárias para executar o encantamento.
    /// 1 base + 1 por transmutação + 1 por modificador.
    /// </summary>
    public static int CalcularAcoes(
        IEnumerable<Transmutacao> transmutacoes,
        IEnumerable<Modificador> modificadores)
        => 1 + transmutacoes.Count() + modificadores.Count();

    // ─── Operações internas ──────────────────────────────────────────────────

    private static double AplicarOperacao(double valor, string operacao, double operando, int vezesApareceu)
        => operacao switch
        {
            "soma"               => valor + operando,
            "subtracao"          => valor - operando,
            "multiplicacao"      => valor * operando,
            "divisao"            => operando != 0 ? valor / operando : valor,
            // n-ésima cópia soma n × operando
            "soma_progressiva"   => valor + vezesApareceu * operando,
            _                    => valor
        };
}
