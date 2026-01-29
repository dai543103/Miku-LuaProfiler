# Lua Profiler CSV 导出功能使用说明

## 功能概述

本功能为 MikuLuaProfiler 添加了 CSV 导出能力，可以将性能分析数据导出为标准 CSV 格式文件，便于在 Excel 或其他数据分析工具中进行深入分析。

## 主要特性

### ✅ 完整的堆栈信息
- 导出完整的函数调用链路径（如：`Root>Update>LuaFunc`）
- 层级深度标识，清晰展现调用关系
- 支持 Lua 和 C# 混合调用栈

### ✅ 丰富的性能指标
包含以下14个性能指标列：

| 指标 | 说明 | 单位 |
|-----|------|-----|
| Full Stack | 完整堆栈路径 | 文本 |
| Depth | 调用深度 | 整数 |
| Function Name | 函数名称 | 文本 |
| File Path | 文件路径 | 文本 |
| Line | 行号 | 整数 |
| Is Lua | 是否为Lua函数 | true/false |
| Calls | 调用次数 | 整数 |
| Total Time(ms) | 总耗时 | 毫秒(3位小数) |
| Self Time(ms) | 自身耗时 | 毫秒(3位小数) |
| Avg Time(ms) | 平均耗时 | 毫秒(3位小数) |
| Total Lua GC(B) | 累计Lua GC | 字节 |
| Self Lua GC(B) | 自身Lua GC | 字节 |
| Total Mono GC(B) | 累计Mono GC | 字节 |
| Self Mono GC(B) | 自身Mono GC | 字节 |

### ✅ Excel 友好
- 使用 UTF-8 with BOM 编码，中文完美显示
- 符合 RFC 4180 CSV 标准
- 特殊字符正确转义（逗号、引号、换行等）

## 使用方法

### 1. 打开 Lua Profiler 窗口
在 Unity 编辑器中打开 Lua Profiler 窗口（通常通过菜单或快捷方式）

### 2. 运行游戏并收集数据
- 点击 "Record" 开始记录性能数据
- 运行游戏场景，执行需要分析的操作
- 停止录制

### 3. 导出 CSV
1. 在 Profiler 窗口工具栏找到 **"Export CSV"** 按钮（位于 Save 和 Load 按钮旁边）
2. 点击按钮
3. 在弹出的保存对话框中选择保存位置和文件名
4. 点击"保存"

### 4. 查看结果
- 成功导出后会显示确认对话框，包含文件路径和导出的数据行数
- 使用 Excel、Numbers 或其他电子表格软件打开 CSV 文件

## CSV 文件示例

```csv
Full Stack,Depth,Function Name,File Path,Line,Is Lua,Calls,Total Time(ms),Self Time(ms),Avg Time(ms),Total Lua GC(B),Self Lua GC(B),Total Mono GC(B),Self Mono GC(B)
Root,0,Root,,,false,1,5420.150,125.340,5420.150,2048000,51200,4096000,102400
Root>GameLoop.Update,1,GameLoop.Update,GameLoop.cs,45,false,180,5294.810,89.450,29.415,1996800,38400,3993600,76800
Root>GameLoop.Update>LuaUpdate,2,[lua]: game_main.lua&line:123,game_main.lua,123,true,180,5205.360,2845.120,28.918,1958400,1228800,3916800,2457600
Root>GameLoop.Update>LuaUpdate>ProcessEvent,3,[lua]: event_mgr.lua&line:56,event_mgr.lua,56,true,540,2360.240,1680.180,4.370,729600,524288,1459200,1048576
```

## 数据分析技巧

### Excel 中的常见分析

#### 1. 按耗时排序
- 选中 Total Time(ms) 列
- 点击"数据" > "排序"，选择降序
- 快速找到性能热点

#### 2. 筛选 Lua 函数
- 选中表头行
- 点击"数据" > "筛选"
- 在 Is Lua 列选择 "true"
- 只显示 Lua 相关函数

