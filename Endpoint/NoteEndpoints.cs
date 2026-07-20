using LexapadAPI.Data;
using LexapadAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LexapadAPI.Endpoints;

public static class NoteEndpoints
{
    public static void MapNoteEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/notes");

        group.MapPost("/", async (Note newNote, LexapadDbContext db) =>
        {
            newNote.Id = Guid.NewGuid();
            newNote.CreateAt = DateTime.UtcNow;
            newNote.UpdateAt = DateTime.UtcNow;
            db.Notes.Add(newNote);
            await db.SaveChangesAsync();
            return Results.Created($"/api/notes/{newNote.Id}", newNote);
        });

        group.MapGet("/", async (LexapadDbContext db) => await db.Notes.ToListAsync());

        group.MapGet("/{id:guid}", async (Guid id, LexapadDbContext db) =>
        {
            var note = await db.Notes.FindAsync(id);
            return note is not null ? Results.Ok(note) : Results.NotFound();
        });

        group.MapPut("/{id:guid}", async (Guid id, Note updatedNote, LexapadDbContext db) =>
        {
            var existingNote = await db.Notes.FindAsync(id);
            if (existingNote is null) return Results.NotFound();
            
            existingNote.Title = updatedNote.Title;
            existingNote.Content = updatedNote.Content;
            existingNote.UpdateAt = DateTime.UtcNow;
            
            await db.SaveChangesAsync();
            return Results.Ok(existingNote);
        });

        group.MapDelete("/{id:guid}", async (Guid id, LexapadDbContext db) =>
        {
            var note = await db.Notes.FindAsync(id);
            if (note is null) return Results.NotFound();
            db.Notes.Remove(note);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
    }
}