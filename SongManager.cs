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
        public List<Song> Songs { get; private set; }

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
            File.WriteAllText(FilePath, json);
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
            var song = new Song
            {
                Title = MenuHelper.GetInput("Titel"),
                Artist = MenuHelper.GetInput("Artist"),
                Genre = MenuHelper.GetInput("Genre"),
                Language = MenuHelper.GetInput("Språk"),
                Notes = MenuHelper.GetInput("Anteckningar"),
                Status = MenuHelper.GetInput("Status"),
                LastRehearsed = DateTime.Now,
                Year = int.TryParse(MenuHelper.GetInput("År"), out int year) ? year : 0,
                Lyrics = new List<SongSection>()
            };

            AnsiConsole.MarkupLine("[bold underline yellow]Lägg till låtsektioner![/]");
            bool addSection = true;
            while (addSection)
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

                var more = MenuHelper.GetInput("Lägg till ytterligare sektion? (j/n)").Trim().ToLower();
                addSection = (more == "j" || more == "ja");
            }

            Songs.Add(song);
            SaveSongs();
            AnsiConsole.MarkupLine("[bold green]Låt tillagd![/]");
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
                AnsiConsole.MarkupLine($"{i}. [bold]{section.SectionType}[/]");
                foreach (var line in section.Lines)
                    AnsiConsole.MarkupLine($"   [white]{line}[/]");
                i++;
            }

            string editSec = MenuHelper.GetInput("Nummer på sektion att ändra (eller 'ny' för ny sektion)");
            if (editSec.ToLower() == "ny")
            {
                // Lägg till ny sektion
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

    }
}
