using System.Text;
using System.Text.Json;
using LexapadAPI.Models;

namespace LexapadAPI.Services;

public class AnalysisService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public AnalysisService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["Groq:ApiKey"] 
            ?? throw new InvalidOperationException("Clé API Groq manquante dans appsettings.json");
    }

    public async Task<AnalysisResponse> AnalyzeTextAsync(string content, string context)
    {
        var requestUrl = "https://api.groq.com/openai/v1/chat/completions";

        var systemPrompt = $@"
Tu es un assistant linguistique et pédagogique expert pour l'application Lexapad.
Ton rôle est d'analyser le texte de l'utilisateur et d'aider les personnes ayant des difficultés de rédaction ou des signes de dyslexie à s'améliorer.

Contexte de l'écrit : {context} (dissertation, mail, script ou général).

Tu DOIS répondre STRICTEMENT au format JSON avec cette structure exacte (sans bloc markdown triple backticks) :
{{
  ""correctedText"": ""Version corrigée sans fautes d'orthographe ni de grammaire"",
  ""vocabularySuggestions"": [""Suggestion 1 de synonymes ou mots plus soutenus"", ""Suggestion 2""],
  ""grammarFeedback"": [""Explication pédagogique 1 sur une faute"", ""Explication 2""],
  ""structuralAdvice"": ""Conseil sur la structure, les transitions et la clarté du texte"",
  ""clarityScore"": 80
}}";

        var payload = new
        {
            model = "llama-3.3-70b-versatile",
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content }
            },
            temperature = 0.3,
            response_format = new { type = "json_object" }
        };

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUrl);
        requestMessage.Headers.Add("Authorization", $"Bearer {_apiKey}");
        requestMessage.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(requestMessage);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(jsonResponse);
        var aiContent = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var result = JsonSerializer.Deserialize<AnalysisResponse>(aiContent ?? "{}", options);

        return result ?? new AnalysisResponse { CorrectedText = content };
    }
}