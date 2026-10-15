namespace MagnusCodex.Domain.Entities;

public class Modificador
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;

    public string OperacaoCusto { get; set; } = string.Empty;
    public double ValorCusto { get; set; }

    /// <summary>Null quando o modificador não altera o poder.</summary>
    public string? OperacaoPoder { get; set; }
    public double? ValorPoder { get; set; }

    /// <summary>Reservado para efeitos textuais futuros.</summary>
    public string? Efeito { get; set; }

    public bool Basica { get; set; }
    public string? Disciplina { get; set; }
    public int NivelMinimo { get; set; }
    public string? Imagem { get; set; }

    // Navegação
    public List<PersonagemModificador> PersonagemModificadors { get; set; } = [];
    public List<EncantamentoModificador> EncantamentoModificadors { get; set; } = [];
}
