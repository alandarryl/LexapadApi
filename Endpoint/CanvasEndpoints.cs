using System.Security.Claims;
using LexapadAPI.Data;
using LexapadAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LexapadAPI.Endpoints;

public static class CanvasEndpoints
{
    public static void MapCanvasEndpoints(this IEndpointRouteBuilder app)
    {
        // 🔒 Exige un Token JWT valide pour toutes les routes du Canvas
        var group = app.MapGroup("/api/canvas").RequireAuthorization();

        // Helper pour extraire l'UserId du token
        static Guid GetUserId(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var id) ? id : Guid.Empty;
        }

        // 1. Récupérer tous les tableaux de l'utilisateur connecté
        group.MapGet("/boards", async (ClaimsPrincipal user, LexapadDbContext db) =>
        {
            var userId = GetUserId(user);
            var boards = await db.CanvasBoards
                .Where(b => b.UserId == userId)
                .Include(b => b.Items)
                .ToListAsync();

            return Results.Ok(boards);
        });

        // 2. Créer un nouveau tableau
        group.MapPost("/boards", async (CanvasBoard newBoard, ClaimsPrincipal user, LexapadDbContext db) =>
        {
            var userId = GetUserId(user);
            if (userId == Guid.Empty) return Results.Unauthorized();

            newBoard.Id = Guid.NewGuid();
            newBoard.UserId = userId; // 🔑 Assigne l'utilisateur connecté
            newBoard.CreatedAt = DateTime.UtcNow;
            newBoard.UpdatedAt = DateTime.UtcNow;

            db.CanvasBoards.Add(newBoard);
            await db.SaveChangesAsync();

            return Results.Created($"/api/canvas/boards/{newBoard.Id}", newBoard);
        });

        // 3. Récupérer un tableau spécifique avec ses éléments (Post-its, cartes...)
        group.MapGet("/boards/{id:guid}", async (Guid id, ClaimsPrincipal user, LexapadDbContext db) =>
        {
            var userId = GetUserId(user);
            var board = await db.CanvasBoards
                .Include(b => b.Items)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            return board is not null ? Results.Ok(board) : Results.NotFound();
        });

        // 4. Supprimer un tableau
        group.MapDelete("/boards/{id:guid}", async (Guid id, ClaimsPrincipal user, LexapadDbContext db) =>
        {
            var userId = GetUserId(user);
            var board = await db.CanvasBoards
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (board is null) return Results.NotFound();

            db.CanvasBoards.Remove(board);
            await db.SaveChangesAsync();

            return Results.NoContent();
        });

        // 5. Ajouter un élément (Post-it / Carte) sur un tableau
        group.MapPost("/boards/{boardId:guid}/items", async (Guid boardId, CanvasItem item, ClaimsPrincipal user, LexapadDbContext db) =>
        {
            var userId = GetUserId(user);
            var board = await db.CanvasBoards
                .FirstOrDefaultAsync(b => b.Id == boardId && b.UserId == userId);

            if (board is null) return Results.NotFound("Tableau introuvable ou accès refusé.");

            item.Id = Guid.NewGuid();
            item.CanvasBoardId = boardId;
            item.UserId = userId;
            item.UpdatedAt = DateTime.UtcNow;

            db.CanvasItems.Add(item);
            await db.SaveChangesAsync();

            return Results.Created($"/api/canvas/items/{item.Id}", item);
        });

        // 6. Mettre à jour la position ou le contenu d'un élément
        group.MapPut("/items/{id:guid}", async (Guid id, CanvasItem updatedItem, ClaimsPrincipal user, LexapadDbContext db) =>
        {
            var userId = GetUserId(user);
            var existingItem = await db.CanvasItems
                .FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);

            if (existingItem is null) return Results.NotFound();

            existingItem.Type = updatedItem.Type;
            existingItem.Content = updatedItem.Content;
            existingItem.PositionX = updatedItem.PositionX;
            existingItem.PositionY = updatedItem.PositionY;
            existingItem.Width = updatedItem.Width;
            existingItem.Height = updatedItem.Height;
            existingItem.Color = updatedItem.Color;
            existingItem.ZIndex = updatedItem.ZIndex;
            existingItem.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();
            return Results.Ok(existingItem);
        });

        // 7. Supprimer un élément du tableau
        group.MapDelete("/items/{id:guid}", async (Guid id, ClaimsPrincipal user, LexapadDbContext db) =>
        {
            var userId = GetUserId(user);
            var item = await db.CanvasItems
                .FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);

            if (item is null) return Results.NotFound();

            db.CanvasItems.Remove(item);
            await db.SaveChangesAsync();

            return Results.NoContent();
        });
    }
}