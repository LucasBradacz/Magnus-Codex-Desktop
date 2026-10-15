namespace MagnusCodex.Domain.Entities;

public class Personagem
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Player { get; set; } = string.Empty;
    public int Nivel { get; set; } = 1;

    // Atributos de disciplina
    public int Superior { get; set; }
    public int EsquerdaSuperior { get; set; }
    public int DireitaSuperior { get; set; }
    public int EsquerdaInferior { get; set; }
    public int DireitaInferior { get; set; }

    // Atributos base
    public int Agilidade { get; set; }
    public int Dominio { get; set; }
    public int Percepcao { get; set; }
    public int Potencia { get; set; }
    public int Resistencia { get; set; }

    // Status atuais (persistidos)
    public int VidaAtual { get; set; }
    public int ManaAtual { get; set; }
    public int EstabilidadeAtual { get; set; }
    public int Dinheiro { get; set; }

    // ─── Propriedades calculadas (não persistidas) ───────────────────────────

    /// <summary>Nivel × (20 + 2 × Resistencia)</summary>
    public int VidaMax => Nivel * (20 + 2 * Resistencia);

    /// <summary>7 + Nivel + Agilidade / 2  (divisão inteira)</summary>
    public int CA => 7 + Nivel + Agilidade / 2;

    /// <summary>Nivel × 20</summary>
    public int ManaMax => Nivel * 20;

    /// <summary>(Nivel + Resistencia) / 2  (divisão inteira)</summary>
    public int EstabilidadeMax => (Nivel + Resistencia) / 2;

    /// <summary>(4 + Agilidade + Nivel) / 2  (divisão inteira)</summary>
    public int Acoes => (4 + Agilidade + Nivel) / 2;

    /// <summary>Nivel + Dominio</summary>
    public int Acerto => Nivel + Dominio;

    /// <summary>Fixo em 8.</summary>
    public int Movimento => 8;

    /// <summary>Potencia + Nivel</summary>
    public int DanoExtra => Potencia + Nivel;

    /// <summary>Nivel × 10</summary>
    public int CM => Nivel * 10;

    /// <summary>5 × ((Nivel + 1) / 2)  (divisão inteira)</summary>
    public int Truque => 5 * ((Nivel + 1) / 2);

    // Navegação
    public List<PersonagemForma> PersonagemFormas { get; set; } = [];
    public List<PersonagemTransmutacao> PersonagemTransmutacaos { get; set; } = [];
    public List<PersonagemModificador> PersonagemModificadors { get; set; } = [];
    public List<Encantamento> Encantamentos { get; set; } = [];
    public List<Item> Itens { get; set; } = [];
}
