using System;
using System.Linq;
using Spectre.Console;

namespace Lyric_Cover_Manager
{
    internal class Program
    {
        static SongManager manager = new SongManager();

        static void Main()
        {
            bool running = true;
            while (running)
            {
                var choice = MenuHelper.ShowMenu();

                switch (choice)
                {
                    case "1":
                        manager.AddSongInteractive();
                        MenuHelper.Pause();
                        break;
                    case "2":
                        SongPresenter.ShowSongs(manager.Songs);
                        MenuHelper.Pause();
                        break;
                    case "3":
                        string q = MenuHelper.GetInput("Sök artist/titel");
                        SongPresenter.ShowSongs(manager.Search(q));
                        MenuHelper.Pause();
                        break;
                    case "4":
                        string g = MenuHelper.GetInput("Genre att filtrera");
                        SongPresenter.ShowSongs(manager.FilterByGenre(g));
                        MenuHelper.Pause();
                        break;
                    case "5":
                        string status = MenuHelper.GetInput("Status att filtrera");
                        SongPresenter.ShowSongs(manager.FilterByStatus(status));
                        MenuHelper.Pause();
                        break;
                    case "6":
                        string lang = MenuHelper.GetInput("Språk att filtrera");
                        SongPresenter.ShowSongs(manager.Songs.Where(s => s.Language.Equals(lang, StringComparison.OrdinalIgnoreCase)));
                        MenuHelper.Pause();
                        break;
                    case "7":
                        SongPresenter.ShowSongs(manager.GetRecent());
                        MenuHelper.Pause();
                        break;
                    case "8":
                        manager.EditLyricsInteractive();
                        MenuHelper.Pause();
                        break;
                    case "9":
                        manager.DeleteSongInteractive(); // Nytt: ta bort låt
                        MenuHelper.Pause();
                        break;
                    case "0":
                        manager.SaveSongs();
                        AnsiConsole.MarkupLine("[bold green]Data sparad. Avslutar...[/]");
                        running = false;
                        MenuHelper.Pause();
                        break;
                    default:
                        AnsiConsole.MarkupLine("[red]Ogiltigt menyval![/]");
                        MenuHelper.Pause();
                        break;
                }

                // Separera varje menyval snyggt
                if (running) AnsiConsole.MarkupLine("\n[grey]===================================[/]\n");
            }
        }
    }
}