#### 3. 按堆栈路径分组
- 使用"Full Stack"列进行透视表分析
- 统计特定调用路径的总耗时和调用次数

#### 4. 计算占比
在新列中使用公式：
```
=H2/SUM($H$2:$H$100)*100
```
计算每个函数占总时间的百分比

#### 5. 查找GC热点
- 按 Self Lua GC(B) 或 Self Mono GC(B) 降序排序
- 找出导致内存分配的热点函数

### 高级用法

#### 数据透视表
1. 选中所有数据
2. 插入 > 数据透视表
3. 行字段：Full Stack
4. 值字段：Total Time(ms)、Calls
5. 可视化调用关系和性能分布

#### 条件格式
- 对 Total Time(ms) 列应用色阶条件格式
- 直观显示性能热点

## 技术细节

### 实现文件

1. **LuaCsvExporter.cs** - CSV导出核心类
   - 位置：`Editor/Window/ProfilerWin/TreeView/LuaCsvExporter.cs`
   - 功能：递归遍历树结构，格式化并写入CSV

2. **LuaProfilerTreeView.cs** - 添加导出方法
   - 位置：`Editor/Window/ProfilerWin/TreeView/LuaProfilerTreeView.cs`
   - 新增方法：`ExportToCSV()`

3. **LuaProfilerWindow.cs** - 添加UI按钮
   - 位置：`Editor/Window/ProfilerWin/TreeView/LuaProfilerWindow.cs`
   - UI位置：工具栏，Save/Load按钮之后

### 数据处理

#### 时间转换
- 内部存储：微秒 (microseconds)
- CSV导出：毫秒 (milliseconds)，保留3位小数
- 转换公式：`milliseconds = microseconds / 1000.0`

#### 堆栈路径构建
- 分隔符：`>`
- 示例：`Root>Update>LuaFunc>SubFunc`
- 递归构建完整路径

#### CSV 特殊字符处理
按照 RFC 4180 标准：
- 包含逗号、引号、换行的字段用双引号包围
- 字段内的双引号转义为两个双引号 `""`
- 示例：`"He said, ""Hello"""`

## 常见问题

### Q: 导出的文件为空或数据很少？
A: 确保在导出前已经运行游戏并记录了性能数据。如果在非录制模式下导出，只会导出当前显示的聚合数据。

### Q: Excel 中文显示乱码？
A: 文件使用 UTF-8 with BOM 编码，正常情况下 Excel 可以自动识别。如果仍有问题：
1. 用记事本打开CSV文件
2. 另存为，编码选择 "UTF-8"
3. 重新用 Excel 打开

### Q: 数据如何对应到源代码？
A: 
- 对于 Lua 函数：查看 "File Path" 和 "Line" 列
- 对于 C# 函数：查看 "Function Name" 列的方法名
- 使用 "Full Stack" 列追踪完整调用链

### Q: 如何导出历史数据？
A: 当前版本导出的是聚合后的根节点数据（`roots`）。如需导出历史帧数据，需要修改代码以支持逐帧导出。

### Q: 可以自定义导出的列吗？
A: 当前版本包含固定的14列。如需自定义，可以修改 `LuaCsvExporter.cs` 中的 `WriteHeader()` 和 `WriteDataRow()` 方法。

## 性能考虑

- **大数据集**：数百个节点可以快速导出（通常<1秒）
- **特大数据集**：数千个节点可能需要几秒钟
- **内存优化**：使用 StreamWriter 流式写入，不在内存中累积全部内容

## 未来扩展

可能的增强功能：
- 支持导出历史数据（逐帧）
- 支持自定义导出列
- 支持过滤和排序后导出
- 支持 JSON/XML 等其他格式
- 自动生成性能报告（HTML）

## 反馈与支持

如有问题或建议，请在项目中提交 Issue。

---

**版本**: 1.0.0  
**创建日期**: 2026-01-29  
**兼容性**: Unity 5.6 或更高版本
