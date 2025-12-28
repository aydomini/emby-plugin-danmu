using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Emby.Plugin.Danmu.Scraper.DanmuApi.Entity
{
    [DataContract]
    public class CommentResponse
    {
        [DataMember(Name = "count")]
        public int Count { get; set; }

        [DataMember(Name = "comments")]
        public List<Comment> Comments { get; set; } = new List<Comment>();
    }

    [DataContract]
    public class Comment
    {
        [DataMember(Name = "cid")]
        public long Cid { get; set; }

        [DataMember(Name = "p")]
        public string P { get; set; } = string.Empty;

        [DataMember(Name = "m")]
        public string M { get; set; } = string.Empty;
    }
}
