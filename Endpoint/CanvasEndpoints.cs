using LexapadAPI.Data;
using LexapadAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LexapadAPI.Endpoints;

// DTO pour la création d'un tableau (évite les erreurs de désérialisation JSON)
public record CreateBoardDto(string Title, string? BackgroundColor);

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

        // POST /api/boards : Créer un nouveau tableau via CreateBoardDto
        group.MapPost("/", async (CreateBoardDto request, LexapadDbContext db) =>
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return Results.BadRequest("Le titre est obligatoire.");
            }

            var newBoard = new CanvasBoard
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                BackgroundColor = request.BackgroundColor ?? "#F9FAFB",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Items = new List<CanvasItem>()
            };

            db.CanvasBoards.Add(newBoard);
            await db.SaveChangesAsync();

            return Results.Created($"/api/boards/{newBoard.Id}", newBoard);
        });

        // POST /api/boards/{boardId}/items : Créer ou mettre à jour un post-it/carte
            group.MapPost("/{boardId:guid}/items", async (Guid boardId, CanvasItem itemRequest, LexapadDbContext db) =>
            {
                var board = await db.CanvasBoards.FindAsync(boardId);
                if (board is null) return Results.NotFound("Tableau introuvable");

                // 1. On cherche d'abord si la carte existe en base de données
                CanvasItem? existingItem = null;
                if (itemRequest.Id != Guid.Empty)
                {
                    existingItem = await db.CanvasItems.FindAsync(itemRequest.Id);
                }

                if (existingItem is null)
                {
                    // 2. CRÉATION : Si elle n'existe pas en base, on la crée (qu'il y ait un ID ou non)
                    if (itemRequest.Id == Guid.Empty)
                    {
                        itemRequest.Id = Guid.NewGuid();
                    }

                    itemRequest.CanvasBoardId = boardId;
                    itemRequest.UpdatedAt = DateTime.UtcNow;
                    
                    db.CanvasItems.Add(itemRequest);
                }
                else
                {
                    // 3. MISE À JOUR : Si la carte existe déjà en BDD
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