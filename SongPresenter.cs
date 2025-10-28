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
                    var style = section.SectionType switch
                    {
                        "Verse" => "[bold green]",
                        "Chorus" => "[bold blue]",
                        "Bridge" => "[bold magenta]",
                        _ => "[bold white]"
                    };
                    AnsiConsole.MarkupLine($"{style}{section.SectionType}:[/]");

                    foreach (var line in section.Lines)
                        AnsiConsole.MarkupLine($"  [white]{line}[/]");
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
                var style = section.SectionType switch
                {
                    "Verse" => "[bold green]",
                    "Chorus" => "[bold blue]",
                    "Bridge" => "[bold magenta]",
                    _ => "[bold white]"
                };
                AnsiConsole.MarkupLine($"{style}{section.SectionType}:[/]");

                foreach (var line in section.Lines)
                    AnsiConsole.MarkupLine($"  [white]{line}[/]");
            }
            AnsiConsole.MarkupLine($"[grey]{new string('─', 40)}[/]\n");
        }
    }
}
