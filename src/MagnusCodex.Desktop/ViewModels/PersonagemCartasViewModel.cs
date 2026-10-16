using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MagnusCodex.Data;
using MagnusCodex.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MagnusCodex.Desktop.ViewModels;

public partial class PersonagemCartasViewModel : ViewModelBase
{
    private int _personagemId;

    // ── Cartas do personagem (separadas por tipo) ────────────────────────────

    public ObservableCollection<CartaViewModel> Formas        { get; } = [];
    public ObservableCollection<CartaViewModel> Transmutacaos { get; } = [];
    public ObservableCollection<CartaViewModel> Modificadors  { get; } = [];

    // ── Painel de adição ─────────────────────────────────────────────────────

    [ObservableProperty] private bool   _painelAdicaoAberto;
    [ObservableProperty] private string _filtroAdicaoTipo       = "Todos";
    [ObservableProperty] private string _filtroAdicaoTexto      = string.Empty;
    [ObservableProperty] private bool   _carregando;

    public static IReadOnlyList<string> OpcoesTipo { get; } =
        ["Todos", "Forma", "Transmutação", "Modificador"];

    // Cache de todas as cartas do banco para o painel de adição
    private List<CartaViewModel> _todasCartas = [];

    /// <summary>Cartas filtradas para exibição no painel de adição.</summary>
    public IEnumerable<CartaViewModel> CartasParaAdicionar => FiltrarParaAdicao();

    // ── Carregamento ─────────────────────────────────────────────────────────

    public async Task CarregarAsync(int personagemId)
    {
        _personagemId = personagemId;
        Carregando = true;

        Formas.Clear();
        Transmutacaos.Clear();
        Modificadors.Clear();

        var assetsDir = Path.Combine(AppContext.BaseDirectory, "Assets");

        await using var db = DatabaseInitializer.CriarContexto();

        // Cartas que o personagem já possui
        var formas = await db.PersonagemFormas
            .Where(pf => pf.PersonagemId == personagemId)
            .Include(pf => pf.Forma)
            .Select(pf => pf.Forma)
            .AsNoTracking()
            .ToListAsync();

        foreach (var f in formas)
            Formas.Add(MapearForma(f, assetsDir));

        var transmutacaos = await db.PersonagemTransmutacaos
            .Where(pt => pt.PersonagemId == personagemId)
            .Include(pt => pt.Transmutacao)
            .Select(pt => pt.Transmutacao)
            .AsNoTracking()
            .ToListAsync();

        foreach (var t in transmutacaos)
            Transmutacaos.Add(MapearTransmutacao(t, assetsDir));

        var modificadors = await db.PersonagemModificadors
            .Where(pm => pm.PersonagemId == personagemId)
            .Include(pm => pm.Modificador)
            .Select(pm => pm.Modificador)
            .AsNoTracking()
            .ToListAsync();

        foreach (var m in modificadors)
            Modificadors.Add(MapearModificador(m, assetsDir));

        // Carrega o catálogo completo para o painel de adição
        _todasCartas = await CarregarCatalogoCacheAsync(db, assetsDir);

        Carregando = false;
    }

    // ── Comandos ─────────────────────────────────────────────────────────────

    [RelayCommand]
    private void AbrirPainelAdicao()
    {
        FiltroAdicaoTexto = string.Empty;
        FiltroAdicaoTipo  = "Todos";
        PainelAdicaoAberto = true;
    }

    [RelayCommand]
    private void FecharPainelAdicao() => PainelAdicaoAberto = false;

    [RelayCommand]
    private void SetFiltroTipo(string tipo)
    {
        FiltroAdicaoTipo = tipo;
        OnPropertyChanged(nameof(CartasParaAdicionar));
    }

    partial void OnFiltroAdicaoTextoChanged(string value)
        => OnPropertyChanged(nameof(CartasParaAdicionar));

    [RelayCommand]
    private async Task AdicionarCartaAsync(CartaViewModel carta)
    {
        await using var db = DatabaseInitializer.CriarContexto();

        switch (carta.Tipo)
        {
            case "Forma":
                if (Formas.Any(f => f.Id == carta.Id)) return;
                db.PersonagemFormas.Add(new PersonagemForma
                    { PersonagemId = _personagemId, FormaId = carta.Id });
                await db.SaveChangesAsync();
                Formas.Add(carta);
                break;

            case "Transmutação":
                if (Transmutacaos.Any(t => t.Id == carta.Id)) return;
                db.PersonagemTransmutacaos.Add(new PersonagemTransmutacao
                    { PersonagemId = _personagemId, TransmutacaoId = carta.Id });
                await db.SaveChangesAsync();
                Transmutacaos.Add(carta);
                break;

            case "Modificador":
                if (Modificadors.Any(m => m.Id == carta.Id)) return;
                db.PersonagemModificadors.Add(new PersonagemModificador
                    { PersonagemId = _personagemId, ModificadorId = carta.Id });
                await db.SaveChangesAsync();
                Modificadors.Add(carta);
                break;
        }
    }

