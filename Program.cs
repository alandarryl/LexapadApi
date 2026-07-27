using LexapadAPI.Data;
using LexapadAPI.Endpoints; // <--- Import nécessaire
using LexapadAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuration de la DB (Supabase/Postgres)
builder.Services.AddDbContext<LexapadDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("SupabaseConnection")));

// Enregistrement de HttpClient et de notre AnalysisService
builder.Services.AddHttpClient<AnalysisService>();
builder.Services.AddHttpClient<EssayService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // Optionnel si tu utilises des cookies/tokens
    });
});

var app = builder.Build();

app.UseCors("AllowAll");

// Tes routes sont maintenant appelées ici
app.MapNoteEndpoints(); 
app.MapAnalysisEndpoints();
app.MapEssayEndpoints();
app.MapCanvasEndpoints();


app.Run();