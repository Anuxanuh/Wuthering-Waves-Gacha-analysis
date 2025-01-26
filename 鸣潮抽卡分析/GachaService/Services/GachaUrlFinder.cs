using Microsoft.Win32;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using 鸣潮抽卡分析.GachaService.Models;

namespace 鸣潮抽卡分析.GachaService.Services;

/// <summary>
/// 查找抽卡记录URL的类
/// </summary>
public class GachaUrlFinder
{
	// 用于匹配抽卡记录URL的正则表达式
	private static readonly Regex GachaUrlRegex = new Regex(
		@"https://aki-gm-resources(-oversea)?\.aki-game\.(net|com)/aki/gacha/index\.html#/record.*",
		RegexOptions.Compiled);

	// 用于匹配调试日志中抽卡记录URL的正则表达式
	private static readonly Regex DebugUrlRegex = new Regex(
		@"""#url"": ""(https://aki-gm-resources(-oversea)?\.aki-game\.(net|com)/aki/gacha/index\.html#/record[^""]*)""",
		RegexOptions.Compiled);

	/// <summary>
	/// 查找指定游戏路径中的抽卡记录URL
	/// </summary>
	/// <param name="gamePath">游戏路径</param>
	/// <returns>抽卡记录URL，如果未找到则返回null</returns>
	public string FindUrl(string gamePath)
	{
		// 检查游戏路径是否为空或不存在
		if (string.IsNullOrEmpty(gamePath) || !Directory.Exists(gamePath))
			return null;

		// 定义日志文件路径
		string[] logPaths = {
			Path.Combine(gamePath, @"Client\Saved\Logs\Client.log"),
			Path.Combine(gamePath, @"Client\Binaries\Win64\ThirdParty\KrPcSdk_Global\KRSDKRes\KRSDKWebView\debug.log")
		};

		// 遍历日志文件路径
		foreach (var logPath in logPaths)
		{
			// 如果日志文件不存在，继续下一个路径
			if (!File.Exists(logPath))
				continue;

			// 新增文件锁定处理逻辑
			try
			{
				// 尝试读取日志文件并解析URL
				return TryReadLogWithRetry(logPath);
			}
			catch (Exception ex)
			{
				// 捕获异常并输出错误信息
				Console.WriteLine($"无法读取日志文件 {logPath}: {ex.Message}");
			}
		}
		return null;
	}

	/// <summary>
	/// 尝试读取日志文件，支持重试机制
	/// </summary>
	/// <param name="logPath">日志文件路径</param>
	/// <param name="maxRetries">最大重试次数</param>
	/// <param name="delayMs">重试间隔时间（毫秒）</param>
	/// <returns>解析出的URL，如果未找到则返回null</returns>
	private string TryReadLogWithRetry(string logPath, int maxRetries = 3, int delayMs = 1000)
	{
		for (int attempt = 1; attempt <= maxRetries; attempt++)
		{
			try
			{
				// 使用FileShare.ReadWrite模式打开文件
				using (var fs = new FileStream(
					logPath,
					FileMode.Open,
					FileAccess.Read,
					FileShare.ReadWrite))
				{
					using (var sr = new StreamReader(fs))
					{
						// 读取文件内容
						var content = sr.ReadToEnd();
						// 解析文件内容并返回URL
						return ParseLogContent(logPath, content);
					}
				}
			}
			catch (IOException ex) when (IsFileLocked(ex))
			{
				// 如果文件被锁定，等待一段时间后重试
				if (attempt == maxRetries)
				{
					// 如果达到最大重试次数，抛出异常
					throw new IOException(
						$"文件 {logPath} 被其他进程锁定，请关闭游戏客户端后重试。" +
						$"（尝试次数：{maxRetries}）", ex);
				}
				Thread.Sleep(delayMs);
			}
		}
		return null;
	}

	/// <summary>
	/// 判断文件是否被锁定
	/// </summary>
	/// <param name="ex">IO异常</param>
	/// <returns>如果文件被锁定则返回true，否则返回false</returns>
	private bool IsFileLocked(IOException ex)
	{
		// 获取异常的错误代码
		var errorCode = Marshal.GetHRForException(ex) & 0xFFFF;
		// 错误代码32表示共享冲突，33表示锁定冲突
		return errorCode == 32 || errorCode == 33;
	}

	/// <summary>
	/// 解析日志文件内容，提取URL
	/// </summary>
	/// <param name="logPath">日志文件路径</param>
	/// <param name="content">日志文件内容</param>
	/// <returns>解析出的URL，如果未找到则返回null</returns>
	private string ParseLogContent(string logPath, string content)
	{
		// 根据日志文件类型选择对应的正则表达式
		var regex = logPath.EndsWith("Client.log") ? GachaUrlRegex : DebugUrlRegex;
		// 匹配日志内容
		var matches = regex.Matches(content);

		// 如果匹配失败，返回null
		if (matches.Count == 0)
			return null;

		// 返回最后一个匹配的URL
		var lastMatch = matches[matches.Count - 1];
		return lastMatch.Groups.Count > 1 && lastMatch.Groups[1].Success
		? lastMatch.Groups[1].Value.Trim()
		: lastMatch.Value.Trim();
	}
}
