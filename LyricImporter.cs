using HtmlAgilityPack;
using Lyric_Cover_Manager;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;

public static class GeniusImporter
{
    public static string GeniusToken = "ZBT1C0fuwSuBIKIDFpNtE-B2lhwYvRt_I5dOh-rME6EnHXoM6bkkiF_90pN2u0d1";

    public static string SearchSong(string artist, string title)
    {
        string url = $"https://api.genius.com/search?q={Uri.EscapeDataString(artist + " " + title)}";
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {GeniusToken}");
            var resp = client.GetStringAsync(url).Result;
            JObject obj = JObject.Parse(resp);
            var hit = obj["response"]?["hits"]?.FirstOrDefault();
            string songUrl = hit?["result"]?["url"]?.ToString();
            return songUrl ?? "";
        }
    }

    public static List<SongSection> FetchSectionsFromGeniusPage(string url)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (compatible; LyricCoverManager/1.0)");
        var html = httpClient.GetStringAsync(url).Result;

        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var lyricsNodes = doc.DocumentNode.SelectNodes("//div[@data-lyrics-container='true']");
        var sections = new List<SongSection>();

        if (lyricsNodes != null)
        {
            var allLines = new List<string>();
            
            foreach (var node in lyricsNodes)
            {
                // Hämta InnerHtml för att behålla <br> taggar
                string innerHtml = node.InnerHtml;
                
                // Ersätt <br> med radbrytningar
                innerHtml = Regex.Replace(innerHtml, @"<br\s*/?>\s*", "\n", RegexOptions.IgnoreCase);
                
                // Ta bort alla HTML-taggar
                string plainText = Regex.Replace(innerHtml, @"<[^>]+>", "");
                
                // HTML-avkoda (konverterar &#x27; till ', &quot; till ", etc)
                plainText = WebUtility.HtmlDecode(plainText);
                
                // Split på radbrytningar och rensa
                var lines = plainText
                    .Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(line => line.Trim())
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .Where(line =>
                        !line.ToLower().Contains("read more") &&
                        !line.ToLower().Contains("embed") &&
                        !line.ToLower().Contains("contributor") &&
                        !line.ToLower().Contains("genius"))
                    .ToList();

                allLines.AddRange(lines);
            }

            // Dela i sektioner
            SongSection current = null;
            foreach (var line in allLines)
            {
                // Matcha [Verse 1], [Chorus], [Bridge], etc
                if (Regex.IsMatch(line, @"^\[?(Verse|Chorus|Refrain|Bridge|Intro|Outro|Instrumental)[\s\d:]*\]?$", RegexOptions.IgnoreCase))
                {
                    if (current != null && current.Lines.Any())
                        sections.Add(current);

                    // Rensa taggar om de finns
                    string sectionType = Regex.Replace(line, @"[\[\]]", "").Trim(':');
                    current = new SongSection
                    {
                        SectionType = sectionType,
                        Lines = new List<string>()
                    };
                }
                else
                {
                    if (current == null)
                    {
                        current = new SongSection
                        {
                            SectionType = $"Verse {sections.Count + 1}",
                            Lines = new List<string>()
                        };
                    }
                    current.Lines.Add(line);
                }
            }
            
            if (current != null && current.Lines.Any())
                sections.Add(current);
        }
        return sections;
    }
}
