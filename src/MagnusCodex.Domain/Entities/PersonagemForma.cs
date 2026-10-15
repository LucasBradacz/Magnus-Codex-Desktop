namespace MagnusCodex.Domain.Entities;

/// <summary>Cartas de Forma que um Personagem possui.</summary>
public class PersonagemForma
{
    public int PersonagemId { get; set; }
    public int FormaId { get; set; }

    public Personagem Personagem { get; set; } = null!;
    public Forma Forma { get; set; } = null!;
}
