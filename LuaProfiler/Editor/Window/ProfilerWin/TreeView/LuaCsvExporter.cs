/*
               #########                       
              ############                     
              #############                    
             ##  ###########                   
            ###  ###### #####                  
            ### #######   ####                 
           ###  ########## ####                
          ####  ########### ####               
         ####   ###########  #####             
        #####   ### ########   #####           
       #####   ###   ########   ######         
      ######   ###  ###########   ######       
     ######   #### ##############  ######      
    #######  #####################  ######     
    #######  ######################  ######    
   #######  ###### #################  ######   
   #######  ###### ###### #########   ######   
   #######    ##  ######   ######     ######   
   #######        ######    #####     #####    
    ######        #####     #####     ####     
     #####        ####      #####     ###      
      #####       ###        ###      #        
        ###       ###        ###               
         ##       ###        ###               
__________#_______####_______####______________
                我们的未来没有BUG              
* ==============================================================================
* Filename: LuaCsvExporter
* Created:  2026/01/29
* Author:   Kilo Code
* Purpose:  CSV export utility for Lua Profiler data with detailed stack information
* ==============================================================================
*/

#if UNITY_5_6_OR_NEWER && UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace MikuLuaProfiler
{
    /// <summary>
    /// CSV导出工具类，负责将性能分析数据导出为CSV格式
    /// </summary>
    public static class LuaCsvExporter
    {
        /// <summary>
        /// 导出TreeViewItem数据到CSV文件
        /// </summary>
        /// <param name="filePath">CSV文件保存路径</param>
        /// <param name="roots">根节点列表</param>
        public static void ExportToCSV(string filePath, List<LuaProfilerTreeViewItem> roots)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
            }

            if (roots == null || roots.Count == 0)
            {
                throw new ArgumentException("No data to export", nameof(roots));
            }

            // 使用UTF-8 with BOM编码，确保Excel正确识别
            using (StreamWriter writer = new StreamWriter(filePath, false, new UTF8Encoding(true)))
            {
                // 写入CSV头部
                WriteHeader(writer);

                // 递归遍历所有节点并写入数据
                foreach (var root in roots)
                {
                    WriteTreeItemRecursive(writer, root, "", 0);
                }
            }
        }

        /// <summary>
        /// 写入CSV文件头部
        /// </summary>
        private static void WriteHeader(StreamWriter writer)
        {
            StringBuilder header = new StringBuilder();
            
            header.Append("Full Stack,");
            header.Append("Depth,");
            header.Append("Function Name,");
            header.Append("File Path,");
            header.Append("Line,");
            header.Append("Is Lua,");
            header.Append("Calls,");
            header.Append("Total Time(ms),");
            header.Append("Self Time(ms),");
            header.Append("Avg Time(ms),");
            header.Append("Total Lua GC(B),");
            header.Append("Self Lua GC(B),");
            header.Append("Total Mono GC(B),");
            header.Append("Self Mono GC(B)");

            writer.WriteLine(header.ToString());
        }

        /// <summary>
        /// 递归遍历树结构并写入CSV行
        /// </summary>
        /// <param name="writer">StreamWriter对象</param>
        /// <param name="item">当前TreeViewItem</param>
        /// <param name="parentPath">父路径</param>
        /// <param name="depth">当前深度</param>
        private static void WriteTreeItemRecursive(
            StreamWriter writer, 
            LuaProfilerTreeViewItem item, 
            string parentPath, 
            int depth)
        {
            if (item == null) return;

            // 构建完整堆栈路径
            string fullStack = BuildStackPath(parentPath, item.displayName);

            // 写入当前节点数据
            WriteDataRow(writer, fullStack, depth, item);

            // 递归处理子节点
            if (item.childs != null && item.childs.Count > 0)
            {
                foreach (var child in item.childs)
                {
                    WriteTreeItemRecursive(writer, child, fullStack, depth + 1);
                }
            }
        }

        /// <summary>
        /// 构建完整堆栈路径
        /// </summary>
        /// <param name="parentPath">父路径</param>
        /// <param name="currentName">当前节点名称</param>
        /// <returns>完整堆栈路径</returns>
        private static string BuildStackPath(string parentPath, string currentName)
        {
            if (string.IsNullOrEmpty(parentPath))
            {
                return currentName;
            }
            return parentPath + ">" + currentName;
        }

        /// <summary>
        /// 写入单行数据
        /// </summary>
        /// <param name="writer">StreamWriter对象</param>
        /// <param name="fullStack">完整堆栈路径</param>
        /// <param name="depth">深度</param>
        /// <param name="item">TreeViewItem数据</param>
        private static void WriteDataRow(
            StreamWriter writer,
            string fullStack,
            int depth,
            LuaProfilerTreeViewItem item)
        {
            StringBuilder row = new StringBuilder();

            // Full Stack - 完整堆栈路径
            row.Append(EscapeCsvField(fullStack));
            row.Append(",");

            // Depth - 深度
            row.Append(depth);
            row.Append(",");

            // Function Name - 函数名称
            row.Append(EscapeCsvField(item.displayName));
            row.Append(",");

            // File Path - 文件路径
            row.Append(EscapeCsvField(item.filePath ?? ""));
            row.Append(",");

            // Line - 行号
            row.Append(item.line);
            row.Append(",");

            // Is Lua - 是否为Lua函数
            row.Append(item.isLua ? "true" : "false");
            row.Append(",");

            // Calls - 调用次数
            row.Append(item.totalCallTime);
            row.Append(",");

            // Total Time(ms) - 总耗时(毫秒)
            row.Append(FormatTime(item.totalTime));
            row.Append(",");

            // Self Time(ms) - 自身耗时(毫秒)
            row.Append(FormatTime(item.selfCostTime));
            row.Append(",");

            // Avg Time(ms) - 平均耗时(毫秒)
            row.Append(FormatTime(item.averageTime));
            row.Append(",");

            // Total Lua GC(B) - 累计Lua GC
            row.Append(item.totalLuaMemory);
            row.Append(",");

            // Self Lua GC(B) - 自身Lua GC
            row.Append(item.selfLuaMemory);
            row.Append(",");

            // Total Mono GC(B) - 累计Mono GC
            row.Append(item.totalMonoMemory);
            row.Append(",");

            // Self Mono GC(B) - 自身Mono GC
            row.Append(item.selfMonoMemory);

            writer.WriteLine(row.ToString());
        }

        /// <summary>
        /// 转义CSV字段 - 处理特殊字符
        /// RFC 4180标准：
        /// 1. 如果字段包含逗号、双引号或换行符，需要用双引号包围
        /// 2. 字段内的双引号需要转义为两个双引号
        /// </summary>
        /// <param name="field">原始字段</param>
        /// <returns>转义后的字段</returns>
        private static string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
            {
                return "";
            }

            // 检查是否需要转义
            bool needsEscape = field.Contains(",") || 
                              field.Contains("\"") || 
                              field.Contains("\n") || 
                              field.Contains("\r");

            if (!needsEscape)
            {
                return field;
            }

            // 转义双引号
            string escaped = field.Replace("\"", "\"\"");
            
            // 用双引号包围
            return "\"" + escaped + "\"";
        }

        /// <summary>
        /// 格式化时间值 - 微秒转毫秒，保留3位小数
        /// </summary>
        /// <param name="microseconds">微秒数</param>
        /// <returns>格式化后的毫秒数字符串</returns>
        private static string FormatTime(long microseconds)
        {
            double milliseconds = microseconds / 1000.0;
            return milliseconds.ToString("F3");
        }

        /// <summary>
        /// 格式化内存值 - 字节数
        /// </summary>
        /// <param name="bytes">字节数</param>
        /// <returns>格式化后的字节数字符串</returns>
        private static string FormatMemory(long bytes)
        {
            return bytes.ToString();
        }
    }
}
#endif
