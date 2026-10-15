using MagnusCodex.Domain.Entities;

namespace MagnusCodex.Data.Seed;

/// <summary>
/// Lê o arquivo magnus_codex_cartas.csv e devolve listas prontas para seed.
/// Colunas esperadas (19, nesta ordem):
/// tipo, nome, descricao, alcance, tamanho, duracao, custo_base, poder_base,
/// dado_poder, custo_multiplicador, operacao_custo, valor_custo, operacao_poder,
/// valor_poder, efeito, basica, disciplina, nivel_minimo, imagem
/// </summary>
public static class CartasCsvReader
{
    public record CartasLidas(
        List<Forma> Formas,
        List<Transmutacao> Transmutacaos,
        List<Modificador> Modificadors);

    public static CartasLidas Ler(string caminhoArquivo)
    {
        var formas       = new List<Forma>();
        var transmutacaos = new List<Transmutacao>();
        var modificadors = new List<Modificador>();

        var linhas = File.ReadAllLines(caminhoArquivo);

        // Pular cabeçalho (primeira linha)
        foreach (var linha in linhas.Skip(1))
        {
            if (string.IsNullOrWhiteSpace(linha)) continue;

            var cols = SplitCsv(linha);
            if (cols.Length < 19) continue;

            string tipo           = cols[0].Trim().ToLowerInvariant();
            string nome           = cols[1].Trim();
            string descricao      = cols[2].Trim();
            string? alcance       = NullIfEmpty(cols[3]);
            string? tamanho       = NullIfEmpty(cols[4]);
            string? duracao       = NullIfEmpty(cols[5]);
            int    custoBase      = ParseInt(cols[6]);
            int    poderBase      = ParseInt(cols[7]);
            string? dadoPoder     = NullIfEmpty(cols[8]);
            double custoMult      = ParseDouble(cols[9], 1.0);
            string opCusto        = cols[10].Trim();
            double valCusto       = ParseDouble(cols[11]);
            string? opPoder       = NullIfEmpty(cols[12]);
            double? valPoder      = ParseDoubleNullable(cols[13]);
            string? efeito        = NullIfEmpty(cols[14]);
            bool   basica         = ParseBool(cols[15]);
            string? disciplina    = NullIfEmpty(cols[16]);
            int    nivelMinimo    = ParseInt(cols[17]);
            // Corrige caminho: "app/assets/images/cartas/..." → "cartas/..."
            string? imagem        = CorrigirCaminhoImagem(cols[18]);

            switch (tipo)
            {
                case "forma":
                    formas.Add(new Forma
                    {
                        Nome              = nome,
                        Descricao         = descricao,
                        Alcance           = alcance,
                        Tamanho           = tamanho,
                        Duracao           = duracao,
                        CustoBase         = custoBase,
                        PoderBase         = poderBase,
                        Basica            = basica,
                        Disciplina        = disciplina,
                        NivelMinimo       = nivelMinimo,
                        Imagem            = imagem,
                        AceitaTransmutacao = true  // Seletor será corrigido por nome no seed
                    });
                    break;

                case "transmutacao":
                    transmutacaos.Add(new Transmutacao
                    {
                        Nome               = nome,
                        Descricao          = descricao,
                        DadoPoder          = dadoPoder,
                        CustoMultiplicador = custoMult,
                        Basica             = basica,
                        Disciplina         = disciplina,
                        NivelMinimo        = nivelMinimo,
                        Imagem             = imagem
                    });
                    break;

                case "modificador":
                    modificadors.Add(new Modificador
                    {
                        Nome          = nome,
                        Descricao     = descricao,
                        OperacaoCusto = opCusto,
                        ValorCusto    = valCusto,
                        OperacaoPoder = opPoder,
                        ValorPoder    = valPoder,
                        Efeito        = efeito,
                        Basica        = basica,
                        Disciplina    = disciplina,
                        NivelMinimo   = nivelMinimo,
                        Imagem        = imagem
                    });
                    break;
            }
        }

        return new CartasLidas(formas, transmutacaos, modificadors);
    }

    // ─── Helpers ─────────────────────────────────────────────────────────────

    private static string? CorrigirCaminhoImagem(string raw)
    {
        var valor = raw.Trim().Trim('"');
        if (string.IsNullOrEmpty(valor)) return null;
        // Remove prefixo legado do Rails
        return valor.Replace("app/assets/images/", "").Replace('\\', '/');
    }

    private static string? NullIfEmpty(string s)
    {
        var v = s.Trim().Trim('"');
        return string.IsNullOrEmpty(v) ? null : v;
    }

    private static int ParseInt(string s)
        => int.TryParse(s.Trim(), out var v) ? v : 0;

    private static double ParseDouble(string s, double fallback = 0.0)
        => double.TryParse(s.Trim(),
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture,
            out var v) ? v : fallback;

    private static double? ParseDoubleNullable(string s)
    {
        var t = s.Trim().Trim('"');
        if (string.IsNullOrEmpty(t)) return null;
        return double.TryParse(t,
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture,
            out var v) ? v : null;
    }

    private static bool ParseBool(string s)
    {
        var t = s.Trim().Trim('"').ToLowerInvariant();
        return t is "true" or "1" or "yes" or "t";
    }

    /// <summary>
    /// Divisão simples por vírgula, respeitando campos entre aspas duplas.
    /// </summary>
    private static string[] SplitCsv(string linha)
    {
        var campos = new List<string>();
        bool dentroAspas = false;
        var atual = new System.Text.StringBuilder();

        foreach (char c in linha)
        {
            if (c == '"')
            {
                dentroAspas = !dentroAspas;
            }
            else if (c == ',' && !dentroAspas)
            {
                campos.Add(atual.ToString());
                atual.Clear();
            }
            else
            {
                atual.Append(c);
            }
        }
        campos.Add(atual.ToString());
        return [.. campos];
    }
}
