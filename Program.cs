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

//Route GET : Récupérer toutes les notes
app.MapGet("/api/notes", async (LexapadAPI.Data.LexapadDbContext db) =>
{
    // On demande à Entity Framework d'aller chercher toutes les notes
    // et de les transformer en une liste asynchrone
    var notes = await db.Notes.ToListAsync();

    //On renvoie un statut 200 ok avec la liste des notes en JSON
    return Results.Ok(notes);
})
.WithName("GetAllNotes");

//Route GET : Récupérer une note spécifique par son ID
app.MapGet("/api/notes/{id:guid}", async (Guid id, LexapadAPI.Data.LexapadDbContext db) =>
{
    //On cherche la note dans la table par sa clé primaire (Id)
    var note = await db.Notes.FindAsync(id);

    //Si la note n'existe pas, on renvoie un statut 404 Not Found
    if(note is null)
    {
        return Results.NotFound(new { message = "Désolé, cette note est introuvable."});

    }
        //Si on la trouve, on renvoie un statut 200 OK avec la note
        return Results.Ok(note); 
})
.WithName("GetNoteById");

// Le serveur démarre ici
app.Run();