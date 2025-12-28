using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Caching.Memory;
using Emby.Plugin.Danmu.Scraper.DanmuApi.Entity;
using Emby.Plugin.Danmu.Configuration;
using MediaBrowser.Common.Net;
using MediaBrowser.Model.Logging;
using Emby.Plugin.Danmu.Core.Extensions;
using Emby.Plugin.Danmu.Core.Singleton;

namespace Emby.Plugin.Danmu.Scraper.DanmuApi
{
    public class DanmuApiApi : AbstractApi
    {
        private static readonly object _lock = new object();
        private DateTime lastRequestTime = DateTime.Now.AddDays(-1);

        public DanmuApiOption Config
        {
            get
            {
                return Plugin.Instance?.Configuration.DanmuApi ?? new DanmuApiOption();
            }
        }

        public string ServerUrl
        {
            get
            {
                var serverUrl = Config.ServerUrl?.Trim();
                if (string.IsNullOrEmpty(serverUrl))
                {
                    // 尝试从环境变量获取
                    serverUrl = Environment.GetEnvironmentVariable("DANMU_API_SERVER_URL");
                    if (string.IsNullOrEmpty(serverUrl))
                    {
                        return string.Empty;
                    }
                }

                // 移除末尾的 /
                return serverUrl.TrimEnd('/');
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DanmuApiApi"/> class.
        /// </summary>
        /// <param name="logManager">The <see cref="ILogManager"/>.</param>
        public DanmuApiApi(ILogManager logManager, IHttpClient httpClient)
            : base(logManager.getDefaultLogger("DanmuApiApi"), httpClient)
        {
        }

        /// <summary>
        /// 搜索动漫
        /// GET /api/v2/search/anime?keyword={keyword}
        /// </summary>
        public async Task<List<Anime>> SearchAsync(string keyword, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(keyword) || string.IsNullOrEmpty(ServerUrl))
            {
                return new List<Anime>();
            }

            var cacheKey = $"danmuapi_search_{keyword}";
            var expiredOption = new MemoryCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) };
            if (_memoryCache.TryGetValue<List<Anime>>(cacheKey, out var searchResult))
            {
                return searchResult;
            }

            keyword = HttpUtility.UrlEncode(keyword);
            var url = $"{ServerUrl}/api/v2/search/anime?keyword={keyword}";

            try
            {
                var httpRequestOptions = GetDefaultHttpRequestOptions(url, null, cancellationToken);
                var response = await httpClient.GetResponse(httpRequestOptions).ConfigureAwait(false);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    _logger.Info("DanmuApi 搜索失败: {0}, 状态码: {1}", keyword, response.StatusCode);
                    return new List<Anime>();
                }

                var result = SingletonManager.JsonSerializer.DeserializeFromStream<SearchResponse>(response.Content);
                if (result != null && result.Success && result.Animes != null)
                {
                    _memoryCache.Set(cacheKey, result.Animes, expiredOption);
                    return result.Animes;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("DanmuApi 搜索失败: {0}", keyword);
            }

            var emptyList = new List<Anime>();
            _memoryCache.Set(cacheKey, emptyList, expiredOption);
            return emptyList;
        }

        /// <summary>
        /// 获取番剧详情和剧集列表
        /// GET /api/v2/bangumi/{id}
        /// </summary>
        public async Task<Bangumi> GetBangumiAsync(string bangumiId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(bangumiId) || string.IsNullOrEmpty(ServerUrl))
            {
                return null;
            }

            var cacheKey = $"danmuapi_bangumi_{bangumiId}";
            var expiredOption = new MemoryCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) };
            if (_memoryCache.TryGetValue<Bangumi>(cacheKey, out var bangumi))
            {
                return bangumi;
            }

            var url = $"{ServerUrl}/api/v2/bangumi/{bangumiId}";

            try
            {
                var httpRequestOptions = GetDefaultHttpRequestOptions(url, null, cancellationToken);
                var response = await httpClient.GetResponse(httpRequestOptions).ConfigureAwait(false);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    _logger.Info("DanmuApi 获取番剧详情失败: {0}, 状态码: {1}", bangumiId, response.StatusCode);
                    return null;
                }

                var result = SingletonManager.JsonSerializer.DeserializeFromStream<BangumiResponse>(response.Content);
                if (result != null && result.Success && result.Bangumi != null)
                {
                    _memoryCache.Set(cacheKey, result.Bangumi, expiredOption);
                    return result.Bangumi;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("DanmuApi 获取番剧详情失败: {0}", bangumiId);
            }

            _memoryCache.Set<Bangumi>(cacheKey, null, expiredOption);
            return null;
        }

        /// <summary>
        /// 获取弹幕内容
        /// GET /api/v2/comment/{id}
        /// </summary>
        public async Task<List<Comment>> GetCommentsAsync(string commentId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(commentId) || string.IsNullOrEmpty(ServerUrl))
            {
                return new List<Comment>();
            }

            await this.LimitRequestFrequently();

            var url = $"{ServerUrl}/api/v2/comment/{commentId}";

            const int maxRetries = 3;
            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                try
                {
                    var httpRequestOptions = GetDefaultHttpRequestOptions(url, null, cancellationToken);
                    var response = await httpClient.GetResponse(httpRequestOptions).ConfigureAwait(false);

                    if (response.StatusCode == (HttpStatusCode)429) // TooManyRequests
                    {
                        if (attempt < maxRetries - 1)
                        {
                            _logger.Warn("DanmuApi 获取弹幕遇到429限流,等待31秒后重试 (尝试 {0}/{1}): {2}", attempt + 1, maxRetries, commentId);
                            await Task.Delay(TimeSpan.FromSeconds(31), cancellationToken);
                            continue;
                        }
                        else
                        {
                            _logger.Error("DanmuApi 获取弹幕遇到429限流,已达到最大重试次数: {0}", commentId);
                            break;
                        }
                    }

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        _logger.Info("DanmuApi 获取弹幕失败: {0}, 状态码: {1}", commentId, response.StatusCode);
                        break;
                    }

                    var result = SingletonManager.JsonSerializer.DeserializeFromStream<CommentResponse>(response.Content);
                    if (result != null && result.Comments != null)
                    {
                        return result.Comments;
                    }
                    break;
                }
                catch (Exception ex)
                {
                    _logger.Error("DanmuApi 获取弹幕失败: {0}", commentId);
                    break;
                }
            }

            return new List<Comment>();
        }

        protected async Task LimitRequestFrequently()
        {
            // 每分钟最多12次请求，约5秒一次
            var diff = 0;
            lock (_lock)
            {
                var ts = DateTime.Now - lastRequestTime;
                diff = (int)(5000 - ts.TotalMilliseconds);
                lastRequestTime = DateTime.Now;
            }

            if (diff > 0)
            {
                this._logger.Debug("请求太频繁，等待{0}毫秒后继续执行...", diff);
                await Task.Delay(diff).ConfigureAwait(false);
            }
        }
    }
}
