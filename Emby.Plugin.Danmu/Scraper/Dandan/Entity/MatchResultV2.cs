using System.Runtime.Serialization;

namespace Emby.Plugin.Danmu.Scraper.Dandan.Entity
{
    [DataContract]
    public class MatchResultV2
    {
        [DataMember(Name = "episodeId")]
        public long EpisodeId { get; set; }

        [DataMember(Name = "animeId")]
        public long AnimeId { get; set; }

        [DataMember(Name = "animeTitle")]
        public string AnimeTitle { get; set; }

        [DataMember(Name = "episodeTitle")]
        public string EpisodeTitle { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "typeDescription")]
        public string TypeDescription { get; set; }

        [DataMember(Name = "shift")]
        public int Shift { get; set; }
    }
}
