using Avalonia.Media.Imaging;

namespace MagnusCodex.Desktop.ViewModels;

/// <summary>
/// Representa uma carta (Forma, Transmutação ou Modificador) na grade de visualização.
/// </summary>
public class CartaViewModel
{
    public int Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Tipo { get; init; } = string.Empty;
    public string Descricao { get; init; } = string.Empty;
    public bool Basica { get; init; }
    public string? Disciplina { get; init; }
    public int NivelMinimo { get; init; }
    public string RodapeInfo { get; init; } = string.Empty;

    /// <summary>Bitmap carregado do disco, ou null se a imagem não foi encontrada.</summary>
    public Bitmap? Imagem { get; init; }

    /// <summary>Carrega o bitmap a partir de um caminho absoluto, retornando null se falhar.</summary>
    public static Bitmap? CarregarImagem(string? caminho)
    {
        if (string.IsNullOrEmpty(caminho) || !File.Exists(caminho))
            return null;
        try { return new Bitmap(caminho); }
        catch { return null; }
    }
}
