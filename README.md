📝 Lexapad API — Documentation Technique Backend (.NET 10)
Description du projet : API Backend pour Lexapad, un environnement d'écriture, d'apprentissage et de structuration de la pensée conçu pour surmonter les blocages d'écriture (dyslexie, manque de confiance, vocabulaire restreint).

🏗️ 1. Architecture & Stack Technique
Framework : ASP.NET Core Minimal APIs (.NET 10)

ORM & Base de données : Entity Framework Core avec PostgreSQL hébergé sur Supabase

Moteur d'IA : Service REST basé sur l'API Groq (Llama 3.3 70B, 100% gratuit & ultra-rapide)

Inclusion CORS : Politique AllowAll active pour faciliter l'intégration avec le Frontend (React, Vue, Flutter, etc.)

📁 Structure des Fichiers du Projet
Plaintext
LexapadAPI/
├── Program.cs                  # Point d'entrée, CORS, Injection de Dépendances, Mapping des routes
├── appsettings.json            # Configuration (Chaîne Supabase, Clé API Groq)
├── Data/
│   └── LexapadDbContext.cs     # Context EF Core (Tables Notes, CanvasBoards, CanvasItems)
├── Models/
│   ├── Note.cs                 # Modèle pour les notes textuelles de base
│   ├── AnalysisModels.cs       # DTOs pour la route d'analyse de texte
│   ├── EssayModels.cs          # DTOs pour les sujets et la notation de dissertations
│   ├── CanvasBoard.cs          # Modèle pour les tableaux visuels (style Milanote)
│   └── CanvasItem.cs           # Modèle pour les post-its/cartes du canvas
├── Services/
│   ├── AnalysisService.cs      # Service de correction et conseils de vocabulaire (Groq)
│   └── EssayService.cs         # Service de génération et correction de dissertations (Groq)
└── Endpoints/
    ├── NoteEndpoints.cs        # CRUD des notes classiques (/api/notes)
    ├── AnalysisEndpoints.cs    # Moteur d'analyse linguistique (/api/analysis)
    ├── EssayEndpoints.cs       # Moteur de dissertation (/api/essays)
    └── CanvasEndpoints.cs      # Espaces visuels & post-its (/api/boards)
🔌 2. Documentation des Endpoints (API)
📄 Module 1 : Notes de base (/api/notes)
Gère la prise de notes textuelles au fil de l'eau (fonctionnalité classique).

GET /api/notes : Récupère toutes les notes.

GET /api/notes/{id} : Récupère une note par son ID (Guid).

POST /api/notes : Crée une nouvelle note.

PUT /api/notes/{id} : Met à jour une note.

DELETE /api/notes/{id} : Supprime une note.


🔍 Module 2 : Analyse & Correction Pédagogique (/api/analysis)
Analyse les textes, suggère du vocabulaire soutenu et donne des conseils de structure.

POST /api/analysis/check

Body JSON :

JSON
{
  "content": "Je veux faire une analyse pour dire que c'est faux.",
  "context": "dissertation"
}
Réponse JSON :

JSON
{
  "correctedText": "Je veux mener une analyse pour réfuter cette thèse.",
  "vocabularySuggestions": ["Au lieu de 'faire une analyse', préfère 'mener une analyse'."],
  "grammarFeedback": ["Explication pédagogique sur l'accord..."],
  "structuralAdvice": "Pense à faire une transition entre les paragraphes.",
  "clarityScore": 85
}
📝 Module 3 : Entraînement aux Dissertations (/api/essays)
Génère des sujets sur-mesure et note les rédactions complètes sur 20.

POST /api/essays/generate-prompt

Body JSON :

JSON
{
  "category": "philosophie",
  "difficulty": "moyen",
  "topicInterest": "intelligence artificielle"
}
Réponse JSON : Renvoie le titre du sujet, les questions clés et les mots-clés suggérés.

POST /api/essays/grade

Body JSON :

JSON
{
  "promptTitle": "L'IA menace-t-elle la liberté ?",
  "essayContent": "Texte complet de la dissertation..."
}
Réponse JSON : Note globale sur /20, détail sur 4 critères (/5 chacun), points forts, axes de progrès et exemple de réécriture d'un passage.


🎨 Module 4 : Canvas Visuel & Post-its style Milanote (/api/boards)
Espace de brainstorming spatial indépendant de la prise de note classique.

GET /api/boards : Liste tous les tableaux visuels.

GET /api/boards/{id} : Récupère un tableau complet avec ses post-its et cartes.

POST /api/boards : Crée un nouveau tableau visuel.

POST /api/boards/{boardId}/items : Crée ou met à jour un post-it/carte sur le tableau (coordonnées $X/Y$, couleur, dimensions).

DELETE /api/boards/items/{itemId} : Supprime un post-it/carte.

🛠️ 3. Démarrage Rapide (Pour réouvrir le projet)
Cloner le projet et ouvrir le dossier dans VS Code.

Vérifier les variables d'environnement / appsettings.json :

Chaîne de connexion PostgreSQL Supabase (SupabaseConnection).

Clé API Groq (Groq:ApiKey).

Lancer l'API :

Bash
dotnet run
Tester l'API :
L'API tournera sur http://localhost:5278 (ou le port indiqué dans la console). Tu peux utiliser Thunder Client ou Postman pour exécuter les requêtes.

