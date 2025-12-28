using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Emby.Plugin.Danmu.Scraper.DanmuApi.Entity
{
    [DataContract]
    public class SearchResponse
    {
        [DataMember(Name = "errorCode")]
        public int ErrorCode { get; set; }

        [DataMember(Name = "success")]
        public bool Success { get; set; }

        [DataMember(Name = "errorMessage")]
        public string ErrorMessage { get; set; }

        [DataMember(Name = "animes")]
        public List<Anime> Animes { get; set; } = new List<Anime>();
    }

    [DataContract]
    public class Anime
    {
        private static readonly Regex YearRegex = new Regex(@"\((\d{4})\)", RegexOptions.Compiled);
        private static readonly Regex FromRegex = new Regex(@"from\s+(\w+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        [DataMember(Name = "animeId")]
        public long AnimeId { get; set; }

        [DataMember(Name = "bangumiId")]
        public string BangumiId { get; set; } = string.Empty;

        [DataMember(Name = "animeTitle")]
        public string AnimeTitle { get; set; } = string.Empty;

        [DataMember(Name = "type")]
        public string Type { get; set; } = string.Empty;

        [DataMember(Name = "typeDescription")]
        public string TypeDescription { get; set; } = string.Empty;

        [DataMember(Name = "imageUrl")]
        public string ImageUrl { get; set; } = string.Empty;

        [DataMember(Name = "episodeCount")]
        public int EpisodeCount { get; set; }

        [DataMember(Name = "source")]
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// 从 AnimeTitle 中解析年份，格式如：火影忍者疾风传剧场版：羁绊(2008)【电影】from tencent
        /// </summary>
        public int? Year
        {
            get
            {
                if (string.IsNullOrEmpty(AnimeTitle))
                {
                    return null;
                }

                var match = YearRegex.Match(AnimeTitle);
                if (match.Success && match.Groups.Count > 1)
                {
                    if (int.TryParse(match.Groups[1].Value, out var year))
                    {
                        // 验证年份在合理范围内（1900-2100）
                        if (year >= 1900 && year <= 2100)
                        {
                            return year;
                        }
                    }
                }

                return null;
            }
        }

    }
}
