using LexapadAPI.Models;
using LexapadAPI.Services;

namespace LexapadAPI.Endpoints;

public static class EssayEndpoints
{
    public static void MapEssayEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/essays");

        // Route 1 : Générer un sujet de dissertation
        group.MapPost("/generate-prompt", async (PromptRequest request, EssayService essayService) =>
        {
            try
            {
                var prompt = await essayService.GeneratePromptAsync(request);
                return Results.Ok(prompt);
            }
            catch (Exception ex)
            {
                return Results.Problem($"Erreur lors de la génération du sujet : {ex.Message}");
            }
        })
        .WithName("GeneratePrompt");

        // Route 2 : Évaluer une dissertation complète
        group.MapPost("/grade", async (GradeEssayRequest request, EssayService essayService) =>
        {
            if (string.IsNullOrWhiteSpace(request.EssayContent))
            {
                return Results.BadRequest(new { message = "Le contenu de la dissertation ne peut pas être vide." });
            }

            try
            {
                var grade = await essayService.GradeEssayAsync(request);
                return Results.Ok(grade);
            }
            catch (Exception ex)
            {
                return Results.Problem($"Erreur lors de la notation : {ex.Message}");
            }
        })
        .WithName("GradeEssay");
    }
}