    [RelayCommand]
    private async Task RemoverCartaAsync(CartaViewModel carta)
    {
        await using var db = DatabaseInitializer.CriarContexto();

        switch (carta.Tipo)
        {
            case "Forma":
                var pf = await db.PersonagemFormas.FindAsync(_personagemId, carta.Id);
                if (pf != null) db.PersonagemFormas.Remove(pf);
                await db.SaveChangesAsync();
                Formas.Remove(carta);
                break;

            case "Transmutação":
                var pt = await db.PersonagemTransmutacaos.FindAsync(_personagemId, carta.Id);
                if (pt != null) db.PersonagemTransmutacaos.Remove(pt);
                await db.SaveChangesAsync();
                Transmutacaos.Remove(carta);
                break;

            case "Modificador":
                var pm = await db.PersonagemModificadors.FindAsync(_personagemId, carta.Id);
                if (pm != null) db.PersonagemModificadors.Remove(pm);
                await db.SaveChangesAsync();
                Modificadors.Remove(carta);
                break;
        }
    }

    // ── Filtro do painel de adição ────────────────────────────────────────────

    private IEnumerable<CartaViewModel> FiltrarParaAdicao()
    {
        var resultado = _todasCartas.AsEnumerable();

        if (FiltroAdicaoTipo != "Todos")
            resultado = resultado.Where(c => c.Tipo == FiltroAdicaoTipo);

        if (!string.IsNullOrWhiteSpace(FiltroAdicaoTexto))
            resultado = resultado.Where(c =>
                c.Nome.Contains(FiltroAdicaoTexto, StringComparison.OrdinalIgnoreCase));

        return resultado;
    }

    // ── Mapeadores ────────────────────────────────────────────────────────────

    private static CartaViewModel MapearForma(Forma f, string assetsDir) => new()
    {
        Id            = f.Id,
        Nome          = f.Nome,
        Tipo          = "Forma",
        Descricao     = f.Descricao,
        Basica        = f.Basica,
        Disciplina    = f.Disciplina,
        NivelMinimo   = f.NivelMinimo,
        Imagem        = CartaViewModel.CarregarImagem(ResolverImagem(assetsDir, f.Imagem)),
        RodapeInfo    = $"Custo {f.CustoBase} | Poder {f.PoderBase}"
    };

    private static CartaViewModel MapearTransmutacao(Transmutacao t, string assetsDir) => new()
    {
        Id            = t.Id,
        Nome          = t.Nome,
        Tipo          = "Transmutação",
        Descricao     = t.Descricao,
        Basica        = t.Basica,
        Disciplina    = t.Disciplina,
        NivelMinimo   = t.NivelMinimo,
        Imagem        = CartaViewModel.CarregarImagem(ResolverImagem(assetsDir, t.Imagem)),
        RodapeInfo    = $"Custo ×{t.CustoMultiplicador:0.##} | Dado {t.DadoPoder ?? "-"}"
    };

    private static CartaViewModel MapearModificador(Modificador m, string assetsDir) => new()
    {
        Id            = m.Id,
        Nome          = m.Nome,
        Tipo          = "Modificador",
        Descricao     = m.Descricao,
        Basica        = m.Basica,
        Disciplina    = m.Disciplina,
        NivelMinimo   = m.NivelMinimo,
        Imagem        = CartaViewModel.CarregarImagem(ResolverImagem(assetsDir, m.Imagem)),
        RodapeInfo    = FormatarModificador(m.OperacaoCusto, m.ValorCusto)
    };

    private static string? ResolverImagem(string assetsDir, string? rel)
    {
        if (string.IsNullOrEmpty(rel)) return null;
        var caminho = Path.Combine(assetsDir, rel.Replace('/', Path.DirectorySeparatorChar));
        return File.Exists(caminho) ? caminho : null;
    }

    private static string FormatarModificador(string op, double val) => op switch
    {
        "soma"             => $"+{val:0.##} custo",
        "subtracao"        => $"-{val:0.##} custo",
        "multiplicacao"    => $"×{val:0.##} custo",
        "divisao"          => $"÷{val:0.##} custo",
        "soma_progressiva" => $"+{val:0.##}/uso custo",
        _                  => op
    };

    private static async Task<List<CartaViewModel>> CarregarCatalogoCacheAsync(
        AppDbContext db, string assetsDir)
    {
        var lista = new List<CartaViewModel>();

        foreach (var f in await db.Formas.AsNoTracking().ToListAsync())
            lista.Add(MapearForma(f, assetsDir));

        foreach (var t in await db.Transmutacaos.AsNoTracking().ToListAsync())
            lista.Add(MapearTransmutacao(t, assetsDir));

        foreach (var m in await db.Modificadors.AsNoTracking().ToListAsync())
            lista.Add(MapearModificador(m, assetsDir));

        return lista;
    }
}
