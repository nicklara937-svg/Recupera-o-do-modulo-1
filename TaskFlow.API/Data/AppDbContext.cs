using Microsoft.EntityFrameworkCore;
using TaskFlow.API.Models.Entities;

namespace TaskFlow.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Tarefa> Tarefas => Set<Tarefa>();
    public DbSet<Chamado> Chamados => Set<Chamado>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Interacao> Interacoes => Set<Interacao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tarefa>(entidade =>
        {
            entidade.ToTable("Tarefas");
            entidade.HasKey(t => t.Id);

            entidade.Property(t => t.Titulo).IsRequired().HasMaxLength(200);
            entidade.Property(t => t.Descricao).HasMaxLength(1000);
            entidade.Property(t => t.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
            entidade.Property(t => t.DataCriacao).IsRequired();

            // Índice para acelerar o filtro por status (RF08)
            entidade.HasIndex(t => t.Status);

            // Relacionamento 1:N (Tarefa -> Chamados)
            entidade.HasMany(t => t.Chamados)
                .WithOne(c => c.Tarefa)
                .HasForeignKey(c => c.TarefaId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Chamado>(entidade =>
        {
            entidade.ToTable("Chamados");
            entidade.HasKey(c => c.Id);

            entidade.Property(c => c.Titulo).IsRequired().HasMaxLength(200);
            entidade.Property(c => c.Descricao).HasMaxLength(1000);
            entidade.Property(c => c.DataCriacao).IsRequired();

            // Categoria é opcional: ao excluir a categoria, o chamado permanece sem categoria
            entidade.HasOne(c => c.Categoria)
                .WithMany(cat => cat.Chamados)
                .HasForeignKey(c => c.CategoriaId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Categoria>(entidade =>
        {
            entidade.ToTable("Categorias");
            entidade.HasKey(c => c.Id);

            entidade.Property(c => c.Nome).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Interacao>(entidade =>
        {
            entidade.ToTable("Interacoes");
            entidade.HasKey(i => i.Id);

            entidade.Property(i => i.Mensagem).IsRequired().HasMaxLength(1000);
            entidade.Property(i => i.DataCriacao).IsRequired();

            entidade.HasOne(i => i.Chamado)
                .WithMany(c => c.Interacoes)
                .HasForeignKey(i => i.ChamadoId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
