using System;

namespace LexapadAPI.Models
{
    public class Note
    {
        //1. L'identifiant unique de la note
        public Guid Id {get; set;} = Guid.NewGuid();

        //2. Le titre et le contenu textuel
        public string Title { get; set;} = string.Empty;
        public string Content {get; set;} = string.Empty;

        //3. Préférence d'affichage
        public string FontName {get; set;} = "OpenDyslexic";
        public double FontSize { get; set;} = 16.0;
        public double LetterSpacing { get; set;} = 0.15;
        public double LineHeight {get; set;} = 1.8;

        //4. Métadonnées temporelles
        public DateTime CreateAt {get; set;} = DateTime.UtcNow;
        public DateTime UpdateAt {get; set;} = DateTime.UtcNow;

        //5. Identification de l'utilisateur
        public string UserId { get; set;} =string.Empty;
    }
}