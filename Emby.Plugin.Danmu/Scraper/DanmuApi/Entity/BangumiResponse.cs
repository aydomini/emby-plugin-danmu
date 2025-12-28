using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Emby.Plugin.Danmu.Scraper.DanmuApi.Entity
{
    [DataContract]
    public class BangumiResponse
    {
        [DataMember(Name = "errorCode")]
        public int ErrorCode { get; set; }

        [DataMember(Name = "success")]
        public bool Success { get; set; }

        [DataMember(Name = "errorMessage")]
        public string ErrorMessage { get; set; }

        [DataMember(Name = "bangumi")]
        public Bangumi Bangumi { get; set; }
    }

    [DataContract]
    public class Bangumi
    {
        [DataMember(Name = "animeId")]
        public long AnimeId { get; set; }

        [DataMember(Name = "bangumiId")]
        public string BangumiId { get; set; } = string.Empty;

        [DataMember(Name = "animeTitle")]
        public string AnimeTitle { get; set; } = string.Empty;

        [DataMember(Name = "episodes")]
        public List<Episode> Episodes { get; set; } = new List<Episode>();

        /// <summary>
        /// 按平台分组的剧集列表，保持原始顺序
        /// </summary>
        public List<EpisodeGroup> EpisodeGroups
        {
            get
            {
                var groups = new List<EpisodeGroup>();
                var currentPlatform = string.Empty;
                var currentEpisodes = new List<Episode>();

                foreach (var episode in Episodes)
                {
                    var platform = episode.Platform ?? string.Empty;

                    if (platform != currentPlatform)
                    {
                        if (currentEpisodes.Count > 0)
                        {
                            groups.Add(new EpisodeGroup
                            {
                                Platform = currentPlatform,
                                Episodes = currentEpisodes
                            });
                        }

                        currentPlatform = platform;
                        currentEpisodes = new List<Episode>();
                    }

                    currentEpisodes.Add(episode);
                }

                // 添加最后一组
                if (currentEpisodes.Count > 0)
                {
                    groups.Add(new EpisodeGroup
                    {
                        Platform = currentPlatform,
                        Episodes = currentEpisodes
                    });
                }

                return groups;
            }
        }
    }

    public class EpisodeGroup
    {
        public string Platform { get; set; } = string.Empty;
        public List<Episode> Episodes { get; set; } = new List<Episode>();
    }

    [DataContract]
    public class Episode
    {
        private static readonly Regex PlatformRegex = new Regex(@"【(.+?)】", RegexOptions.Compiled);

        [DataMember(Name = "seasonId")]
        public string SeasonId { get; set; } = string.Empty;

        [DataMember(Name = "episodeId")]
        public string EpisodeId { get; set; } = string.Empty;

        [DataMember(Name = "episodeTitle")]
        public string EpisodeTitle { get; set; } = string.Empty;

        [DataMember(Name = "episodeNumber")]
        public string EpisodeNumber { get; set; } = string.Empty;

        /// <summary>
        /// 从 EpisodeTitle 中解析平台标识，格式如：【qq】 第1集
        /// </summary>
        public string Platform
        {
            get
            {
                if (string.IsNullOrEmpty(EpisodeTitle))
                {
                    return null;
                }

                var match = PlatformRegex.Match(EpisodeTitle);
                if (match.Success && match.Groups.Count > 1)
                {
                    return match.Groups[1].Value.Trim();
                }

                return null;
            }
        }
    }
}
