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
	private static readonly Regex GachaUrlRegex = new Regex(
		@"https://aki-gm-resources(-oversea)?\.aki-game\.(net|com)/aki/gacha/index\.html#/record.*",
		RegexOptions.Compiled);

	private static readonly Regex DebugUrlRegex = new Regex(
		@"""#url"": ""(https://aki-gm-resources(-oversea)?\.aki-game\.(net|com)/aki/gacha/index\.html#/record[^""]*)""",
		RegexOptions.Compiled);

	public string FindUrl(string gamePath)
	{
		if (string.IsNullOrEmpty(gamePath) || !Directory.Exists(gamePath))
			return null;

		string[] logPaths = {
			Path.Combine(gamePath, @"Client\Saved\Logs\Client.log"),
			Path.Combine(gamePath, @"Client\Binaries\Win64\ThirdParty\KrPcSdk_Global\KRSDKRes\KRSDKWebView\debug.log")
		};

		foreach (var logPath in logPaths)
		{
			if (!File.Exists(logPath))
				continue;

			// 新增文件锁定处理逻辑
			try
			{
				return TryReadLogWithRetry(logPath);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"无法读取日志文件 {logPath}: {ex.Message}");
			}
		}
		return null;
	}

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
						var content = sr.ReadToEnd();
						return ParseLogContent(logPath, content);
					}
				}
			}
			catch (IOException ex) when (IsFileLocked(ex))
			{
				if (attempt == maxRetries)
				{
					throw new IOException(
						$"文件 {logPath} 被其他进程锁定，请关闭游戏客户端后重试。" +
						$"（尝试次数：{maxRetries}）", ex);
				}
				Thread.Sleep(delayMs);
			}
		}
		return null;
	}

	private bool IsFileLocked(IOException ex)
	{
		var errorCode = Marshal.GetHRForException(ex) & 0xFFFF;
		return errorCode == 32 || errorCode == 33; // 32: Sharing violation, 33: Lock violation
	}

	private string ParseLogContent(string logPath, string content)
	{
		var regex = logPath.EndsWith("Client.log") ? GachaUrlRegex : DebugUrlRegex;
		var match = regex.Match(content);

		if (!match.Success)
			return null;

		return match.Groups.Count > 1 && match.Groups[1].Success
			? match.Groups[1].Value.Trim()
			: match.Value.Trim();
	}
}