using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MagnusCodex.Data;
using MagnusCodex.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MagnusCodex.Desktop.ViewModels;

public partial class PersonagemListaViewModel : ViewModelBase
{
    public ObservableCollection<Personagem> Personagens { get; } = [];

    [ObservableProperty]
    private bool _carregando;

    // ── Novo personagem (formulário inline) ──────────────────────────────────

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CriarPersonagemCommand))]
    private bool _criandoNovo;

    [ObservableProperty]
    private string _novoNome = string.Empty;

    [ObservableProperty]
    private string _novoPlayer = string.Empty;

    /// <summary>Invocado pela MainViewModel quando o usuário seleciona um personagem.</summary>
    public event Action<Personagem>? PersonagemSelecionado;

    // ── Comandos ─────────────────────────────────────────────────────────────

    [RelayCommand]
    public async Task CarregarAsync()
    {
        Carregando = true;
        Personagens.Clear();

        await using var db = DatabaseInitializer.CriarContexto();
        var lista = await db.Personagens.AsNoTracking()
            .OrderBy(p => p.Nome)
            .ToListAsync();

        foreach (var p in lista)
            Personagens.Add(p);

        Carregando = false;
    }

    [RelayCommand]
    private void IniciarCriacao()
    {
        NovoNome   = string.Empty;
        NovoPlayer = string.Empty;
        CriandoNovo = true;
    }

    [RelayCommand]
    private void CancelarCriacao() => CriandoNovo = false;

    [RelayCommand]
    private async Task CriarPersonagemAsync()
    {
        if (string.IsNullOrWhiteSpace(NovoNome)) return;

        var personagem = new Personagem
        {
            Nome   = NovoNome.Trim(),
            Player = NovoPlayer.Trim(),
            Nivel  = 1
        };

        await using var db = DatabaseInitializer.CriarContexto();
        db.Personagens.Add(personagem);
        await db.SaveChangesAsync();

        Personagens.Add(personagem);
        CriandoNovo = false;

        // Abre direto a ficha do personagem recém-criado
        PersonagemSelecionado?.Invoke(personagem);
    }

    [RelayCommand]
    private void Selecionar(Personagem personagem)
        => PersonagemSelecionado?.Invoke(personagem);

    /// <summary>
    /// Atualiza o item na lista após salvar na ficha,
    /// para o nível e outros campos aparecerem atualizados.
    /// </summary>
    public void AtualizarPersonagem(Personagem atualizado)
    {
        var existente = Personagens.FirstOrDefault(p => p.Id == atualizado.Id);
        if (existente == null) return;

        var idx = Personagens.IndexOf(existente);
        Personagens[idx] = atualizado;
    }

    [RelayCommand]
    private async Task ExcluirAsync(Personagem personagem)
    {
        await using var db = DatabaseInitializer.CriarContexto();
        db.Personagens.Remove(personagem);
        await db.SaveChangesAsync();
        Personagens.Remove(personagem);
    }
}
