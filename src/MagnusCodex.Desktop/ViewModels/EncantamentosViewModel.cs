using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MagnusCodex.Data;
using MagnusCodex.Domain.Entities;
using MagnusCodex.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace MagnusCodex.Desktop.ViewModels;

public partial class EncantamentosViewModel : ViewModelBase
{
    private int _personagemId;

    // ── Lista de encantamentos salvos ────────────────────────────────────────
    public ObservableCollection<EncantamentoViewModel> Encantamentos { get; } = [];

    [ObservableProperty] private bool _carregando;

    // ── Construtor: estado ───────────────────────────────────────────────────
    [ObservableProperty] private bool _construtorAberto;

    // Campos do encantamento sendo criado
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PreviewCusto))]
    [NotifyPropertyChangedFor(nameof(PreviewPoder))]
    [NotifyPropertyChangedFor(nameof(PreviewAcoes))]
    [NotifyPropertyChangedFor(nameof(TransmutacaoBloqueada))]
    [NotifyPropertyChangedFor(nameof(Duracao))]
    [NotifyPropertyChangedFor(nameof(Tamanho))]
    [NotifyPropertyChangedFor(nameof(Alcance))]
    private CartaViewModel? _formaSelecionada;

    // Transmutações selecionadas (máx 2, ordem importa)
    public ObservableCollection<CartaViewModel> TransmutacoesSelecionadas { get; } = [];

    // Modificadores selecionados (ordem importa, repetição permitida)
    public ObservableCollection<CartaViewModel> ModificadoresSelecionados { get; } = [];

    [ObservableProperty] private string _novoNome      = string.Empty;
    [ObservableProperty] private string _novaDescricao = string.Empty;

    // ── Cartas disponíveis do personagem (para o construtor) ─────────────────
    public ObservableCollection<CartaViewModel> FormasDisponiveis        { get; } = [];
    public ObservableCollection<CartaViewModel> TransmutacoesDisponiveis { get; } = [];
    public ObservableCollection<CartaViewModel> ModificadoresDisponiveis { get; } = [];

    // ── Preview em tempo real ────────────────────────────────────────────────

    public bool TransmutacaoBloqueada =>
        FormaSelecionada != null &&
        // Busca nas formas disponíveis se AceitaTransmutacao = false
        !_aceitaTransmutacaoMap.GetValueOrDefault(FormaSelecionada.Id, true);

    // Mapa de Id -> AceitaTransmutacao carregado junto com as formas
    private readonly Dictionary<int, bool> _aceitaTransmutacaoMap = new();

    public int PreviewCusto => CalcularPreviewCusto();
    public string PreviewPoder => CalcularPreviewPoder();
    public int PreviewAcoes => 1 + TransmutacoesSelecionadas.Count + ModificadoresSelecionados.Count;

    public string? Duracao => FormaSelecionada != null
        ? _duracaoMap.GetValueOrDefault(FormaSelecionada.Id) : null;
    public string? Tamanho => FormaSelecionada != null
        ? _tamanhoMap.GetValueOrDefault(FormaSelecionada.Id) : null;
    public string? Alcance => FormaSelecionada != null
        ? _alcanceMap.GetValueOrDefault(FormaSelecionada.Id) : null;

    private readonly Dictionary<int, string?> _duracaoMap  = new();
    private readonly Dictionary<int, string?> _tamanhoMap  = new();
    private readonly Dictionary<int, string?> _alcanceMap  = new();

    // Mapas para cálculo (Id -> entidade leve)
    private readonly Dictionary<int, (double CustoMultiplicador, string? DadoPoder)> _transmutacaoMap = new();
    private readonly Dictionary<int, (string OperacaoCusto, double ValorCusto, string? OperacaoPoder, double? ValorPoder)> _modificadorMap = new();
    private readonly Dictionary<int, (int CustoBase, int PoderBase)> _formaMap = new();

    // ── Carregamento ─────────────────────────────────────────────────────────

    public async Task CarregarAsync(int personagemId)
    {
        _personagemId = personagemId;
        Carregando = true;

        FormasDisponiveis.Clear();
        TransmutacoesDisponiveis.Clear();
        ModificadoresDisponiveis.Clear();
        Encantamentos.Clear();
        _aceitaTransmutacaoMap.Clear();
        _duracaoMap.Clear();
        _tamanhoMap.Clear();
        _alcanceMap.Clear();
        _formaMap.Clear();
        _transmutacaoMap.Clear();
        _modificadorMap.Clear();

        var assetsDir = Path.Combine(AppContext.BaseDirectory, "Assets");
        await using var db = DatabaseInitializer.CriarContexto();

        // Formas do personagem
        var formas = await db.PersonagemFormas
            .Where(pf => pf.PersonagemId == personagemId)
            .Include(pf => pf.Forma)
            .Select(pf => pf.Forma)
            .AsNoTracking().ToListAsync();

        foreach (var f in formas)
        {
            FormasDisponiveis.Add(MapearCarta(f, assetsDir));
            _aceitaTransmutacaoMap[f.Id] = f.AceitaTransmutacao;
            _duracaoMap[f.Id]  = f.Duracao;
            _tamanhoMap[f.Id]  = f.Tamanho;
            _alcanceMap[f.Id]  = f.Alcance;
            _formaMap[f.Id]    = (f.CustoBase, f.PoderBase);
        }

        // Transmutações do personagem
        var transmutacaos = await db.PersonagemTransmutacaos
            .Where(pt => pt.PersonagemId == personagemId)
            .Include(pt => pt.Transmutacao)
            .Select(pt => pt.Transmutacao)
            .AsNoTracking().ToListAsync();

        foreach (var t in transmutacaos)
        {
            TransmutacoesDisponiveis.Add(MapearCarta(t, assetsDir));
            _transmutacaoMap[t.Id] = (t.CustoMultiplicador, t.DadoPoder);
        }

        // Modificadores do personagem
        var modificadors = await db.PersonagemModificadors
            .Where(pm => pm.PersonagemId == personagemId)
            .Include(pm => pm.Modificador)
            .Select(pm => pm.Modificador)
            .AsNoTracking().ToListAsync();

        foreach (var m in modificadors)
        {
            ModificadoresDisponiveis.Add(MapearCarta(m, assetsDir));
            _modificadorMap[m.Id] = (m.OperacaoCusto, m.ValorCusto, m.OperacaoPoder, m.ValorPoder);
        }

        // Encantamentos salvos
        var encantamentos = await db.Encantamentos
            .Where(e => e.PersonagemId == personagemId)
            .Include(e => e.Forma)
            .Include(e => e.EncantamentoTransmutacaos)
                .ThenInclude(et => et.Transmutacao)
            .Include(e => e.EncantamentoModificadors)
                .ThenInclude(em => em.Modificador)
            .AsNoTracking()
            .ToListAsync();

        foreach (var e in encantamentos)
            Encantamentos.Add(MapearEncantamento(e));

        Carregando = false;
    }

    // ── Comandos do construtor ───────────────────────────────────────────────

    [RelayCommand]
    private void AbrirConstrutor()
    {
        FormaSelecionada = null;
        TransmutacoesSelecionadas.Clear();
        ModificadoresSelecionados.Clear();
        NovoNome      = string.Empty;
        NovaDescricao = string.Empty;
        ConstrutorAberto = true;
    }

    [RelayCommand]
    private void FecharConstrutor() => ConstrutorAberto = false;

    [RelayCommand]
    private void SelecionarForma(CartaViewModel forma)
    {
        FormaSelecionada = forma;
        // Se a forma não aceita transmutação, limpa as já selecionadas
        if (TransmutacaoBloqueada)
            TransmutacoesSelecionadas.Clear();
        NotificarPreview();
    }

    [RelayCommand]
    private void AdicionarTransmutacao(CartaViewModel t)
    {
        if (TransmutacaoBloqueada) return;
        if (TransmutacoesSelecionadas.Count >= 2) return;
        if (TransmutacoesSelecionadas.Any(x => x.Id == t.Id)) return;
        TransmutacoesSelecionadas.Add(t);
        NotificarPreview();
    }

    [RelayCommand]
    private void RemoverTransmutacao(CartaViewModel t)
    {
        TransmutacoesSelecionadas.Remove(t);
        NotificarPreview();
    }

    [RelayCommand]
    private void AdicionarModificador(CartaViewModel m)
    {
        ModificadoresSelecionados.Add(m);
        NotificarPreview();
    }

    [RelayCommand]
    private void RemoverModificador(CartaViewModel m)
    {
        // Remove apenas a última ocorrência (permite repetição)
        var idx = ModificadoresSelecionados
            .Select((x, i) => (x, i))
            .LastOrDefault(p => p.x.Id == m.Id).i;
        if (idx >= 0)
            ModificadoresSelecionados.RemoveAt(idx);
        NotificarPreview();
    }

    [RelayCommand]
    private async Task SalvarEncantamentoAsync()
    {
        if (FormaSelecionada == null || string.IsNullOrWhiteSpace(NovoNome)) return;

        var encantamento = new Encantamento
        {
            Nome         = NovoNome.Trim(),
            PersonagemId = _personagemId,
            FormaId      = FormaSelecionada.Id,
            CustoFinal   = PreviewCusto,
            PoderFinal   = PreviewPoder,
            Alcance      = Alcance,
            Tamanho      = Tamanho,
            Duracao      = Duracao,
            Observacao   = NovaDescricao.Trim()
        };

        try
        {
            await using var db = DatabaseInitializer.CriarContexto();
            db.Encantamentos.Add(encantamento);
            await db.SaveChangesAsync();

            // Transmutações com ordem
            for (int i = 0; i < TransmutacoesSelecionadas.Count; i++)
            {
                db.EncantamentoTransmutacaos.Add(new EncantamentoTransmutacao
                {
                    EncantamentoId = encantamento.Id,
                    TransmutacaoId = TransmutacoesSelecionadas[i].Id,
                    Ordem          = i
                });
            }

            // Modificadores com ordem
            for (int i = 0; i < ModificadoresSelecionados.Count; i++)
            {
                db.EncantamentoModificadors.Add(new EncantamentoModificador
                {
                    EncantamentoId = encantamento.Id,
                    ModificadorId  = ModificadoresSelecionados[i].Id,
                    Ordem          = i
                });
            }

            await db.SaveChangesAsync();

            // Recarrega o encantamento completo para exibir na lista
            var salvo = await db.Encantamentos
                .Include(e => e.Forma)
                .Include(e => e.EncantamentoTransmutacaos).ThenInclude(et => et.Transmutacao)
                .Include(e => e.EncantamentoModificadors).ThenInclude(em => em.Modificador)
                .FirstAsync(e => e.Id == encantamento.Id);

            Encantamentos.Add(MapearEncantamento(salvo));
            FecharConstrutor();
            Console.WriteLine($"[Encantamento] '{salvo.Nome}' salvo. Custo {salvo.CustoFinal}, Poder {salvo.PoderFinal}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("[ERRO Encantamento] " + ex.Message);
        }
    }

    [RelayCommand]
    private async Task ExcluirEncantamentoAsync(EncantamentoViewModel enc)
    {
        try
        {
            await using var db = DatabaseInitializer.CriarContexto();
            var existente = await db.Encantamentos.FindAsync(enc.Id);
            if (existente != null)
            {
                db.Encantamentos.Remove(existente);
                await db.SaveChangesAsync();
            }
            Encantamentos.Remove(enc);
        }
        catch (Exception ex)
        {
            Console.WriteLine("[ERRO Excluir Encantamento] " + ex.Message);
        }
    }

    // ── Cálculos de preview ──────────────────────────────────────────────────

    private int CalcularPreviewCusto()
    {
        if (FormaSelecionada == null) return 0;
        if (!_formaMap.TryGetValue(FormaSelecionada.Id, out var forma)) return 0;

        double custo = forma.CustoBase;

        // Multiplicador da transmutação principal (primeira)
        if (TransmutacoesSelecionadas.Count > 0 &&
            _transmutacaoMap.TryGetValue(TransmutacoesSelecionadas[0].Id, out var t))
            custo *= t.CustoMultiplicador;

        var contagem = new Dictionary<int, int>();
        foreach (var mod in ModificadoresSelecionados)
        {
            if (!_modificadorMap.TryGetValue(mod.Id, out var m)) continue;
            contagem.TryGetValue(mod.Id, out int vezes);
            vezes++;
            contagem[mod.Id] = vezes;
            custo = AplicarOperacao(custo, m.OperacaoCusto, m.ValorCusto, vezes);
        }

        return Math.Max(1, (int)Math.Floor(custo));
    }

    private string CalcularPreviewPoder()
    {
        if (FormaSelecionada == null) return "-";
        if (!_formaMap.TryGetValue(FormaSelecionada.Id, out var forma)) return "-";

        double poder = forma.PoderBase;

        foreach (var mod in ModificadoresSelecionados)
        {
            if (!_modificadorMap.TryGetValue(mod.Id, out var m)) continue;
            if (m.OperacaoPoder != null && m.OperacaoPoder != "divisao")
                poder = AplicarOperacao(poder, m.OperacaoPoder, m.ValorPoder ?? 0, 1);
        }

        int poderFinal = Math.Max(1, (int)Math.Floor(poder));

        if (TransmutacoesSelecionadas.Count > 0 &&
            _transmutacaoMap.TryGetValue(TransmutacoesSelecionadas[0].Id, out var trans))
            return $"{poderFinal}{trans.DadoPoder}";

        return "-";
    }

    private static double AplicarOperacao(double valor, string op, double operando, int vezes)
        => op switch
        {
            "soma"             => valor + operando,
            "subtracao"        => valor - operando,
            "multiplicacao"    => valor * operando,
            "divisao"          => operando != 0 ? valor / operando : valor,
            "soma_progressiva" => valor + vezes * operando,
            _                  => valor
        };

    private void NotificarPreview()
    {
        OnPropertyChanged(nameof(PreviewCusto));
        OnPropertyChanged(nameof(PreviewPoder));
        OnPropertyChanged(nameof(PreviewAcoes));
        OnPropertyChanged(nameof(Duracao));
        OnPropertyChanged(nameof(Tamanho));
        OnPropertyChanged(nameof(Alcance));
    }

    // ── Mapeadores ────────────────────────────────────────────────────────────

    private static CartaViewModel MapearCarta(Forma f, string assetsDir) => new()
    {
        Id = f.Id, Nome = f.Nome, Tipo = "Forma", Descricao = f.Descricao,
        Basica = f.Basica, Disciplina = f.Disciplina, NivelMinimo = f.NivelMinimo,
        Imagem = CartaViewModel.CarregarImagem(ResolverImagem(assetsDir, f.Imagem)),
        RodapeInfo = $"Custo {f.CustoBase} | Poder {f.PoderBase}"
    };

    private static CartaViewModel MapearCarta(Transmutacao t, string assetsDir) => new()
    {
        Id = t.Id, Nome = t.Nome, Tipo = "Transmutação", Descricao = t.Descricao,
        Basica = t.Basica, Disciplina = t.Disciplina, NivelMinimo = t.NivelMinimo,
        Imagem = CartaViewModel.CarregarImagem(ResolverImagem(assetsDir, t.Imagem)),
        RodapeInfo = $"×{t.CustoMultiplicador:0.##} | {t.DadoPoder ?? "-"}"
    };

    private static CartaViewModel MapearCarta(Modificador m, string assetsDir) => new()
    {
        Id = m.Id, Nome = m.Nome, Tipo = "Modificador", Descricao = m.Descricao,
        Basica = m.Basica, Disciplina = m.Disciplina, NivelMinimo = m.NivelMinimo,
        Imagem = CartaViewModel.CarregarImagem(ResolverImagem(assetsDir, m.Imagem)),
        RodapeInfo = $"{m.OperacaoCusto} {m.ValorCusto}"
    };

    private static EncantamentoViewModel MapearEncantamento(Encantamento e) => new()
    {
        Id        = e.Id,
        Nome      = e.Nome,
        Descricao = e.Observacao ?? string.Empty,
        Custo     = e.CustoFinal,
        Poder     = e.PoderFinal,
        Acoes     = 1 + e.EncantamentoTransmutacaos.Count + e.EncantamentoModificadors.Count,
        Duracao   = e.Duracao,
        Tamanho   = e.Tamanho,
        Alcance   = e.Alcance,
        FormaNome = e.Forma?.Nome ?? string.Empty,
        TransmutacoesNome = string.Join(", ",
            e.EncantamentoTransmutacaos.OrderBy(et => et.Ordem)
             .Select(et => et.Transmutacao?.Nome ?? "")),
        ModificadoresNome = string.Join(", ",
            e.EncantamentoModificadors.OrderBy(em => em.Ordem)
             .Select(em => em.Modificador?.Nome ?? ""))
    };

    private static string? ResolverImagem(string assetsDir, string? rel)
    {
        if (string.IsNullOrEmpty(rel)) return null;
        var caminho = Path.Combine(assetsDir, rel.Replace('/', Path.DirectorySeparatorChar));
        return File.Exists(caminho) ? caminho : null;
    }
}
