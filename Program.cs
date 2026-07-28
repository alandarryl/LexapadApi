using System.Text;
using LexapadAPI.Data;
using LexapadAPI.Endpoints;
using LexapadAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuration de la DB
builder.Services.AddDbContext<LexapadDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("SupabaseConnection")));

// 2. Enregistrement des Services
builder.Services.AddScoped<AuthService>();
builder.Services.AddHttpClient<AnalysisService>();
builder.Services.AddHttpClient<EssayService>();

// 3. Configuration de l'authentification JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["Secret"] ?? "SuperSecretKeyLexapadDefaultKey1234567890!";
var key = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    // 🔑 Correction ici : utilisation de JwtBearerDefaults.AuthenticationScheme
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// 4. Configuration CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors("AllowAll");

// 🔑 Middleware d'authentification & d'autorisation
app.UseAuthentication();
app.UseAuthorization();

// Map des endpoints
app.MapAuthEndpoints();
app.MapNoteEndpoints();
app.MapAnalysisEndpoints();
app.MapEssayEndpoints();
app.MapCanvasEndpoints();

app.Run();