using LexapadAPI.Models;

namespace LexapadAPI.Endpoints;

public static class AnalysisEndpoints
{
    public static void MapAnalysisEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/analysis");

        group.MapPost("/check", (AnalysisRequest request) =>
        {
            if (string.IsNullOrWhiteSpace(request.Content))
            {
                return Results.BadRequest(new { message = "Le texte à analyser ne peut pas être vide." });
            }

            // Mock temporaire pour valider la structure de réponse du backend
            var response = new AnalysisResponse
            {
                CorrectedText = request.Content, // On renverra la version corrigée ici
                ClarityScore = 85,
                VocabularySuggestions = new List<string>
                {
                    "Au lieu de 'faire une analyse', préfère 'mener une analyse' ou 'effectuer une étude'.",
                    "Pour exprimer ton désaccord, 'réfuter cette thèse' est plus précis que 'dire que c'est faux'."
                },
                GrammarFeedback = new List<string>
                {
                    "Attention aux accords de participes passés.",
                    "Pense à aérer tes phrases longues avec des connecteurs logiques (Cependant, Néanmoins)."
                },
                StructuralAdvice = $"[Mode {request.Context}] : Ton idée principale est bien présente. Pense à ajouter une transition explicite entre tes deux premiers paragraphes."
            };

            return Results.Ok(response);
        })
        .WithName("CheckText");
    }
}