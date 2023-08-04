using Microsoft.EntityFrameworkCore;
using TrabalhoModuloTres.Api.Entities;

namespace TrabalhoModuloTres.Api.DbContexts;

public class ClienteContext : DbContext
{
    public DbSet<Cliente> Clientes { get; set; } = null!;
    public DbSet<Cidade> Cidades { get; set; } = null!;

    public ClienteContext(DbContextOptions<ClienteContext> options)
    : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ClienteInitFluentApi(modelBuilder);
        CidadeInitFluentApi(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    private static void ClienteInitFluentApi(ModelBuilder modelBuilder)
    {
        var cliente = modelBuilder.Entity<Cliente>();

        cliente
            .HasOne(cliente => cliente.Cidade)
            .WithMany(cidade => cidade.Clientes);

        cliente.Property(cliente => cliente.Nome)
            .HasMaxLength(60)
            .IsRequired();

        cliente.Property(cliente => cliente.Endereco)
            .HasMaxLength(100)
            .IsRequired();

        cliente.Property(cliente => cliente.Telefone)
            .HasMaxLength(18)
            .IsRequired();
    }

    private static void CidadeInitFluentApi(ModelBuilder modelBuilder)
    {
        var cidade = modelBuilder.Entity<Cidade>();

        cidade
            .HasMany(cidade => cidade.Clientes)
            .WithOne(cliente => cliente.Cidade)
            .HasForeignKey(cliente => cliente.CidadeId);

        cidade.Property(cidade => cidade.Nome)
            .HasMaxLength(60)
            .IsRequired();

        cidade.Property(cidade => cidade.UF)
            .HasMaxLength(2)
            .IsRequired();
    }
}