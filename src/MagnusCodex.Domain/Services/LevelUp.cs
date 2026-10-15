using MagnusCodex.Domain.Entities;

namespace MagnusCodex.Domain.Services;

/// <summary>
/// Aplica as regras de level up ao personagem.
/// Não persiste — apenas muta o objeto em memória.
/// </summary>
public static class LevelUp
{
    /// <summary>Nomes das disciplinas na ordem do ciclo de adjacência do pentagrama.</summary>
    private static readonly string[] CicloAdjacencia =
    [
        "Superior",
        "EsquerdaSuperior",
        "EsquerdaInferior",
        "DireitaInferior",
        "DireitaSuperior"
    ];

    /// <summary>
    /// Incrementa o nível e sobe a disciplina escolhida em +1,
    /// propagando +1 para adjacentes conforme a regra de par.
    /// </summary>
    /// <param name="personagem">Personagem a ser modificado.</param>
    /// <param name="disciplinaEscolhida">Nome da disciplina (ex: "Superior").</param>
    public static void Aplicar(Personagem personagem, string disciplinaEscolhida)
    {
        personagem.Nivel++;

        int novoValor = GetDisciplina(personagem, disciplinaEscolhida) + 1;
        SetDisciplina(personagem, disciplinaEscolhida, novoValor);

        // Se o novo valor for par, cada adjacente ganha +1
        // EXCETO se a adjacente já for estritamente maior que o novo valor.
        // Propagação de um passo só — nunca encadeia.
        if (novoValor % 2 == 0)
        {
            foreach (var adjacente in GetAdjacentes(disciplinaEscolhida))
            {
                int valorAdj = GetDisciplina(personagem, adjacente);
                if (valorAdj < novoValor)
                    SetDisciplina(personagem, adjacente, valorAdj + 1);
            }
        }
    }

    // ─── Helpers ────────────────────────────────────────────────────────────

    private static IEnumerable<string> GetAdjacentes(string disciplina)
    {
        int idx = Array.IndexOf(CicloAdjacencia, disciplina);
        if (idx < 0) yield break;

        int len = CicloAdjacencia.Length;
        yield return CicloAdjacencia[(idx - 1 + len) % len];
        yield return CicloAdjacencia[(idx + 1) % len];
    }

    private static int GetDisciplina(Personagem p, string nome) => nome switch
    {
        "Superior"          => p.Superior,
        "EsquerdaSuperior"  => p.EsquerdaSuperior,
        "DireitaSuperior"   => p.DireitaSuperior,
        "EsquerdaInferior"  => p.EsquerdaInferior,
        "DireitaInferior"   => p.DireitaInferior,
        _                   => throw new ArgumentException($"Disciplina desconhecida: {nome}")
    };

    private static void SetDisciplina(Personagem p, string nome, int valor)
    {
        switch (nome)
        {
            case "Superior":          p.Superior = valor;          break;
            case "EsquerdaSuperior":  p.EsquerdaSuperior = valor;  break;
            case "DireitaSuperior":   p.DireitaSuperior = valor;   break;
            case "EsquerdaInferior":  p.EsquerdaInferior = valor;  break;
            case "DireitaInferior":   p.DireitaInferior = valor;   break;
            default: throw new ArgumentException($"Disciplina desconhecida: {nome}");
        }
    }
}
