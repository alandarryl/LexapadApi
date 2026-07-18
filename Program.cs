var builder = WebApplication.CreateBuilder(args);

// Enregistrement des services
builder.Services.AddOpenApi();

var app = builder.Build();

// Configuration du pipeline de middlewares
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// --- NOS ROUTES ---

app.MapGet("/hello", () =>
{
    return "Bienvenue sur Lexapad, ton API de prise de notes accessible !";
})
.WithName("GetWelcomeMessage");

app.MapGet("/world", () =>
{
    return "The world of the living is there";   
})
.WithName("GetWorld");

// Le serveur démarre ici
app.Run();