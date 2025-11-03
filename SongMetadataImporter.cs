using System.Net;
using Newtonsoft.Json.Linq;

public static class SongMetadataImporter
{
    public static (string, int) FetchMetadata(string artist, string title)
    {
        string url = $"https://itunes.apple.com/search?term={Uri.EscapeDataString(artist + " " + title)}&entity=song&limit=1";
        using (var client = new WebClient())
        {
            try
            {
                string response = client.DownloadString(url);
                JObject obj = JObject.Parse(response);
                var result = obj["results"]?.FirstOrDefault();
                if (result != null)
                {
                    string genre = result.Value<string>("primaryGenreName") ?? "Okänd";
                    string releaseDate = result.Value<string>("releaseDate") ?? "";
                    int year = 0;
                    if (!string.IsNullOrEmpty(releaseDate) && DateTime.TryParse(releaseDate, out DateTime dt))
                        year = dt.Year;
                    return (genre, year);
                }
                return ("Okänd", 0);
            }
            catch
            {
                return ("Okänd", 0);
            }
        }
    }
}
