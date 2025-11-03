using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lyric_Cover_Manager
{
    public class SongSection
    {
        public string SectionType { get; set; }   // "Verse", "Chorus", "Bridge", etc.
        private List<string> _lines = new();

        public List<string> Lines
        {
            get => _lines;
            set => _lines = value.Select(l => System.Net.WebUtility.HtmlDecode(l)).ToList();
        }
    }

    public class Song
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Genre { get; set; }
        public int Year { get; set; }
        public string Language { get; set; }
        public List<SongSection> Lyrics { get; set; }  // Nu med sektioner!
        public string Notes { get; set; }
        public string Status { get; set; }
        public DateTime LastRehearsed { get; set; }
    }
}
