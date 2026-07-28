namespace LexapadAPI.Models;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relations avec les autres fonctionnalités
    public List<Note> Notes { get; set; } = new();
    public List<CanvasItem> CanvasItems { get; set; } = new();
}