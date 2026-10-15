using MagnusCodex.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MagnusCodex.Data;

public class AppDbContext : DbContext
{
    public DbSet<Forma> Formas => Set<Forma>();
    public DbSet<Transmutacao> Transmutacaos => Set<Transmutacao>();
    public DbSet<Modificador> Modificadors => Set<Modificador>();
    public DbSet<Personagem> Personagens => Set<Personagem>();
    public DbSet<Encantamento> Encantamentos => Set<Encantamento>();
    public DbSet<Item> Itens => Set<Item>();

    // Tabelas de junção
    public DbSet<PersonagemForma> PersonagemFormas => Set<PersonagemForma>();
    public DbSet<PersonagemTransmutacao> PersonagemTransmutacaos => Set<PersonagemTransmutacao>();
    public DbSet<PersonagemModificador> PersonagemModificadors => Set<PersonagemModificador>();
    public DbSet<EncantamentoTransmutacao> EncantamentoTransmutacaos => Set<EncantamentoTransmutacao>();
    public DbSet<EncantamentoModificador> EncantamentoModificadors => Set<EncantamentoModificador>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ── Chaves compostas das tabelas de junção ──────────────────────────

        modelBuilder.Entity<PersonagemForma>()
            .HasKey(pf => new { pf.PersonagemId, pf.FormaId });

        modelBuilder.Entity<PersonagemTransmutacao>()
            .HasKey(pt => new { pt.PersonagemId, pt.TransmutacaoId });

        modelBuilder.Entity<PersonagemModificador>()
            .HasKey(pm => new { pm.PersonagemId, pm.ModificadorId });

        // EncantamentoTransmutacao: chave composta + ordem
        modelBuilder.Entity<EncantamentoTransmutacao>()
            .HasKey(et => new { et.EncantamentoId, et.TransmutacaoId });

        // EncantamentoModificador: tem Id próprio (permite repetição do mesmo modificador)
        modelBuilder.Entity<EncantamentoModificador>()
            .HasKey(em => em.Id);

        // ── Propriedades calculadas — não persistidas ───────────────────────

        modelBuilder.Entity<Personagem>()
            .Ignore(p => p.VidaMax)
            .Ignore(p => p.CA)
            .Ignore(p => p.ManaMax)
            .Ignore(p => p.EstabilidadeMax)
            .Ignore(p => p.Acoes)
            .Ignore(p => p.Acerto)
            .Ignore(p => p.Movimento)
            .Ignore(p => p.DanoExtra)
            .Ignore(p => p.CM)
            .Ignore(p => p.Truque);

        // Item.EhArma é calculado — não persistido
        modelBuilder.Entity<Item>()
            .Ignore(i => i.EhArma);
    }
}
