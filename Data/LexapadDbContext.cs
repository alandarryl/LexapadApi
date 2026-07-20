using Microsoft.EntityFrameworkCore;
using LexapadAPI.Models;

namespace LexapadAPI.Data
{
    public class LexapadDbContext : DbContext
    {
        // Constructeur obligatoire pour qu'ASP.NET Core puisse lui injecter la configuration (chaîne de connexion)
        public LexapadDbContext(DbContextOptions<LexapadDbContext> options) : base(options)
        {
        }

        // Table des notes textuelles classiques (inchangée)
        public DbSet<Note> Notes { get; set; }

        // Nouvelles tables dédiées à la fonctionnalité Canvas / Milanote (indépendantes des notes classiques)
        public DbSet<CanvasBoard> CanvasBoards { get; set; }
        public DbSet<CanvasItem> CanvasItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Indique à EF Core qu'un tableau contient plusieurs éléments (post-its, cartes)
            // et que si on supprime un tableau, ses éléments sont supprimés automatiquement.
            modelBuilder.Entity<CanvasBoard>()
                .HasMany(b => b.Items)
                .WithOne(i => i.CanvasBoard)
                .HasForeignKey(i => i.CanvasBoardId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}