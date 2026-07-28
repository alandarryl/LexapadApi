using System.Security.Claims;
using LexapadAPI.Data;
using LexapadAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LexapadAPI.Endpoints;

public static class NoteEndpoints
{
    public static void MapNoteEndpoints(this IEndpointRouteBuilder app)
    {
        // 🔒 Ajout de RequireAuthorization() sur le groupe
        var group = app.MapGroup("/api/notes").RequireAuthorization();

        // Méthode utilitaire pour extraire l'UserId du token JWT
        static Guid GetUserId(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var id) ? id : Guid.Empty;
        }

        // POST : Créer une note pour l'utilisateur connecté
        group.MapPost("/", async (Note newNote, ClaimsPrincipal user, LexapadDbContext db) =>
        {
            var userId = GetUserId(user);
            if (userId == Guid.Empty) return Results.Unauthorized();

            newNote.Id = Guid.NewGuid().ToString();
            newNote.UserId = userId; // 🔑 Liaison automatique à l'utilisateur
            newNote.CreatedAt = DateTime.UtcNow;
            newNote.UpdateAt = DateTime.UtcNow;

            db.Notes.Add(newNote);
            await db.SaveChangesAsync();
            return Results.Created($"/api/notes/{newNote.Id}", newNote);
        });

        // GET : Récupérer UNIQUEMENT les notes de l'utilisateur connecté
        group.MapGet("/", async (ClaimsPrincipal user, LexapadDbContext db) =>
        {
            var userId = GetUserId(user);
            var notes = await db.Notes
                .Where(n => n.UserId == userId)
                .ToListAsync();

            return Results.Ok(notes);
        });

        // GET /{id} : Récupérer une note spécifique si elle appartient à l'utilisateur
        group.MapGet("/{id}", async (string id, ClaimsPrincipal user, LexapadDbContext db) =>
        {
            var userId = GetUserId(user);
            var note = await db.Notes
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            return note is not null ? Results.Ok(note) : Results.NotFound();
        });

        // PUT /{id} : Modifier une note si elle appartient à l'utilisateur
        group.MapPut("/{id}", async (string id, Note updatedNote, ClaimsPrincipal user, LexapadDbContext db) =>
        {
            var userId = GetUserId(user);
            var existingNote = await db.Notes
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (existingNote is null) return Results.NotFound();

            existingNote.Title = updatedNote.Title;
            existingNote.Content = updatedNote.Content;
            existingNote.FontName = updatedNote.FontName;
            existingNote.FontSize = updatedNote.FontSize;
            existingNote.LetterSpacing = updatedNote.LetterSpacing;
            existingNote.LineHeight = updatedNote.LineHeight;
            existingNote.UpdateAt = DateTime.UtcNow;

            await db.SaveChangesAsync();
            return Results.Ok(existingNote);
        });

        // DELETE /{id} : Supprimer une note si elle appartient à l'utilisateur
        group.MapDelete("/{id}", async (string id, ClaimsPrincipal user, LexapadDbContext db) =>
        {
            var userId = GetUserId(user);
            var note = await db.Notes
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (note is null) return Results.NotFound();

            db.Notes.Remove(note);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
    }
}