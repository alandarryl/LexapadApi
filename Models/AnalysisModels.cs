namespace LexapadAPI.Models;

// Ce que le Frontend envoie
public class AnalysisRequest
{
    public string Content { get; set; } = string.Empty;
    public string Context { get; set; } = "general"; // "dissertation", "mail", "script", "general"
}

// Ce que le Backend renvoie
public class AnalysisResponse
{
    public string CorrectedText { get; set; } = string.Empty;
    public List<string> VocabularySuggestions { get; set; } = new();
    public List<string> GrammarFeedback { get; set; } = new();
    public string StructuralAdvice { get; set; } = string.Empty;
    public int ClarityScore { get; set; } // Note sur 100
}