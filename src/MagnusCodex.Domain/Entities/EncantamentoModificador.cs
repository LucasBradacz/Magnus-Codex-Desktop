namespace MagnusCodex.Domain.Entities;

/// <summary>
/// Modificadores de um Encantamento com ordem definida.
/// Repetição é permitida — o mesmo Modificador pode aparecer mais de uma vez.
/// </summary>
public class EncantamentoModificador
{
    public int Id { get; set; }
    public int EncantamentoId { get; set; }
    public int ModificadorId { get; set; }

    /// <summary>Posição na sequência de aplicação (começa em 0).</summary>
    public int Ordem { get; set; }

    public Encantamento Encantamento { get; set; } = null!;
    public Modificador Modificador { get; set; } = null!;
}
