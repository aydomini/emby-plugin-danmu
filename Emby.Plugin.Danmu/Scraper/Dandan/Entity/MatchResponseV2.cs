using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Emby.Plugin.Danmu.Scraper.Dandan.Entity
{
    [DataContract]
    public class MatchResponseV2
    {
        [DataMember(Name = "errorCode")]
        public int ErrorCode { get; set; }

        [DataMember(Name = "success")]
        public bool Success { get; set; }

        [DataMember(Name = "errorMessage")]
        public string ErrorMessage { get; set; }

        [DataMember(Name = "isMatched")]
        public bool IsMatched { get; set; }

        [DataMember(Name = "matches")]
        public List<MatchResultV2> Matches { get; set; }
    }
}
