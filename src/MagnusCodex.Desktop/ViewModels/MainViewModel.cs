using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MagnusCodex.Domain.Entities;

namespace MagnusCodex.Desktop.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    // ── Sub-ViewModels ───────────────────────────────────────────────────────

    public PersonagemListaViewModel Lista { get; } = new();
    public PersonagemFichaViewModel Ficha { get; } = new();
    public CartasViewModel Cartas { get; } = new();

    // ── Navegação ────────────────────────────────────────────────────────────

    /// <summary>
    /// Aba ativa: "personagens" ou "cartas".
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AbaPersonagensAtiva))]
    [NotifyPropertyChangedFor(nameof(AbaCartasAtiva))]
    [NotifyPropertyChangedFor(nameof(MostrarPlaceholderPersonagem))]
    private string _abaAtiva = "personagens";

    public bool AbaPersonagensAtiva => AbaAtiva == "personagens";
    public bool AbaCartasAtiva      => AbaAtiva == "cartas";

    /// <summary>True quando há um personagem aberto na ficha.</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MostrarPlaceholderPersonagem))]
    private bool _fichaAberta;

    /// <summary>Mostra o placeholder quando está na aba de personagens sem ficha aberta.</summary>
    public bool MostrarPlaceholderPersonagem => AbaPersonagensAtiva && !FichaAberta;

    // ── Inicialização ────────────────────────────────────────────────────────

    public MainViewModel()
    {
        Lista.PersonagemSelecionado += AbrirFicha;
        // Quando a ficha salva, atualiza o item correspondente na lista
        Ficha.PersonagemAtualizado  += Lista.AtualizarPersonagem;
    }

    public async Task InicializarAsync()
    {
        await Lista.CarregarAsync();
        await Cartas.CarregarAsync();
    }

    // ── Comandos ─────────────────────────────────────────────────────────────

    [RelayCommand]
    private void NavPersonagens() => AbaAtiva = "personagens";

    [RelayCommand]
    private void NavCartas() => AbaAtiva = "cartas";

    [RelayCommand]
    private void FecharFicha() => FichaAberta = false;

    // ── Helpers ──────────────────────────────────────────────────────────────

    private void AbrirFicha(Personagem personagem)
    {
        Ficha.CarregarPersonagem(personagem);
        FichaAberta = true;
        AbaAtiva = "personagens";
    }
}
