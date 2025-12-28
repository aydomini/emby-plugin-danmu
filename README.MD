# Emby 弹幕插件

为 Emby 提供弹幕支持的插件，支持多个主流弹幕平台。

## 🎯 支持的弹幕平台

- ✅ **Bilibili**（哔哩哔哩）
- ✅ **弹弹play**（需要配置 API_ID 和 API_SECRET）
- ✅ **优酷**
- ✅ **爱奇艺**
- ✅ **腾讯视频**
- ✅ **芒果TV**
- ✅ **DanmuApi**（自定义弹幕 API）

## 📦 安装方法

### 方式一：通过 GitHub Releases 安装（推荐）

1. 从 [Releases](https://github.com/aydomini/emby-plugin-danmu/releases) 下载最新版本的 `Emby.Plugin.Danmu.dll`
2. 将 DLL 文件复制到 Emby 插件目录：
   - **Windows**: `%AppData%\Emby-Server\plugins\`
   - **Linux**: `~/.config/emby-server/plugins/`
   - **macOS**: `~/.config/emby-server/plugins/`
3. 重启 Emby 服务器
4. 在 Emby 管理后台 → 插件 → 弹幕配置 中进行设置

### 方式二：自行编译

```bash
git clone https://github.com/aydomini/emby-plugin-danmu.git
cd emby-plugin-danmu
dotnet build Emby.Plugin.Danmu/Emby.Plugin.Danmu.csproj -c Release
```

编译后的 DLL 位于 `Emby.Plugin.Danmu/bin/Release/netstandard2.0/Emby.Plugin.Danmu.dll`

## ⚙️ 配置说明

### 弹弹play 配置（可选）

如需使用弹弹play作为弹幕源，需要配置 API 凭证：

1. 访问 [弹弹play官网](https://www.dandanplay.com/) 获取 `API_ID` 和 `API_SECRET`
2. 在 Emby 管理后台 → 插件 → 弹幕配置 中填写凭证

**注意**：不配置弹弹play不影响其他弹幕源的使用。

## 🔧 支持的 API 操作

### 刷新某个媒体的弹幕
```
GET /api/danmu/{媒体id}?option=Refresh&X-Emby-Token=xxx
```

### 获取媒体对应的 JSON 格式弹幕
```
GET /api/danmu/{媒体id}?option=GetJsonById&X-Emby-Token=xxx
```

## 📝 更新日志

### v1.2.6 (2025-12-28)
- 🚀 优化所有平台的标题匹配逻辑
- 降低相似度阈值：0.7 → 0.5，大幅提高匹配成功率
- 修复腾讯、优酷 API 过滤逻辑过于严格的问题

### v1.2.5 (2025-12-28)
- 🐛 修复 Bilibili 标题匹配问题
- 🐛 修复优酷电影 CommentId 为空问题
- 🐛 优化剧集匹配逻辑，添加剧集数为 0 的检查

### v1.2.4 (2025-12-28)
- 🐛 修复插件加载失败问题
- 移除 Plugin 构造函数中的 ToJson() 序列化调用
- 更新 Costura.Fody 配置

### v1.2.3 (2025-12-28)
- 🐛 修复 System.Memory 版本冲突

### v1.2.2 (2025-12-28)
- 🐛 修复依赖版本兼容性问题

### v1.2.1 (2025-12-28)
- 🐛 修复 DandanApi 类型错误

### 历史更新
- 2025-04-23：修复爱奇艺弹幕下载问题，支持更新和扫描任务
- 2024-07-26：支持弹幕搜索，支持新增媒体自动下载

## 🛠️ 技术栈

- .NET Standard 2.0
- Emby Server Core 4.8.5
- 支持 Emby 4.9.x

## 📚 参考

设置方式参考 [jellyfin-plugin-danmu](https://github.com/cxfksword/jellyfin-plugin-danmu)

## 🐛 问题反馈

遇到问题请在 [Issues](https://github.com/aydomini/emby-plugin-danmu/issues) 中反馈，并提供：
- Emby 服务器版本
- 插件版本
- 相关日志（位于 Emby 日志目录中包含 "danmu" 或 "弹幕" 的内容）

## ⚠️ 已知问题

1. **配置页面不支持顺序调整**：弹幕源的优先级顺序暂时无法通过配置页面调整
2. **部分影片平台无版权**：某些电影/剧集可能因平台版权限制无法获取弹幕

## 📄 许可证

本项目使用的许可证请参考原项目。

---

**原作者声明**：因为本人自己不再使用 EMBY，不同环境需自行调试和解决兼容问题。
