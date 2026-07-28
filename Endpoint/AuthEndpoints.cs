using LexapadAPI.Data;
using LexapadAPI.DTOs;
using LexapadAPI.Models;
using LexapadAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace LexapadAPI.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");

        // Route 1 : Inscription
        group.MapPost("/register", async (RegisterDto dto, LexapadDbContext db, AuthService authService) =>
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return Results.BadRequest(new { message = "L'email et le mot de passe sont requis." });
            }

            var normalizedEmail = dto.Email.Trim().ToLower();

            // Vérifier si l'utilisateur existe déjà
            var existingUser = await db.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail);
            if (existingUser != null)
            {
                return Results.BadRequest(new { message = "Un compte avec cet email existe déjà." });
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = normalizedEmail,
                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash = authService.HashPassword(user, dto.Password);

            db.Users.Add(user);
            await db.SaveChangesAsync();

            var token = authService.GenerateJwtToken(user);

            return Results.Ok(new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                UserId = user.Id
            });
        });

        // Route 2 : Connexion
        group.MapPost("/login", async (LoginDto dto, LexapadDbContext db, AuthService authService) =>
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return Results.BadRequest(new { message = "L'email et le mot de passe sont requis." });
            }

            var normalizedEmail = dto.Email.Trim().ToLower();

            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail);
            if (user == null || !authService.VerifyPassword(user, dto.Password))
            {
                return Results.BadRequest(new { message = "Email ou mot de passe incorrect." });
            }

            var token = authService.GenerateJwtToken(user);

            return Results.Ok(new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                UserId = user.Id
            });
        });
    }
}