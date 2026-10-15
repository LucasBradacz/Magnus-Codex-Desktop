namespace MagnusCodex.Domain.Entities;

/// <summary>Cartas de Modificador que um Personagem possui.</summary>
public class PersonagemModificador
{
    public int PersonagemId { get; set; }
    public int ModificadorId { get; set; }

    public Personagem Personagem { get; set; } = null!;
    public Modificador Modificador { get; set; } = null!;
}
