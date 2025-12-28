using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Emby.Plugin.Danmu.Scraper.Bilibili.Entity
{
    /// <summary>
    /// B站合集信息
    /// </summary>
    [DataContract]
    public class VideoUgcSeason
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "cover")]
        public string Cover { get; set; }

        [DataMember(Name = "mid")]
        public long Mid { get; set; }

        [DataMember(Name = "intro")]
        public string Intro { get; set; }

        [DataMember(Name = "sections")]
        public List<VideoUgcSection> Sections { get; set; }
    }

    /// <summary>
    /// 合集分区
    /// </summary>
    [DataContract]
    public class VideoUgcSection
    {
        [DataMember(Name = "season_id")]
        public long SeasonId { get; set; }

        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "type")]
        public int Type { get; set; }

        [DataMember(Name = "episodes")]
        public List<VideoUgcEpisode> Episodes { get; set; }
    }

    /// <summary>
    /// 合集剧集
    /// </summary>
    [DataContract]
    public class VideoUgcEpisode
    {
        [DataMember(Name = "season_id")]
        public long SeasonId { get; set; }

        [DataMember(Name = "section_id")]
        public long SectionId { get; set; }

        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "aid")]
        public long Aid { get; set; }

        [DataMember(Name = "cid")]
        public long CId { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "attribute")]
        public int Attribute { get; set; }

        [DataMember(Name = "arc")]
        public VideoUgcArc Arc { get; set; }

        [DataMember(Name = "page")]
        public VideoUgcPage Page { get; set; }

        [DataMember(Name = "bvid")]
        public string Bvid { get; set; }
    }

    /// <summary>
    /// 合集稿件信息
    /// </summary>
    [DataContract]
    public class VideoUgcArc
    {
        [DataMember(Name = "aid")]
        public long Aid { get; set; }

        [DataMember(Name = "videos")]
        public int Videos { get; set; }

        [DataMember(Name = "type_id")]
        public int TypeId { get; set; }

        [DataMember(Name = "type_name")]
        public string TypeName { get; set; }

        [DataMember(Name = "copyright")]
        public int Copyright { get; set; }

        [DataMember(Name = "pic")]
        public string Pic { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "pubdate")]
        public long Pubdate { get; set; }

        [DataMember(Name = "ctime")]
        public long Ctime { get; set; }

        [DataMember(Name = "desc")]
        public string Desc { get; set; }

        [DataMember(Name = "duration")]
        public int Duration { get; set; }
    }

    /// <summary>
    /// 合集分P信息
    /// </summary>
    [DataContract]
    public class VideoUgcPage
    {
        [DataMember(Name = "cid")]
        public long Cid { get; set; }

        [DataMember(Name = "page")]
        public int Page { get; set; }

        [DataMember(Name = "from")]
        public string From { get; set; }

        [DataMember(Name = "part")]
        public string Part { get; set; }

        [DataMember(Name = "duration")]
        public int Duration { get; set; }

        [DataMember(Name = "dimension")]
        public object Dimension { get; set; }
    }
}
