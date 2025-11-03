using Newtonsoft.Json;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lyric_Cover_Manager
{
    public class SongManager
    {
        private const string FilePath = "songs.json";

        public List<Song> Songs;

        public SongManager()
        {
            Songs = LoadSongs();
        }

        public List<Song> LoadSongs()
        {
            if (!File.Exists(FilePath))
                return new List<Song>();
            string json = File.ReadAllText(FilePath);
            return JsonConvert.DeserializeObject<List<Song>>(json) ?? new List<Song>();
        }

        public void SaveSongs()
        {
            string json = JsonConvert.SerializeObject(Songs, Formatting.Indented);
            File.WriteAllText(FilePath, json); // FilePath = din path till songs.json
            AnsiConsole.MarkupLine($"[italic grey]Songs saved to: {FilePath}[/]");
        }

        public void AddSong(Song song)
        {
            Songs.Add(song);
        }

        public IEnumerable<Song> Search(string query)
        {
            return Songs.Where(s =>
                s.Artist.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                s.Title.Contains(query, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<Song> FilterByGenre(string genre)
        {
            return Songs
                .Where(s => s.Genre.Equals(genre, StringComparison.OrdinalIgnoreCase))
                .OrderBy(s => s.Title);
        }

        public IEnumerable<Song> FilterByStatus(string status)
        {
            return Songs
                .Where(s => s.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<Song> GetRecent(int count = 5)
        {
            return Songs
                .OrderByDescending(s => s.LastRehearsed)
                .Take(count);
        }
        public void AddSongInteractive()
        {
            var title = MenuHelper.GetInput("Titel");
            var artist = MenuHelper.GetInput("Artist");

            // Hämta metadata automatiskt
            var (genre, year) = SongMetadataImporter.FetchMetadata(artist, title);

            var song = new Song
            {
                Title = title,
                Artist = artist,
                Genre = genre,
                Language = MenuHelper.GetInput("Språk"),
                Notes = MenuHelper.GetInput("Anteckningar"),
                Status = MenuHelper.GetInput("Status"),
                LastRehearsed = DateTime.Now,
                Year = year,
                Lyrics = new List<SongSection>()
            };

            // 1. Hitta lyrics-sidan från Genius
            string songUrl = GeniusImporter.SearchSong(artist, title);
            if (string.IsNullOrWhiteSpace(songUrl))
            {
                AnsiConsole.MarkupLine("[red]Kunde inte hitta låten på Genius![/]");
                return;
            }

            // 2. Hämta och sektionera lyrics direkt från Genius-sidan
            var sections = GeniusImporter.FetchSectionsFromGeniusPage(songUrl);
            if (sections == null || sections.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]Kunde inte hitta lyrics på Genius![/]");
                return;
            }

            // 3. Låt användaren granska och ev. ändra sektionstyper
            for (int i = 0; i < sections.Count; i++)
            {
                SongPresenter.ShowSection(sections[i]);
                var newType = MenuHelper.GetInput($"Vill du ändra sektionstypen för ovan? ({sections[i].SectionType}) Tryck enter för att behålla.");
                if (!string.IsNullOrWhiteSpace(newType))
                    sections[i].SectionType = newType;
            }
            song.Lyrics.AddRange(sections);

            // 4. Visa låten snyggt med Spectre.Console
            AnsiConsole.MarkupLine($"[bold green]);Låten hittades och lyrics importerade![/]");

            // 5. Lägg till låten i samlingen och spara
            Songs.Add(song);
            SaveSongs();
            AnsiConsole.MarkupLine("[bold green]Låt tillagd och sparad![/]");
        }

        public void EditLyricsInteractive()
        {
            string query = MenuHelper.GetInput("Vilken låt vill du ändra? Skriv titel eller artist");
            var found = Search(query).ToList();

            if (found.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]Ingen låt hittad.[/]");
                return;
            }
            SongPresenter.ShowSongs(found);

            Console.WriteLine("Skriv exakt titel på låten du vill ändra:");
            string title = Console.ReadLine();
            var song = Songs.FirstOrDefault(s => s.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
            if (song == null)
            {
                AnsiConsole.MarkupLine("[red]Titel ej funnen.[/]");
                return;
            }

            // Visa befintliga sektioner
            AnsiConsole.MarkupLine("[bold yellow]Nuvarande låtstruktur:[/]");
            int i = 1;
            foreach (var section in song.Lyrics)
            {
                // Använd GetSectionStyle från SongPresenter istället
                SongPresenter.ShowSection(section);
                i++;
            }

            string editSec = MenuHelper.GetInput("Nummer på sektion att ändra (eller 'ny' för ny sektion)");
            if (editSec.ToLower() == "ny")
            {
                var sectionType = MenuHelper.GetInput("Sektionstyp (Verse, Chorus, Bridge, etc)").Trim();
                var lines = new List<string>();
                AnsiConsole.MarkupLine($"[green]Skriv lyrics en rad i taget! Tom rad för att avsluta sektion.[/]");
                while (true)
                {
                    string line = MenuHelper.GetInput($"{sectionType} rad");
                    if (string.IsNullOrWhiteSpace(line)) break;
                    lines.Add(line);
                }
                song.Lyrics.Add(new SongSection
                {
                    SectionType = sectionType,
                    Lines = lines
                });
            }
            else if (int.TryParse(editSec, out int secNum) && secNum > 0 && secNum <= song.Lyrics.Count)
            {
                var section = song.Lyrics[secNum - 1];
                AnsiConsole.MarkupLine($"[bold cyan]Redigerar sektion: {section.SectionType}[/]");
                var newLines = new List<string>();
                AnsiConsole.MarkupLine("[green]Skriv nya rader (tom rad avslutar sektionen)[/]");
                while (true)
                {
                    string line = MenuHelper.GetInput($"{section.SectionType} rad");
                    if (string.IsNullOrWhiteSpace(line)) break;
                    newLines.Add(line);
                }
                section.Lines = newLines;
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Ogiltigt val.[/]");
                return;
            }

            SaveSongs();
            AnsiConsole.MarkupLine("[bold green]Lyrics uppdaterade![/]");
        }

        public void DeleteSongInteractive()
        {
            if (!Songs.Any())
            {
                AnsiConsole.MarkupLine("[red]Inga låtar att ta bort.[/]");
                return;
            }

            var titles = Songs.Select(s => s.Title).ToList();
            var songTitle = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("Välj låt att ta bort:")
                .AddChoices(titles));
            var song = Songs.FirstOrDefault(s => s.Title.Equals(songTitle, StringComparison.OrdinalIgnoreCase));
            if (song != null)
            {
                Songs.Remove(song);
                SaveSongs();
                AnsiConsole.MarkupLine($"[bold red]Låten \"{song.Title}\" borttagen och sparad![/]");
            }
            else
                AnsiConsole.MarkupLine("[red]Låt ej hittad.[/]");
        }
    }
}
