using Microsoft.Win32;
using System.IO;
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
	private readonly GachaUrlResult _result = new GachaUrlResult(); // 查找结果实例
	private StringBuilder errorLog = new StringBuilder(); // 错误日志记录器
	private bool urlFound = false; // 是否找到URL的标志

	/// <summary>
	/// 尝试查找游戏安装路径并获取抽卡记录的URL
	/// </summary>
	/// <returns>包含查找结果的GachaUrlResult对象</returns>
	public GachaUrlResult FindUrl()
	{
		//Console.WriteLine("Attempting to find URL automatically...");

		CheckMuiCache(); // 检查MUI缓存
		if (!_result.Success)
			CheckFirewallRules(); // 检查防火墙规则
		if (!_result.Success)
			CheckNativeInstall(); // 检查本地安装路径
		if (!_result.Success)
			CheckCommonPaths(); // 检查常见路径

		//if (!_result.Success)
		//	ManualInput(); // 手动输入路径

		_result.ErrorLog = _result.ErrorLog.Trim();
		return _result;
	}

	/// <summary>
	/// 检查MUI缓存以查找游戏路径
	/// </summary>
	private void CheckMuiCache()
	{
		try
		{
			using (var key = Registry.CurrentUser.OpenSubKey(
				@"Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\MuiCache"))
			{
				if (key == null)
					return;

				foreach (var valueName in key.GetValueNames()
					.Where(n => n.Contains("client-win64-shipping.exe", StringComparison.OrdinalIgnoreCase)
							 && n.Contains("wuthering", StringComparison.OrdinalIgnoreCase)))
				{
					ProcessPath(valueName.Split(new[] { @"\client\" }, StringSplitOptions.None)[0]);
					if (_result.Success)
						return;
				}
			}
		}
		catch (Exception ex)
		{
			_result.ErrorLog += $"MUI Cache Error: {ex.Message}\n";
		}
	}

	/// <summary>
	/// 检查防火墙规则以查找游戏路径
	/// </summary>
	private void CheckFirewallRules()
	{
		try
		{
			using (RegistryKey key = Registry.LocalMachine.OpenSubKey(
				@"SYSTEM\CurrentControlSet\Services\SharedAccess\Parameters\FirewallPolicy\FirewallRules"))
			{
				if (key == null)
					return;

				foreach (string value in key.GetValueNames()
					.Where(n => n.Contains("client-win64-shipping") && key.GetValue(n).ToString().Contains("wuthering")))
				{
					string valueData = key.GetValue(value).ToString();
					string path = valueData.Split(new[] { "App=" }, StringSplitOptions.None)[1]
						.Split(new[] { @"\client\" }, StringSplitOptions.None)[0];
					ProcessPath(path);
					if (urlFound)
						return;
				}
			}
		}
		catch (SecurityException ex)
		{
			errorLog.AppendLine("Permission denied accessing Firewall Rules");
		}
	}

	/// <summary>
	/// 检查本地安装路径以查找游戏路径
	/// </summary>
	private void CheckNativeInstall()
	{
		CheckRegistryPath(Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"));
		CheckRegistryPath(Registry.LocalMachine.OpenSubKey(
			@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"));
	}

	/// <summary>
	/// 检查注册表路径以查找游戏路径
	/// </summary>
	/// <param name="baseKey">注册表基键</param>
	private void CheckRegistryPath(RegistryKey baseKey)
	{
		if (baseKey == null)
			return;

		foreach (string subKeyName in baseKey.GetSubKeyNames())
		{
			using (RegistryKey subKey = baseKey.OpenSubKey(subKeyName))
			{
				object displayName = subKey?.GetValue("DisplayName");
				if (displayName?.ToString().ToLower().Contains("wuthering") == true)
				{
					ProcessPath(subKey.GetValue("InstallPath")?.ToString());
					if (urlFound)
						return;
				}
			}
		}
	}

	/// <summary>
	/// 检查常见路径以查找游戏路径
	/// </summary>
	private void CheckCommonPaths()
	{
		DriveInfo.GetDrives()
			.Where(d => d.DriveType == DriveType.Fixed)
			.SelectMany(d => new[]
			{
				$@"{d.Name}\Wuthering Waves Game",
				$@"{d.Name}\Wuthering Waves\Wuthering Waves Game",
				$@"{d.Name}\Program Files\Epic Games\WutheringWavesj3oFh",
				$@"{d.Name}\Program Files\Epic Games\WutheringWavesj3oFh\Wuthering Waves Game"
			})
			.ToList()
			.ForEach(ProcessPath);
	}

	/// <summary>
	/// 提示用户手动输入游戏路径
	/// </summary>
	private void ManualInput()
	{
		Console.WriteLine("\nGame install location not found or log files missing.");
		Console.WriteLine("Common install locations:");
		Console.WriteLine("  C:\\Wuthering Waves");
		Console.WriteLine("  C:\\Wuthering Waves\\Wuthering Waves Game");

		while (!_result.Success)
		{
			Console.Write("\nEnter game path (or 'exit' to quit): ");
			var path = Console.ReadLine()?.Trim();

			if ("exit".Equals(path, StringComparison.OrdinalIgnoreCase))
				break;
			if (!string.IsNullOrEmpty(path))
				ProcessPath(path);
		}
	}

	/// <summary>
	/// 处理给定路径以查找日志文件
	/// </summary>
	/// <param name="path">游戏安装路径</param>
	private void ProcessPath(string path)
	{
		if (string.IsNullOrEmpty(path) ||
			path.Contains("OneDrive", StringComparison.OrdinalIgnoreCase) ||
			_result.CheckedPaths.Contains(path))
			return;

		_result.CheckedPaths.Add(path);
		Console.WriteLine($"Checking path: {path}");

		var logPaths = new[]
		{
			Path.Combine(path, @"Client\Saved\Logs\Client.log"),
			Path.Combine(path, @"Client\Binaries\Win64\ThirdParty\KrPcSdk_Global\KRSDKRes\KRSDKWebView\debug.log")
		};

		foreach (var logPath in logPaths)
		{
			if (TryFindUrlInLog(logPath))
			{
				_result.Success = true;
				return;
			}
		}

		_result.ErrorLog += $"No valid logs found at: {path}\n";
	}

	/// <summary>
	/// 尝试在日志文件中查找抽卡记录的URL
	/// </summary>
	/// <param name="filePath">日志文件路径</param>
	/// <returns>是否找到URL</returns>
	private bool TryFindUrlInLog(string filePath)
	{
		if (!File.Exists(filePath))
			return false;

		try
		{
			var content = File.ReadAllText(filePath);
			var match = Regex.Match(content,
				@"(https://aki-gm-resources(-oversea)?\.aki-game\.(net|com)/aki/gacha/index\.html#/record[^\s""]*)",
				RegexOptions.IgnoreCase | RegexOptions.RightToLeft);

			if (!match.Success)
				return false;

			_result.Url = match.Groups[1].Value;
			Console.WriteLine($"Found URL in {Path.GetFileName(filePath)}");
			return true;
		}
		catch (Exception ex)
		{
			_result.ErrorLog += $"Error reading {filePath}: {ex.Message}\n";
			return false;
		}
	}
}
