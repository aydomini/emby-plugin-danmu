# Emby 弹幕插件故障排查指南

## 问题：插件列表不显示，但计划任务中能看到

### 🔍 问题诊断

**症状：**
- ✅ 计划任务中显示插件的计划
- ❌ 控制台 → 插件列表中看不到插件
- ❌ 配置页面无法访问

**原因：**
这说明插件被部分加载了（计划任务注册成功），但插件本身初始化失败。

---

## 🛠️ 解决方案

### 方案 1：检查 Emby 日志（最重要）

**查看日志文件位置：**

```bash
# Linux/Docker
/config/logs/embyserver.txt

# Windows
C:\ProgramData\Emby-Server\logs\embyserver.txt

# macOS
~/Library/Application Support/Emby-Server/logs/embyserver.txt
```

**查找关键错误：**

```bash
# 搜索插件相关错误
grep -i "danmu\|plugin" embyserver.txt

# 搜索加载错误
grep -i "error\|exception" embyserver.txt | grep -i danmu
```

**常见错误信息：**

1. **依赖缺失错误：**
   ```
   Could not load file or assembly 'System.Net.Http.Json, Version=8.0.0.0'
   ```
   **解决：** 使用修复后的版本（已降级到 5.0.0）

2. **API 配置错误：**
   ```
   弹弹接口缺少API_ID和API_SECRET
   ```
   **解决：** 这是正常的警告，可以忽略（如果不使用 Dandan 源）

3. **权限错误：**
   ```
   Access denied
   ```
   **解决：** 检查插件文件权限

---

### 方案 2：正确安装插件

**步骤：**

1. **停止 Emby Server**
   ```bash
   # Linux
   sudo systemctl stop emby-server

   # Docker
   docker stop emby

   # Windows - 在服务中停止
   ```

2. **清理旧版本**
   ```bash
   # 删除旧的插件文件
   rm /config/plugins/Emby.Plugin.Danmu.dll

   # 清理插件配置缓存（可选）
   rm /config/plugins/configurations/Emby.Plugin.Danmu.xml
   ```

3. **安装新版本**
   - 从 [GitHub Releases](https://github.com/aydomini/emby-plugin-danmu/releases) 下载最新的 `danmu_*.zip`
   - 解压 ZIP 文件，得到 `Emby.Plugin.Danmu.dll`
   - 复制到插件目录：
     ```bash
     # Linux/Docker
     cp Emby.Plugin.Danmu.dll /config/plugins/

     # Windows
     # 复制到 C:\ProgramData\Emby-Server\plugins\

     # macOS
     # 复制到 ~/Library/Application Support/Emby-Server/plugins/
     ```

4. **设置权限（Linux/Docker）**
   ```bash
   chmod 644 /config/plugins/Emby.Plugin.Danmu.dll
   chown emby:emby /config/plugins/Emby.Plugin.Danmu.dll
   ```

5. **启动 Emby Server**
   ```bash
   # Linux
   sudo systemctl start emby-server

   # Docker
   docker start emby
   ```

6. **验证安装**
   - 等待 30 秒让插件加载
   - 访问：控制台 → 插件
   - 应该看到 "Danmu" 插件

---

### 方案 3：版本兼容性检查

**检查 Emby Server 版本：**

```bash
# 在 Emby Web 界面查看
控制台 → 帮助 → 关于

# 或查看日志
grep "Emby Server" embyserver.txt
```

**最低要求：**
- Emby Server 版本：≥ 4.8.5
- .NET 运行时：.NET Core 3.1 或 .NET 6.0

**如果版本太旧：**
1. 升级 Emby Server
2. 或降级插件的 `MediaBrowser.Server.Core` 依赖版本

---

### 方案 4：手动构建插件

**如果从源码构建：**

```bash
# 1. 清理旧的构建
dotnet clean

# 2. 恢复依赖
dotnet restore

# 3. 构建 Release 版本
dotnet build -c Release

# 4. 找到生成的 DLL
ls -la Emby.Plugin.Danmu/bin/Release/netstandard2.0/Emby.Plugin.Danmu.dll

# 5. 复制到 Emby 插件目录
cp Emby.Plugin.Danmu/bin/Release/netstandard2.0/Emby.Plugin.Danmu.dll \
   /config/plugins/
```

---

### 方案 5：依赖打包问题

**检查是否使用了 Costura.Fody：**

项目使用 `Costura.Fody` 将依赖打包到单个 DLL。如果这个过程失败，可能导致运行时找不到依赖。

**验证打包：**

```bash
# 检查 DLL 大小（应该 > 1MB）
ls -lh Emby.Plugin.Danmu.dll

# 如果太小（< 500KB），说明依赖没打包进去
```

**解决方法：**

1. 确保 `Fody` 和 `Costura.Fody` 正确安装
2. 重新构建项目
3. 或者手动复制依赖 DLL 到插件目录

---

## 🧪 验证步骤

**完整的验证流程：**

1. **检查插件文件**
   ```bash
   ls -la /config/plugins/Emby.Plugin.Danmu.dll
   # 应该显示文件存在且大小 > 500KB
   ```

2. **检查日志**
   ```bash
   tail -f /config/logs/embyserver.txt | grep -i danmu
   # 重启 Emby，观察插件加载过程
   ```

3. **预期的日志输出**
   ```
   [INFO] Plugin "Danmu" loaded
   [INFO] danmu 插件加载完成, 支持7个
   ```

4. **访问配置页面**
   ```
   http://your-emby-server:8096/web/index.html#!/configurationpage?name=danmu
   ```

5. **测试功能**
   - 搜索弹幕
   - 下载弹幕
   - 查看计划任务

---

## 📞 仍然无法解决？

**提供以下信息以获取帮助：**

1. **Emby Server 版本**
   ```bash
   # 示例：Emby Server 4.8.5.0
   ```

2. **操作系统**
   ```bash
   # 示例：Ubuntu 22.04 / Docker / Windows 11
   ```

3. **插件文件信息**
   ```bash
   ls -la /config/plugins/Emby.Plugin.Danmu.dll
   md5sum /config/plugins/Emby.Plugin.Danmu.dll
   ```

4. **关键日志**
   ```bash
   grep -A 5 -B 5 "Danmu\|danmu" /config/logs/embyserver.txt
   ```

5. **错误截图**
   - 插件列表截图
   - 日志中的错误信息

---

## 🎯 快速诊断命令

```bash
#!/bin/bash
echo "=== Emby 弹幕插件诊断 ==="
echo ""
echo "1. 检查插件文件："
ls -lh /config/plugins/Emby.Plugin.Danmu.dll 2>/dev/null || echo "❌ 插件文件不存在"
echo ""
echo "2. 检查最近的错误："
tail -100 /config/logs/embyserver.txt | grep -i "error\|exception" | tail -5
echo ""
echo "3. 检查插件加载："
grep -i "danmu.*加载" /config/logs/embyserver.txt | tail -3
echo ""
echo "4. Emby 版本："
grep "Emby Server" /config/logs/embyserver.txt | head -1
```

**保存为 `diagnose.sh` 并运行：**
```bash
chmod +x diagnose.sh
./diagnose.sh
```

---

## ✅ 成功标志

**插件正常加载的标志：**
1. ✅ 控制台 → 插件 → 看到 "Danmu" 插件
2. ✅ 控制台 → 服务器 → 看到 "弹幕配置" 菜单
3. ✅ 日志中显示：`danmu 插件加载完成, 支持X个`
4. ✅ 可以搜索和下载弹幕
