using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MagnusCodex.Data;
using MagnusCodex.Domain.Entities;
using MagnusCodex.Domain.Services;

namespace MagnusCodex.Desktop.ViewModels;

public partial class PersonagemFichaViewModel : ViewModelBase
{
    private Personagem _personagem = new();

    /// <summary>Disparado após salvar — permite que a lista atualize o item.</summary>
    public event Action<Personagem>? PersonagemAtualizado;

    /// <summary>ViewModel das cartas do personagem.</summary>
    public PersonagemCartasViewModel Cartas { get; } = new();

    /// <summary>ViewModel dos encantamentos do personagem.</summary>
    public EncantamentosViewModel Encantamentos { get; } = new();

    /// <summary>Aba ativa: "ficha", "cartas" ou "encantamentos".</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AbaFichaAtiva))]
    [NotifyPropertyChangedFor(nameof(AbaCartasAtiva))]
    [NotifyPropertyChangedFor(nameof(AbaEncantamentosAtiva))]
    private string _abaAtiva = "ficha";

    public bool AbaFichaAtiva          => AbaAtiva == "ficha";
    public bool AbaCartasAtiva         => AbaAtiva == "cartas";
    public bool AbaEncantamentosAtiva  => AbaAtiva == "encantamentos";

    // ── Dados básicos ────────────────────────────────────────────────────────

