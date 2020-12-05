using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProAgil.Domain;

namespace ProAgil.Repository
{
  public class ProAgilContext : IdentityDbContext
  {
    public ProAgilContext(DbContextOptions<ProAgilContext> options) : base(options) { }

    //Cria as tabelas do banco de dados com o DbSet
    //Se precisar criar outra tabela é só repetir o processo para as classes que você desejar que tenham uma tabela, como foi o caso do Evento
    public DbSet<Evento> Eventos { get; set; }

    public DbSet<Palestrante> Palestrantes { get; set; }

    public DbSet<PalestranteEvento> PalestrantesEventos { get; set; }

    public DbSet<Lote> Lotes { get; set; }

    public DbSet<RedeSocial> RedesSociais { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<PalestranteEvento>().HasKey(PE => new { PE.EventoId, PE.PalestranteId });
    }
  }
}