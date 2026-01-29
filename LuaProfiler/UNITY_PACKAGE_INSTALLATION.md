# Unity Package 模式下的 CSV 导出功能使用说明

## 重要提示 ⚠️

由于这个 Lua Profiler 是通过 GitHub URL 作为 Unity Package 安装的，位于 `PackageCache` 目录下，有以下几点需要注意：

## 生效方式

### 方式一：重新导入包（推荐）

1. **刷新 Package Manager**
   - 打开 Unity 编辑器
   - 打开 Package Manager (Window > Package Manager)
   - 找到 "Miku Lua Profiler" 包
   - 点击包，选择 "Remove"（移除）
   - 重新通过 Git URL 添加：`https://github.com/[user]/com.miku.luaprofiler.git#4cbf1bbd22`
   - Unity 会重新下载并编译

### 方式二：强制重新编译

1. **触发重新编译**
   - 关闭 Unity 编辑器
   - 删除 `Library/ScriptAssemblies` 目录
   - 重新打开 Unity，会自动重新编译所有脚本

2. **或者手动刷新**
   - 在 Unity 编辑器中按 `Ctrl+R` (Windows) 或 `Cmd+R` (Mac) 强制刷新资源

### 方式三：从 PackageCache 复制到 Assets（开发模式）

如果你需要频繁修改代码：

1. **复制整个包到 Assets**
   ```
   从：Client\ClientProject\Library\PackageCache\com.miku.luaprofiler@4cbf1bbd22
   到：Client\ClientProject\Assets\ThirdParty\MikuLuaProfiler
   ```

2. **修改 manifest.json**
   - 打开 `Packages/manifest.json`
   - 移除或注释掉 Git URL 的引用
   - Unity 会使用 Assets 中的本地版本

## 验证是否生效

1. **检查编译错误**
   - 打开 Unity Console 窗口
   - 确认没有编译错误

2. **检查按钮是否出现**
   - 打开 Lua Profiler 窗口
   - 查看工具栏是否有 "Export CSV" 按钮（在 Save/Load 按钮旁边）

3. **测试导出功能**
   - 点击 "Export CSV" 按钮
   - 应该弹出文件保存对话框

## 当前状态分析

### PackageCache 特性

`PackageCache` 目录的特点：
- ✅ Unity 会自动编译其中的代码
- ✅ 修改后需要 Unity 重新识别
- ⚠️ Unity 关闭时可能被清理
- ⚠️ 不建议直接在此修改代码

### 推荐的开发流程

如果你是包的开发者或需要频繁修改：

1. **Fork 或克隆原仓库**
   ```bash
   git clone https://github.com/[original]/com.miku.luaprofiler.git
   cd com.miku.luaprofiler
   ```

2. **应用 CSV 导出功能的修改**
   - 复制新增的文件到本地仓库
   - 提交修改

3. **使用本地包**
   在 `Packages/manifest.json` 中使用本地路径：
   ```json
   {
     "dependencies": {
       "com.miku.luaprofiler": "file:../path/to/local/com.miku.luaprofiler"
     }
   }
   ```

## 快速验证步骤

### 步骤 1：确认文件已修改

检查以下文件是否包含新代码：

1. **LuaCsvExporter.cs**
   ```
   位置：Editor/Window/ProfilerWin/TreeView/LuaCsvExporter.cs
   检查：文件是否存在且包含 ExportToCSV 方法
   ```

2. **LuaProfilerTreeView.cs**
   ```
   位置：Editor/Window/ProfilerWin/TreeView/LuaProfilerTreeView.cs
   检查：搜索 "ExportToCSV" 方法是否存在
   ```

3. **LuaProfilerWindow.cs**
   ```
   位置：Editor/Window/ProfilerWin/TreeView/LuaProfilerWindow.cs
   检查：搜索 "Export CSV" 按钮代码
   ```

### 步骤 2：强制重新编译

在 Unity 中执行：
1. 菜单：Assets > Refresh
2. 或按快捷键 Ctrl+R (Windows) / Cmd+R (Mac)

### 步骤 3：检查控制台

查看 Unity Console：
- 如果有红色编译错误，需要先解决
- 常见问题：文件编码、命名空间等

## 常见问题排查

### Q1: 修改后看不到 "Export CSV" 按钮

**可能原因：**
1. Unity 未重新编译代码
2. 编辑器缓存问题
3. 代码有编译错误

**解决方法：**
```
1. 关闭 Unity
2. 删除以下目录：
   - Library/ScriptAssemblies
   - Library/ScriptAssemblies.meta
3. 重新打开 Unity
4. 等待自动重新编译完成
```

### Q2: PackageCache 中的修改丢失

**原因：**
Unity 可能会重新下载包并覆盖 PackageCache

**解决方法：**
- 使用本地包模式（见上文）
- 或将包复制到 Assets 目录

### Q3: 编译错误

**检查：**
1. 打开 Console 查看具体错误信息
2. 确认所有文件编码为 UTF-8
3. 确认文件结尾有换行符

## Unity Package 开发最佳实践

### 本地开发模式（推荐）

```json
// Packages/manifest.json
{
  "dependencies": {
    "com.miku.luaprofiler": "file:../../LocalPackages/com.miku.luaprofiler"
  }
}
```

### Git 依赖模式

```json
// Packages/manifest.json
{
  "dependencies": {
    "com.miku.luaprofiler": "https://github.com/user/com.miku.luaprofiler.git#branch-or-tag"
  }
}
```

## 如何应用这些修改到你的项目

### 选项 A：等待包更新（如果有权限）

1. 将修改提交到 Git 仓库
2. 在 Unity 中更新包版本

### 选项 B：使用本地包（推荐）

1. **创建本地包副本**
   ```bash
   mkdir -p Client/ClientProject/LocalPackages
   cp -r Client/ClientProject/Library/PackageCache/com.miku.luaprofiler@4cbf1bbd22 \
        Client/ClientProject/LocalPackages/com.miku.luaprofiler
   ```

2. **修改 manifest.json**
   ```json
   {
     "dependencies": {
       "com.miku.luaprofiler": "file:../LocalPackages/com.miku.luaprofiler"
     }
   }
   ```

3. **应用新功能文件**
   将以下文件复制到本地包：
   - `Editor/Window/ProfilerWin/TreeView/LuaCsvExporter.cs`
   - `Editor/Window/ProfilerWin/TreeView/LuaCsvExporter.cs.meta`
   - 修改后的 `LuaProfilerTreeView.cs`
   - 修改后的 `LuaProfilerWindow.cs`

4. **刷新 Unity**
   - Assets > Refresh 或重启 Unity

### 选项 C：直接复制到 Assets

```
从：Library/PackageCache/com.miku.luaprofiler@4cbf1bbd22
到：Assets/Plugins/MikuLuaProfiler
```

然后从 manifest.json 中移除包引用。

## 验证清单

- [ ] 文件已正确放置在对应目录
- [ ] Unity Console 无编译错误
- [ ] 打开 Lua Profiler 窗口
- [ ] 工具栏有 "Export CSV" 按钮
- [ ] 点击按钮弹出文件保存对话框
- [ ] 成功导出 CSV 文件
- [ ] 可以用 Excel 打开 CSV 文件

## 技术支持

如果按照上述步骤仍无法生效：

1. **提供以下信息：**
   - Unity 版本
   - Console 中的错误信息
   - 包的安装方式（Git URL 或本地）
   - manifest.json 内容

2. **临时解决方案：**
   使用原有的 Save 功能保存 .sample 文件，然后编写转换脚本

---

**更新时间**: 2026-01-29  
**适用于**: Unity Package Manager 包模式