    [ObservableProperty] private int    _id;
    [ObservableProperty] private string _nome   = string.Empty;
    [ObservableProperty] private string _player = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StatusCalculados))]
    private int _nivel;

    // ── Atributos base ───────────────────────────────────────────────────────

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StatusCalculados))]
    private int _agilidade;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StatusCalculados))]
    private int _dominio;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StatusCalculados))]
    private int _percepcao;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StatusCalculados))]
    private int _potencia;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StatusCalculados))]
    private int _resistencia;

    // ── Disciplinas ──────────────────────────────────────────────────────────

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StatusCalculados))]
    private int _superior;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StatusCalculados))]
    private int _esquerdaSuperior;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StatusCalculados))]
    private int _direitaSuperior;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StatusCalculados))]
    private int _esquerdaInferior;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StatusCalculados))]
    private int _direitaInferior;

    // ── Status atuais ────────────────────────────────────────────────────────

    [ObservableProperty] private int _vidaAtual;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(VidaTotal))]
    [NotifyPropertyChangedFor(nameof(StatusCalculados))]
    private int _vidaAdicional;

    public int VidaTotal => VidaAtual + VidaAdicional;

    partial void OnNivelChanged(int value)       => OnPropertyChanged(nameof(VidaTotal));
    partial void OnResistenciaChanged(int value)  => OnPropertyChanged(nameof(VidaTotal));
    partial void OnVidaAtualChanged(int value)    => OnPropertyChanged(nameof(VidaTotal));

    [ObservableProperty] private int _manaAtual;
    [ObservableProperty] private int _estabilidadeAtual;
    [ObservableProperty] private int _dinheiro;

    // ── Level up ─────────────────────────────────────────────────────────────

    [ObservableProperty] private bool   _levelUpAberto;
    [ObservableProperty] private string _disciplinaEscolhida = "Superior";

    public static IReadOnlyList<string> Disciplinas { get; } =
        ["Superior", "EsquerdaSuperior", "DireitaSuperior",
         "EsquerdaInferior", "DireitaInferior"];

    // ── Status calculados ────────────────────────────────────────────────────

    public StatusCalculadosViewModel StatusCalculados => new(
        Nivel, Agilidade, Dominio, Potencia, Resistencia);

    // ── Inicialização ────────────────────────────────────────────────────────

    public void CarregarPersonagem(Personagem p)
    {
        _personagem       = p;
        Id                = p.Id;
        Nome              = p.Nome;
        Player            = p.Player;
        Nivel             = p.Nivel;
        Agilidade         = p.Agilidade;
        Dominio           = p.Dominio;
        Percepcao         = p.Percepcao;
        Potencia          = p.Potencia;
        Resistencia       = p.Resistencia;
        Superior          = p.Superior;
        EsquerdaSuperior  = p.EsquerdaSuperior;
        DireitaSuperior   = p.DireitaSuperior;
        EsquerdaInferior  = p.EsquerdaInferior;
        DireitaInferior   = p.DireitaInferior;
        VidaAtual         = p.VidaAtual;
        VidaAdicional     = p.VidaAdicional;
        ManaAtual         = p.ManaAtual;
        EstabilidadeAtual = p.EstabilidadeAtual;
        Dinheiro          = p.Dinheiro;
        LevelUpAberto     = false;
        AbaAtiva          = "ficha";

        _ = Cartas.CarregarAsync(p.Id);
        _ = Encantamentos.CarregarAsync(p.Id);
    }

    // ── Comandos ─────────────────────────────────────────────────────────────

    [RelayCommand] private void NavFicha()          => AbaAtiva = "ficha";
    [RelayCommand] private void NavCartas()         => AbaAtiva = "cartas";
    [RelayCommand] private void NavEncantamentos()  => AbaAtiva = "encantamentos";

    [RelayCommand]
    private async Task SalvarAsync()
    {
        SincronizarParaEntidade();
        try
        {
            await using var db = DatabaseInitializer.CriarContexto();
            var existente = await db.Personagens.FindAsync(_personagem.Id);
            if (existente == null) return;

            existente.Nome              = _personagem.Nome;
            existente.Player            = _personagem.Player;
            existente.Nivel             = _personagem.Nivel;
            existente.Agilidade         = _personagem.Agilidade;
            existente.Dominio           = _personagem.Dominio;
            existente.Percepcao         = _personagem.Percepcao;
            existente.Potencia          = _personagem.Potencia;
            existente.Resistencia       = _personagem.Resistencia;
            existente.Superior          = _personagem.Superior;
            existente.EsquerdaSuperior  = _personagem.EsquerdaSuperior;
            existente.DireitaSuperior   = _personagem.DireitaSuperior;
            existente.EsquerdaInferior  = _personagem.EsquerdaInferior;
            existente.DireitaInferior   = _personagem.DireitaInferior;
            existente.VidaAtual         = _personagem.VidaAtual;
            existente.VidaAdicional     = _personagem.VidaAdicional;
            existente.ManaAtual         = _personagem.ManaAtual;
            existente.EstabilidadeAtual = _personagem.EstabilidadeAtual;
            existente.Dinheiro          = _personagem.Dinheiro;

            await db.SaveChangesAsync();
            Console.WriteLine($"[Salvar] '{existente.Nome}' — Nível {existente.Nivel}");
            PersonagemAtualizado?.Invoke(existente);
        }
        catch (Exception ex)
        {
            Console.WriteLine("[ERRO Salvar] " + ex.Message);
        }
    }

    [RelayCommand] private void AbrirLevelUp()  => LevelUpAberto = true;
    [RelayCommand] private void CancelarLevelUp() => LevelUpAberto = false;
    [RelayCommand] private void EscolherDisciplina(string d) => DisciplinaEscolhida = d;

    [RelayCommand]
    private async Task ConfirmarLevelUpAsync()
    {
        SincronizarParaEntidade();
        LevelUp.Aplicar(_personagem, DisciplinaEscolhida);
        CarregarPersonagem(_personagem);
        await SalvarAsync();
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private void SincronizarParaEntidade()
    {
        _personagem.Nome              = Nome;
        _personagem.Player            = Player;
        _personagem.Nivel             = Nivel;
        _personagem.Agilidade         = Agilidade;
        _personagem.Dominio           = Dominio;
        _personagem.Percepcao         = Percepcao;
        _personagem.Potencia          = Potencia;
        _personagem.Resistencia       = Resistencia;
        _personagem.Superior          = Superior;
        _personagem.EsquerdaSuperior  = EsquerdaSuperior;
        _personagem.DireitaSuperior   = DireitaSuperior;
        _personagem.EsquerdaInferior  = EsquerdaInferior;
        _personagem.DireitaInferior   = DireitaInferior;
        _personagem.VidaAtual         = VidaAtual;
        _personagem.VidaAdicional     = VidaAdicional;
        _personagem.ManaAtual         = ManaAtual;
        _personagem.EstabilidadeAtual = EstabilidadeAtual;
        _personagem.Dinheiro          = Dinheiro;
    }
}

/// <summary>Status calculados — sem estado, recalcula a cada acesso.</summary>
public record StatusCalculadosViewModel(
    int Nivel, int Agilidade, int Dominio, int Potencia, int Resistencia)
{
    public int VidaMax         => Nivel * (20 + 2 * Resistencia);
    public int ManaMax         => Nivel * 20;
    public int EstabilidadeMax => (Nivel + Resistencia) / 2;
    public int CA              => 7 + Nivel + Agilidade / 2;
    public int Acoes           => (4 + Agilidade + Nivel) / 2;
    public int Acerto          => Nivel + Dominio;
    public int Movimento       => 8;
    public int DanoExtra       => Potencia + Nivel;
    public int CM              => Nivel * 10;
    public int Truque          => 5 * ((Nivel + 1) / 2);
}
