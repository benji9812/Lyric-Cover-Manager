using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console;

namespace Lyric_Cover_Manager
{
    public static class SongPresenter
    {
        public static void ShowSongs(IEnumerable<Song> songsList)
        {
            foreach (var s in songsList)
            {
                // Snygg färg och markering
                AnsiConsole.MarkupLine($"[bold underline yellow]Titel:[/] {s.Title}  [italic cyan]({s.Artist})[/]");
                AnsiConsole.MarkupLine($"Genre: [green]{s.Genre}[/]  Språk: [blue]{s.Language}[/]  År: [purple]{s.Year}[/]");
                AnsiConsole.MarkupLine($"Status: [bold magenta]{s.Status}[/]  Senast övad: [grey]{s.LastRehearsed:d}[/]");
                AnsiConsole.MarkupLine($"Anteckningar: [italic]{s.Notes}[/]");

                AnsiConsole.MarkupLine("[bold]Lyrics:[/]");
                foreach (var section in s.Lyrics)
                {
                    Style sectionStyle = GetSectionStyle(section.SectionType);
                    Text styledText = new Text($"{section.SectionType}:\n", sectionStyle);
                    AnsiConsole.Write(styledText);

                    foreach (var line in section.Lines)
                        AnsiConsole.Write(new Text($"  {line}\n", new Style(Color.White)));
                }
                AnsiConsole.MarkupLine($"[grey]{new string('─', 40)}[/]\n");
            }
        }

        public static void ShowSingleSong(Song s)
        {
            AnsiConsole.MarkupLine($"[bold underline yellow]Titel:[/] {s.Title}  [italic cyan]({s.Artist})[/]");
            AnsiConsole.MarkupLine($"Genre: [green]{s.Genre}[/]  Språk: [blue]{s.Language}[/]  År: [purple]{s.Year}[/]");
            AnsiConsole.MarkupLine($"Status: [bold magenta]{s.Status}[/]  Senast övad: [grey]{s.LastRehearsed:d}[/]");
            AnsiConsole.MarkupLine($"Anteckningar: [italic]{s.Notes}[/]");
            AnsiConsole.MarkupLine("[bold]Lyrics:[/]");
            foreach (var section in s.Lyrics)
            {
                Style sectionStyle = GetSectionStyle(section.SectionType);
                Text styledText = new Text($"{section.SectionType}:\n", sectionStyle);
                AnsiConsole.Write(styledText);

                foreach (var line in section.Lines)
                    AnsiConsole.Write(new Text($"  {line}\n", new Style(Color.White)));
            }
            AnsiConsole.MarkupLine($"[grey]{new string('─', 40)}[/]\n");
        }

        public static void ShowSection(SongSection section)
        {
            Style sectionStyle = GetSectionStyle(section.SectionType);
            Text styledText = new Text($"\n{section.SectionType}:\n", sectionStyle);
            AnsiConsole.Write(styledText);
            
            foreach (var line in section.Lines)
                AnsiConsole.Write(new Text($"  {line}\n", new Style(Color.White)));
        }

        public static Style GetSectionStyle(string sectionType)
        {
            return sectionType.ToLower() switch
            {
                string s when s.Contains("verse") => new Style(Color.Green, decoration: Decoration.Bold),
                string s when s.Contains("chorus") => new Style(Color.Blue, decoration: Decoration.Bold),
                string s when s.Contains("bridge") => new Style(new Color(255, 0, 255), decoration: Decoration.Bold),
                string s when s.Contains("intro") => new Style(Color.Yellow, decoration: Decoration.Bold),
                string s when s.Contains("outro") => new Style(new Color(0, 255, 255), decoration: Decoration.Bold),
                string s when s.Contains("instrumental") || s.Contains("solo") => new Style(Color.White, decoration: Decoration.Dim | Decoration.Italic),
                _ => new Style(Color.White, decoration: Decoration.Bold)
            };
        }

        public static void ShowSongWithSections(Song song)
        {
            AnsiConsole.MarkupLine($"[bold yellow]Titel:[/] [bold lime]{song.Title}[/] [grey]({song.Artist})[/]");
            AnsiConsole.MarkupLine($"[yellow]Genre:[/] [green]{song.Genre}[/] [yellow]Språk:[/] [blue]{song.Language}[/] [yellow]År:[/] [fuchsia]{song.Year}[/]");
            AnsiConsole.MarkupLine($"[green]Status:[/] [magenta]{song.Status}[/] [green]Senast övad:[/] [yellow]{song.LastRehearsed:yyyy-MM-dd}[/]");
            AnsiConsole.MarkupLine($"[green]Anteckningar:[/] [lime]{song.Notes}[/]");
            AnsiConsole.MarkupLine("[bold yellow]Lyrics:[/]");

            foreach (var section in song.Lyrics)
                ShowSection(section);
        }
    }
}