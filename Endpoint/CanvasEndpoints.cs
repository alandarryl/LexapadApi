using LexapadAPI.Data;
using LexapadAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LexapadAPI.Endpoints;

public static class CanvasEndpoints
{
    public static void MapCanvasEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/boards");

        // GET /api/boards : Récupérer tous les tableaux
        group.MapGet("/", async (LexapadDbContext db) =>
        {
            return await db.CanvasBoards
                .Include(b => b.Items)
                .ToListAsync();
        });

        // GET /api/boards/{id} : Récupérer un tableau par ID avec ses cartes
        group.MapGet("/{id:guid}", async (Guid id, LexapadDbContext db) =>
        {
            var board = await db.CanvasBoards
                .Include(b => b.Items)
                .FirstOrDefaultAsync(b => b.Id == id);

            return board is not null ? Results.Ok(board) : Results.NotFound();
        });

        // POST /api/boards : Créer un nouveau tableau
        group.MapPost("/", async (CanvasBoard newBoard, LexapadDbContext db) =>
        {
            newBoard.Id = Guid.NewGuid();
            newBoard.CreatedAt = DateTime.UtcNow;
            newBoard.UpdatedAt = DateTime.UtcNow;
            
            db.CanvasBoards.Add(newBoard);
            await db.SaveChangesAsync();
            return Results.Created($"/api/boards/{newBoard.Id}", newBoard);
        });

        // POST /api/boards/{boardId}/items : Créer ou mettre à jour un post-it/carte
        group.MapPost("/{boardId:guid}/items", async (Guid boardId, CanvasItem itemRequest, LexapadDbContext db) =>
        {
            var board = await db.CanvasBoards.FindAsync(boardId);
            if (board is null) return Results.NotFound("Tableau introuvable");

            if (itemRequest.Id == Guid.Empty)
            {
                // CRÉATION : On utilise CanvasBoardId (nom exact dans ton modèle !)
                itemRequest.Id = Guid.NewGuid();
                itemRequest.CanvasBoardId = boardId;
                itemRequest.UpdatedAt = DateTime.UtcNow;
                
                db.CanvasItems.Add(itemRequest);
            }
            else
            {
                // MISE À JOUR d'une carte existante
                var existingItem = await db.CanvasItems.FindAsync(itemRequest.Id);
                if (existingItem is null) return Results.NotFound("Carte introuvable");

                existingItem.Content = itemRequest.Content;
                existingItem.PositionX = itemRequest.PositionX;
                existingItem.PositionY = itemRequest.PositionY;
                existingItem.Width = itemRequest.Width;
                existingItem.Height = itemRequest.Height;
                existingItem.Color = itemRequest.Color;
                existingItem.Type = itemRequest.Type;
                existingItem.ZIndex = itemRequest.ZIndex;
                existingItem.UpdatedAt = DateTime.UtcNow;
            }

            await db.SaveChangesAsync();
            return Results.Ok(itemRequest);
        });

        // DELETE /api/boards/items/{itemId} : Supprimer une carte
        group.MapDelete("/items/{itemId:guid}", async (Guid itemId, LexapadDbContext db) =>
        {
            var item = await db.CanvasItems.FindAsync(itemId);
            if (item is null) return Results.NotFound();

            db.CanvasItems.Remove(item);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
    }
}