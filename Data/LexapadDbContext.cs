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
        //c'est cette propriété qui dit à .NET : "Crée une table 'Note' basée sur mon modèle 'Note'"
        public DbSet<Note> Notes {get; set;}
    }
}


