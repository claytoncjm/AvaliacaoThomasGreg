using Microsoft.EntityFrameworkCore;
using ApiCadCliente.Models;

namespace ApiCadCliente.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Logradouro> Logradouros { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar Cliente
            modelBuilder.Entity<Cliente>()
                .HasIndex(c => c.Email)
                .IsUnique()
                .HasDatabaseName("IX_Clientes_Email");

            // Configurar campos do logotipo
            modelBuilder.Entity<Cliente>()
                .Property(c => c.LogotipoBytes)
                .HasColumnType("varbinary(max)");

            modelBuilder.Entity<Cliente>()
                .Property(c => c.LogotipoContentType)
                .HasMaxLength(100);

            // Configurar relacionamento Cliente-Logradouro
            modelBuilder.Entity<Cliente>()
                .HasMany(c => c.Logradouros)
                .WithOne()
                .HasForeignKey(l => l.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuração do Logradouro
            modelBuilder.Entity<Logradouro>(entity =>
            {
                entity.Property(e => e.Endereco)
                    .IsRequired(false)
                    .HasMaxLength(200);
            });

            // Configuração do User
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();
        }
    }
}
