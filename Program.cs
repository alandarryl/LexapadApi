using LexapadAPI.Data;
using LexapadAPI.Endpoints; // <--- Import nécessaire
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuration de la DB (Supabase/Postgres)
builder.Services.AddDbContext<LexapadDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("SupabaseConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin() // À restreindre plus tard avec ton URL de prod
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("AllowAll");

// Tes routes sont maintenant appelées ici
app.MapNoteEndpoints(); 

app.Run();