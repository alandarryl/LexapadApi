namespace LexapadAPI.Models;

// 1. Demande de génération de sujet
public class PromptRequest
{
    public string Category { get; set; } = "général"; // "philosophie", "littérature", "histoire", "culture générale"
    public string Difficulty { get; set; } = "moyen"; // "facile", "moyen", "difficile"
    public string? TopicInterest { get; set; } // Optionnel : ex. "technologie", "art", "société"
}

// Réponse avec le sujet généré
public class PromptResponse
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> KeyQuestions { get; set; } = new();
    public List<string> SuggestedKeywords { get; set; } = new();
}

// 2. Demande d'évaluation de dissertation
public class GradeEssayRequest
{
    public string PromptTitle { get; set; } = string.Empty; // Le sujet de départ
    public string EssayContent { get; set; } = string.Empty; // Ton texte complet
}

// Critère de notation individuel
public class ScoringCriterion
{
    public string Category { get; set; } = string.Empty; // ex: "Problématique & Plan"
    public int Score { get; set; } // Score sur /5 ou /10
    public int MaxScore { get; set; }
    public string Feedback { get; set; } = string.Empty;
}

// Réponse complète d'évaluation
public class EssayGradeResponse
{
    public int OverallScore { get; set; } // Note sur 20
    public string GeneralFeedback { get; set; } = string.Empty;
    public List<ScoringCriterion> DetailedCriteria { get; set; } = new();
    public List<string> Strengths { get; set; } = new();
    public List<string> AreasForImprovement { get; set; } = new();
    public string RewriteExample { get; set; } = string.Empty; // Exemple de passage amélioré
}