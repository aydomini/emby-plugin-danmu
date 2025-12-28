using System;
using System.Collections.Generic;

namespace Danmaku2Ass
{
    /// <summary>
    /// 过滤器基类
    /// </summary>
    public class Filter
    {
        public virtual List<Danmaku> DoFilter(List<Danmaku> danmakus)
        {
            throw new NotImplementedException("使用了过滤器的未实现的方法。");
        }
    }

    /// <summary>
    /// 顶部样式过滤器
    /// </summary>
    public class TopFilter : Filter
    {
        public override List<Danmaku> DoFilter(List<Danmaku> danmakus)
        {
            List<Danmaku> keep = new List<Danmaku>();
            foreach (var danmaku in danmakus)
            {
                if (danmaku.Style == "top")
                {
                    continue;
                }
                keep.Add(danmaku);
            }
            return keep;
        }
    }

    /// <summary>
    /// 底部样式过滤器
    /// </summary>
    public class BottomFilter : Filter
    {
        public override List<Danmaku> DoFilter(List<Danmaku> danmakus)
        {
            List<Danmaku> keep = new List<Danmaku>();
            foreach (var danmaku in danmakus)
            {
                if (danmaku.Style == "bottom")
                {
                    continue;
                }
                keep.Add(danmaku);
            }
            return keep;
        }
    }

    /// <summary>
    /// 滚动样式过滤器
    /// </summary>
    public class ScrollFilter : Filter
    {
        public override List<Danmaku> DoFilter(List<Danmaku> danmakus)
        {
            List<Danmaku> keep = new List<Danmaku>();
            foreach (var danmaku in danmakus)
            {
                if (danmaku.Style == "scroll")
                {
                    continue;
                }
                keep.Add(danmaku);
            }
            return keep;
        }
    }

    /// <summary>
    /// 自定义过滤器
    /// </summary>
    public class CustomFilter : Filter
    {
        public override List<Danmaku> DoFilter(List<Danmaku> danmakus)
        {
            // TODO
            return base.DoFilter(danmakus);
        }
    }

    /// <summary>
    /// Emoji 表情过滤器
    /// </summary>
    public class EmojiFilter : Filter
    {
        // 常见Emoji Unicode范围
        private static readonly System.Text.RegularExpressions.Regex EmojiRegex = new System.Text.RegularExpressions.Regex(
            @"[\u00A0-\u00FF]|[\u2000-\u3300]|[\u{1F000}-\u{1F9FF}]|[\u{1FA00}-\u{1FAFF}]",
            System.Text.RegularExpressions.RegexOptions.Compiled);

        public override List<Danmaku> DoFilter(List<Danmaku> danmakus)
        {
            List<Danmaku> keep = new List<Danmaku>();
            foreach (var danmaku in danmakus)
            {
                if (!string.IsNullOrEmpty(danmaku.Text))
                {
                    danmaku.Text = EmojiRegex.Replace(danmaku.Text, "");
                }
                keep.Add(danmaku);
            }
            return keep;
        }
    }
}
