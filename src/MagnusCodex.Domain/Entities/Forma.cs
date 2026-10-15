namespace MagnusCodex.Domain.Entities;

public class Forma
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string? Alcance { get; set; }
    public string? Tamanho { get; set; }
    public string? Duracao { get; set; }

    public int CustoBase { get; set; }
    public int PoderBase { get; set; }

    public bool Basica { get; set; }
    public string? Disciplina { get; set; }
    public int NivelMinimo { get; set; }

    /// <summary>Caminho relativo a partir da pasta Assets, ex: cartas/formas/esfera.png</summary>
    public string? Imagem { get; set; }

    /// <summary>
    /// Quando false, o encantamento não aceita transmutação (ex: Seletor).
    /// </summary>
    public bool AceitaTransmutacao { get; set; } = true;

    // Navegação
    public List<PersonagemForma> PersonagemFormas { get; set; } = [];
}
