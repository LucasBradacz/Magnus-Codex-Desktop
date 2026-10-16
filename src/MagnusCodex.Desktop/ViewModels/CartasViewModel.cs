using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MagnusCodex.Data;
using Microsoft.EntityFrameworkCore;

namespace MagnusCodex.Desktop.ViewModels;

public partial class CartasViewModel : ViewModelBase
{
    // ── Opções de filtro (usadas pela View para gerar os botões) ─────────────

    public static IReadOnlyList<string> OpcoesNivel { get; } =
        ["Todas", "Básicas", "Nível 1", "Nível 3", "Nível 5", "Nível 7", "Nível 9"];

    public static IReadOnlyList<string> OpcoesDisciplina { get; } =
        ["Todas", "Superior", "EsquerdaSuperior", "DireitaSuperior",
         "EsquerdaInferior", "DireitaInferior"];

    public static IReadOnlyList<string> OpcoesTipo { get; } =
        ["Todos", "Forma", "Transmutação", "Modificador"];

    // ── Filtros selecionados ─────────────────────────────────────────────────

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CartasFiltradas))]
    private string _filtroNivel = "Todas";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CartasFiltradas))]
    private string _filtroDisciplina = "Todas";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CartasFiltradas))]
    private string _filtroTipo = "Todos";

    [ObservableProperty]
    private bool _carregando = false;

    // Cache de todas as cartas
    private List<CartaViewModel> _todasCartas = [];

    /// <summary>Coleção filtrada exibida na grade.</summary>
    public IEnumerable<CartaViewModel> CartasFiltradas => AplicarFiltros();

    // ── Comandos de filtro ───────────────────────────────────────────────────

    [RelayCommand]
    private void SetFiltroNivel(string valor) => FiltroNivel = valor;

    [RelayCommand]
    private void SetFiltroDisciplina(string valor) => FiltroDisciplina = valor;

    [RelayCommand]
    private void SetFiltroTipo(string valor) => FiltroTipo = valor;

    // ── Carregamento ─────────────────────────────────────────────────────────

    [RelayCommand]
    public async Task CarregarAsync()
    {
        Carregando = true;
        _todasCartas = await CarregarCartasDoBancoAsync();
        OnPropertyChanged(nameof(CartasFiltradas));
        Carregando = false;
    }

    // ── Lógica de filtro ─────────────────────────────────────────────────────

    private IEnumerable<CartaViewModel> AplicarFiltros()
    {
        var resultado = _todasCartas.AsEnumerable();

        // Tipo
        resultado = FiltroTipo switch
        {
            "Forma"        => resultado.Where(c => c.Tipo == "Forma"),
            "Transmutação" => resultado.Where(c => c.Tipo == "Transmutação"),
            "Modificador"  => resultado.Where(c => c.Tipo == "Modificador"),
            _              => resultado
        };

        // Nível
        resultado = FiltroNivel switch
        {
            "Básicas" => resultado.Where(c => c.Basica),
            "Nível 1" => resultado.Where(c => c.NivelMinimo <= 1),
            "Nível 3" => resultado.Where(c => c.NivelMinimo <= 3),
            "Nível 5" => resultado.Where(c => c.NivelMinimo <= 5),
            "Nível 7" => resultado.Where(c => c.NivelMinimo <= 7),
            "Nível 9" => resultado.Where(c => c.NivelMinimo <= 9),
            _         => resultado
        };

        // Disciplina — ao filtrar por disciplina mostra também as básicas
        resultado = FiltroDisciplina switch
        {
            "Todas" => resultado,
            _       => resultado.Where(c =>
                c.Disciplina == FiltroDisciplina || c.Basica)
        };

        return resultado;
    }

    // ── Carregamento do banco ────────────────────────────────────────────────

    private static async Task<List<CartaViewModel>> CarregarCartasDoBancoAsync()
    {
        var assetsDir = Path.Combine(AppContext.BaseDirectory, "Assets");
        Console.WriteLine($"[Assets] Procurando imagens em: {assetsDir}");
        Console.WriteLine($"[Assets] Pasta existe: {Directory.Exists(assetsDir)}");
        var lista = new List<CartaViewModel>();

        await using var db = DatabaseInitializer.CriarContexto();

        var formas = await db.Formas.AsNoTracking().ToListAsync();
        foreach (var f in formas)
        {
            lista.Add(new CartaViewModel
            {
                Id          = f.Id,
                Nome        = f.Nome,
                Tipo        = "Forma",
                Descricao   = f.Descricao,
                Basica      = f.Basica,
                Disciplina  = f.Disciplina,
                NivelMinimo = f.NivelMinimo,
                Imagem      = CartaViewModel.CarregarImagem(ResolverImagem(assetsDir, f.Imagem)),
                RodapeInfo  = $"Custo {f.CustoBase} | Poder {f.PoderBase}"
            });
        }

        var transmutacaos = await db.Transmutacaos.AsNoTracking().ToListAsync();
        foreach (var t in transmutacaos)
        {
            lista.Add(new CartaViewModel
            {
                Id          = t.Id,
                Nome        = t.Nome,
                Tipo        = "Transmutação",
                Descricao   = t.Descricao,
                Basica      = t.Basica,
                Disciplina  = t.Disciplina,
                NivelMinimo = t.NivelMinimo,
                Imagem      = CartaViewModel.CarregarImagem(ResolverImagem(assetsDir, t.Imagem)),
                RodapeInfo  = $"Custo ×{t.CustoMultiplicador:0.##} | Dado {t.DadoPoder ?? "-"}"
            });
        }

        var modificadors = await db.Modificadors.AsNoTracking().ToListAsync();
        foreach (var m in modificadors)
        {
            lista.Add(new CartaViewModel
            {
                Id          = m.Id,
                Nome        = m.Nome,
                Tipo        = "Modificador",
                Descricao   = m.Descricao,
                Basica      = m.Basica,
                Disciplina  = m.Disciplina,
                NivelMinimo = m.NivelMinimo,
                Imagem      = CartaViewModel.CarregarImagem(ResolverImagem(assetsDir, m.Imagem)),
                RodapeInfo  = FormatarModificador(m.OperacaoCusto, m.ValorCusto)
            });
        }

        return lista;
    }

    private static string? ResolverImagem(string assetsDir, string? caminhoRelativo)
    {
        if (string.IsNullOrEmpty(caminhoRelativo)) return null;
        var caminho = Path.Combine(assetsDir,
            caminhoRelativo.Replace('/', Path.DirectorySeparatorChar));
        return File.Exists(caminho) ? caminho : null;
    }

    private static string FormatarModificador(string operacao, double valor) => operacao switch
    {
        "soma"             => $"+{valor:0.##} custo",
        "subtracao"        => $"-{valor:0.##} custo",
        "multiplicacao"    => $"×{valor:0.##} custo",
        "divisao"          => $"÷{valor:0.##} custo",
        "soma_progressiva" => $"+{valor:0.##}/uso custo",
        _                  => operacao
    };
}
