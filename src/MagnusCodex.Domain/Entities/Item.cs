namespace MagnusCodex.Domain.Entities;

public class Item
{
    public int Id { get; set; }
    public int PersonagemId { get; set; }

    public string Nome { get; set; } = string.Empty;
    public int Quantidade { get; set; } = 1;
    public string? Descricao { get; set; }

    // Campos opcionais — preenchidos apenas quando o item é uma arma
    public string? Acoes { get; set; }
    public string? Municao { get; set; }
    public string? Alcance { get; set; }
    public string? Dano { get; set; }

    /// <summary>True quando Dano não é nulo — item é uma arma.</summary>
    public bool EhArma => Dano != null;

    // Navegação
    public Personagem Personagem { get; set; } = null!;
}
