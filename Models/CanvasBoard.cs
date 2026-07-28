using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LexapadAPI.Models;

// Tableau blanc visuel (Totalement indépendant de Note.cs)
public class CanvasBoard
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string Title { get; set; } = "Mon Tableau Visuel";

    public string BackgroundColor { get; set; } = "#F9FAFB"; // Couleur du fond de travail

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // 🔑 1. Clé étrangère vers l'utilisateur propriétaire
    public Guid UserId { get; set; }

    // 🔑 2. Relation vers le modèle User
    [JsonIgnore]
    public User? User { get; set; }

    // Liste des éléments appartenant uniquement à ce tableau
    public List<CanvasItem> Items { get; set; } = new();
}