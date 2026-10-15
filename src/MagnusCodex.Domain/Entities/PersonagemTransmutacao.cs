namespace MagnusCodex.Domain.Entities;

/// <summary>Cartas de Transmutação que um Personagem possui.</summary>
public class PersonagemTransmutacao
{
    public int PersonagemId { get; set; }
    public int TransmutacaoId { get; set; }

    public Personagem Personagem { get; set; } = null!;
    public Transmutacao Transmutacao { get; set; } = null!;
}
