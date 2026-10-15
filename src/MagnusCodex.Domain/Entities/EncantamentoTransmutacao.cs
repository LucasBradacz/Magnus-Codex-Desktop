namespace MagnusCodex.Domain.Entities;

/// <summary>
/// Transmutações de um Encantamento com ordem definida.
/// Máximo de 2 por encantamento; a primeira (Ordem = 0) é a transmutação principal
/// que multiplica o custo e define o dado de poder.
/// </summary>
public class EncantamentoTransmutacao
{
    public int EncantamentoId { get; set; }
    public int TransmutacaoId { get; set; }

    /// <summary>Posição na combinação (0 = principal, 1 = secundária).</summary>
    public int Ordem { get; set; }

    public Encantamento Encantamento { get; set; } = null!;
    public Transmutacao Transmutacao { get; set; } = null!;
}
