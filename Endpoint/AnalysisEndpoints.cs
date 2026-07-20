using LexapadAPI.Models;
using LexapadAPI.Services;

namespace LexapadAPI.Endpoints;

public static class AnalysisEndpoints
{
    public static void MapAnalysisEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/analysis");

        group.MapPost("/check", async (AnalysisRequest request, AnalysisService analysisService) =>
        {
            if (string.IsNullOrWhiteSpace(request.Content))
            {
                return Results.BadRequest(new { message = "Le texte à analyser ne peut pas être vide." });
            }

            try
            {
                var response = await analysisService.AnalyzeTextAsync(request.Content, request.Context);
                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                return Results.Problem($"Erreur lors de l'analyse linguistique : {ex.Message}");
            }
        })
        .WithName("CheckText");
    }
}