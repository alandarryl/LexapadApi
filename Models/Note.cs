namespace LexapadAPI.Models;

public class Note
{
    public string Id { get; set; } = Guid.NewGuid().ToString(); // Ou Guid si tu préfères
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    
    // Propriétés de style existantes
    public string FontName { get; set; } = "Inter";
    public int FontSize { get; set; } = 16;
    public double LetterSpacing { get; set; } = 0;
    public double LineHeight { get; set; } = 1.5;

    // Dates
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdateAt { get; set; } = DateTime.UtcNow;

    // 🔑 1. Clé étrangère vers l'utilisateur (changée en Guid)
    public Guid UserId { get; set; }

    // 🔑 2. Relation avec la table User
    public User? User { get; set; }
}