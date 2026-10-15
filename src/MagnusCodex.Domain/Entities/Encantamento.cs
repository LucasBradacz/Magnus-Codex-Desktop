namespace MagnusCodex.Domain.Entities;

public class Encantamento
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;

    /// <summary>Null = modo livre, sem personagem associado.</summary>
    public int? PersonagemId { get; set; }

    public int FormaId { get; set; }

    public int CustoFinal { get; set; }

    /// <summary>Resultado formatado, ex: "4d6" ou "-".</summary>
    public string PoderFinal { get; set; } = string.Empty;

    // Campos herdados da Forma, podendo ser sobrescritos
    public string? Alcance { get; set; }
    public string? Tamanho { get; set; }
    public string? Duracao { get; set; }
    public string? Observacao { get; set; }

    // Navegação
    public Personagem? Personagem { get; set; }
    public Forma Forma { get; set; } = null!;
    public List<EncantamentoTransmutacao> EncantamentoTransmutacaos { get; set; } = [];
    public List<EncantamentoModificador> EncantamentoModificadors { get; set; } = [];
}
