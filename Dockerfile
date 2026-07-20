# 1. Étape de Build avec le SDK .NET 10
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["LexapadAPI.csproj", "./"]
RUN dotnet restore "LexapadAPI.csproj"
COPY . .
RUN dotnet publish "LexapadAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 2. Étape d'exécution (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "LexapadAPI.dll"]