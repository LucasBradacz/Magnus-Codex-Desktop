namespace MagnusCodex.Domain.Entities;

public class Transmutacao
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;

    /// <summary>Dado de poder aplicado ao resultado, ex: "d6".</summary>
    public string? DadoPoder { get; set; }

    /// <summary>Multiplica o custo base da Forma.</summary>
    public double CustoMultiplicador { get; set; } = 1.0;

    public bool Basica { get; set; }
    public string? Disciplina { get; set; }
    public int NivelMinimo { get; set; }
    public string? Imagem { get; set; }

    // Navegação
    public List<PersonagemTransmutacao> PersonagemTransmutacaos { get; set; } = [];
    public List<EncantamentoTransmutacao> EncantamentoTransmutacaos { get; set; } = [];
}
