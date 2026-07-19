using LexapadAPI.Data;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

//1. On récupère l'url de connexion cachée dans appsettings.json
var connectionString = builder.Configuration.GetConnectionString("SupabaseConnection");

//2. On branche notre lexapaddBbContext à PostgreSQL (Supabase)
builder.Services.AddDbContext<LexapadDbContext>(options =>
    options.UseNpgsql(connectionString));

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

// --- NOS ROUTES CRUD ---

// Route POST : Créer une nouvelle note
app.MapPost("/api/notes", async (LexapadAPI.Models.Note newNote, LexapadAPI.Data.LexapadDbContext db) =>
{
    // 1. On s'assure que la note a bien un ID unique et des dates fraîches
    newNote.Id = Guid.NewGuid();
    newNote.CreateAt = DateTime.UtcNow;
    newNote.UpdateAt = DateTime.UtcNow;

    // 2. On demande à Entity Framework de préparer l'ajout de la note
    db.Notes.Add(newNote);

    // 3. On pousse le changement physiquement dans la base Supabase
    await db.SaveChangesAsync();

    // 4. On renvoie un statut "201 Created" avec la note qu'on vient de stocker
    return Results.Created($"/api/notes/{newNote.Id}", newNote);
})
.WithName("CreateNote");

// Le serveur démarre ici
app.Run();