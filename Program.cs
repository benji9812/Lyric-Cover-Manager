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
                MenuHelper.ShowMenu();
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        manager.AddSongInteractive();
                        break;
                    case "2":
                        SongPresenter.ShowSongs(manager.Songs);
                        break;
                    case "3":
                        string q = MenuHelper.GetInput("Sök artist/titel");
                        SongPresenter.ShowSongs(manager.Search(q));
                        break;
                    case "4":
                        string g = MenuHelper.GetInput("Genre att filtrera");
                        SongPresenter.ShowSongs(manager.FilterByGenre(g));
                        break;
                    case "5":
                        string status = MenuHelper.GetInput("Status att filtrera");
                        SongPresenter.ShowSongs(manager.FilterByStatus(status));
                        break;
                    case "6":
                        string lang = MenuHelper.GetInput("Språk att filtrera");
                        SongPresenter.ShowSongs(manager.Songs.Where(s => s.Language.Equals(lang, StringComparison.OrdinalIgnoreCase)));
                        break;
                    case "7":
                        SongPresenter.ShowSongs(manager.GetRecent());
                        break;
                    case "8":
                        manager.EditLyricsInteractive();
                        break;
                    case "0":
                        manager.SaveSongs();
                        Console.WriteLine("Data sparad. Avslutar...");
                        running = false;
                        break;
                }
            }
        }
    }
}