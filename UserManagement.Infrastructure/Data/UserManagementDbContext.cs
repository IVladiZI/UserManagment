using Microsoft.EntityFrameworkCore;
using UserManagement.Domain;

namespace UserManagement.Infrastructure.Data;

public class UserManagementDbContext(DbContextOptions<UserManagementDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(builder =>
        {
            builder.HasKey(u => u.Id);

            // Mapear FullName como columnas simples
            builder.OwnsOne(u => u.Name, fn =>
            {
                fn.Property(f => f.Name).HasColumnName("FirstName");
                fn.Property(f => f.LastName).HasColumnName("LastName");
                fn.Property(f => f.SecondLastName).HasColumnName("SecondLastName");
            });

            // Mapear Email como columna simple
            builder.OwnsOne(u => u.Email, e =>
            {
                e.Property(em => em.Value).HasColumnName("Email");
            });

            // Mapear Document como columna simple
            builder.OwnsOne(u => u.Document, e =>
            {
                e.Property(em => em.NumberDocument).HasColumnName("NumberDocument");
            });
        });
    }
}