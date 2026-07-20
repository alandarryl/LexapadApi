using System.Text;
using System.Text.Json;
using LexapadAPI.Models;

namespace LexapadAPI.Services;

public class EssayService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public EssayService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["Groq:ApiKey"] 
            ?? throw new InvalidOperationException("Clé API Groq manquante.");
    }

    // 1. Génération de sujet
    public async Task<PromptResponse> GeneratePromptAsync(PromptRequest request)
    {
        var requestUrl = "https://api.groq.com/openai/v1/chat/completions";

        var systemPrompt = $@"
Tu es un professeur inspirant et pédagogique sur Lexapad.
Génère un sujet de dissertation stimulant.
Catégorie : {request.Category}.
Niveau : {request.Difficulty}.
Domaine d'intérêt spécifique : {request.TopicInterest ?? "Au choix"}.

Tu DOIS répondre STRICTEMENT au format JSON (sans markdown) :
{{
  ""title"": ""Le sujet sous forme de question ou d'affirmation à discuter"",
  ""description"": ""Une courte mise en contexte du sujet"",
  ""keyQuestions"": [""Question de réflexion 1"", ""Question 2""],
  ""suggestedKeywords"": [""Mot-clé 1"", ""Mot-clé 2""]
}}";

        var payload = new
        {
            model = "llama-3.3-70b-versatile",
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = "Génère un sujet de dissertation." }
            },
            temperature = 0.7,
            response_format = new { type = "json_object" }
        };

        return await SendGroqRequestAsync<PromptResponse>(requestUrl, payload);
    }

    // 2. Évaluation de la dissertation
    public async Task<EssayGradeResponse> GradeEssayAsync(GradeEssayRequest request)
    {
        var requestUrl = "https://api.groq.com/openai/v1/chat/completions";

        var systemPrompt = $@"
Tu es un correcteur d'examen bienveillant, exigeant et constructif pour Lexapad.
Ton objectif est de noter cette dissertation sur 20 et d'expliquer comment progresser.

Sujet traité : {request.PromptTitle}

Évalue selon 4 critères :
1. Accroche, Problématique & Plan (/5)
2. Cohérence de l'Argumentation & Exemples (/5)
3. Qualité de la Langue, Orthographe & Richesse du Vocabulaire (/5)
4. Conclusion & Réflexion globale (/5)

Tu DOIS répondre STRICTEMENT au format JSON (sans markdown) :
{{
  ""overallScore"": 14,
  ""generalFeedback"": ""Un avis général encouragant et synthétique sur la prestation"",
  ""detailedCriteria"": [
    {{
      ""category"": ""Problématique & Plan"",
      ""score"": 3,
      ""maxScore"": 5,
      ""feedback"": ""Analyse du plan...""
    }},
    {{
      ""category"": ""Argumentation & Exemples"",
      ""score"": 4,
      ""maxScore"": 5,
      ""feedback"": ""Analyse des arguments...""
    }},
    {{
      ""category"": ""Langue & Vocabulaire"",
      ""score"": 3,
      ""maxScore"": 5,
      ""feedback"": ""Analyse du vocabulaire...""
    }},
    {{
      ""category"": ""Conclusion"",
      ""score"": 4,
      ""maxScore"": 5,
      ""feedback"": ""Analyse de la conclusion...""
    }}
  ],
  ""strengths"": [""Point fort 1"", ""Point fort 2""],
  ""areasForImprovement"": [""Axe de progrès 1"", ""Axe de progrès 2""],
  ""rewriteExample"": ""Prends un paragraphe faible du texte et montre comment le réécrire de manière fluide et soutenue.""
}}";

        var payload = new
        {
            model = "llama-3.3-70b-versatile",
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = request.EssayContent }
            },
            temperature = 0.2,
            response_format = new { type = "json_object" }
        };

        return await SendGroqRequestAsync<EssayGradeResponse>(requestUrl, payload);
    }

    // Méthode utilitaire générique pour Groq
    private async Task<T> SendGroqRequestAsync<T>(string url, object payload)
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
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
        return JsonSerializer.Deserialize<T>(aiContent ?? "{}", options)!;
    }
}