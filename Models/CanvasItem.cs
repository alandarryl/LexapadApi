using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LexapadAPI.Models;

// Élément individuel (Post-it, Carte texte, Image) posé sur un CanvasBoard
public class CanvasItem
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid CanvasBoardId { get; set; }

    [JsonIgnore]
    public CanvasBoard? CanvasBoard { get; set; }

    // Type d'élément : "postit", "card", "text"
    public string Type { get; set; } = "postit";

    public string Content { get; set; } = string.Empty;

    // Positionnement spatial sur l'écran
    public double PositionX { get; set; } = 0;
    public double PositionY { get; set; } = 0;

    // Dimensions de l'élément
    public double Width { get; set; } = 200;
    public double Height { get; set; } = 200;

    // Style visuel
    public string Color { get; set; } = "#FEF08A"; // Jaune post-it par défaut
    public int ZIndex { get; set; } = 1;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}