namespace MagnusCodex.Desktop.ViewModels;

/// <summary>
/// Representa um encantamento salvo, para exibição na lista da ficha.
/// </summary>
public class EncantamentoViewModel
{
    public int    Id        { get; init; }
    public string Nome      { get; init; } = string.Empty;
    public string Descricao { get; init; } = string.Empty;

    // Resultados calculados
    public int    Custo     { get; init; }
    public string Poder     { get; init; } = "-";   // ex: "4d6" ou "-"
    public int    Acoes     { get; init; }

    // Da Forma base
    public string? Duracao  { get; init; }
    public string? Tamanho  { get; init; }
    public string? Alcance  { get; init; }

    // Resumo das peças (para tooltip/detalhe futuro)
    public string FormaNome         { get; init; } = string.Empty;
    public string TransmutacoesNome { get; init; } = string.Empty;
    public string ModificadoresNome { get; init; } = string.Empty;
}
