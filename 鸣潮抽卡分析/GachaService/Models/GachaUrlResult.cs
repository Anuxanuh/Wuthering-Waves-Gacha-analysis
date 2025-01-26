using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 鸣潮抽卡分析.GachaService.Models;

/// <summary>
/// 表示查找结果的类
/// </summary>
public class GachaUrlResult
{
	public bool Success { get; set; } // 查找是否成功
	public string Url { get; set; } // 找到的URL
	public string ErrorLog { get; set; } // 错误日志
	public List<string> CheckedPaths { get; set; } // 已检查的路径列表

	public GachaUrlResult()
	{
		CheckedPaths = new List<string>();
		ErrorLog = "";
	}
}
