using Microsoft.Win32;
using System.IO;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using Ãù³±³é¿¨·ÖÎö.Models;

namespace Ãù³±³é¿¨·ÖÎö.Services;

/// <summary>
/// ²éÕÒÓÎÏ·Ä¿Â¼µÄÀà
/// </summary>
public class GamePathFinder
{
	private readonly List<string> _checkedPaths = new List<string>();
	public string LastError { get; private set; } = string.Empty;

	public string FindGamePath()
	{
		string path = CheckMuiCache()
				   ?? CheckFirewallRules()
				   ?? CheckNativeInstallation()
				   ?? CheckCommonPaths();
		return path;
	}

	private string CheckMuiCache()
	{
		try
		{
			using var key = Registry.CurrentUser.OpenSubKey(
				@"Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\MuiCache");

			foreach (var valueName in key.GetValueNames())
			{
				if (valueName.Contains("client-win64-shipping.exe") &&
					key.GetValue(valueName) is string value &&
					value.Contains("wuthering"))
				{
					var path = value.Split(new[] { @"\client\" }, StringSplitOptions.None)[0];
					if (ValidatePath(path))
						return path;
				}
			}
		}
		catch (Exception ex)
		{
			LastError += $"MUI Cache Error: {ex.Message}\n";
		}
		return null;
	}

	private string CheckFirewallRules()
	{
		try
		{
			using var key = Registry.LocalMachine.OpenSubKey(
				@"SYSTEM\CurrentControlSet\Services\SharedAccess\Parameters\FirewallPolicy\FirewallRules");

			foreach (var valueName in key.GetValueNames())
			{
				if (valueName.Contains("client-win64-shipping") &&
					key.GetValue(valueName) is string value &&
					value.Contains("wuthering"))
				{
					var parts = value.Split(new[] { "App=" }, StringSplitOptions.None);
					if (parts.Length > 1)
					{
						var path = parts[1].Split(new[] { @"\client\" }, StringSplitOptions.None)[0];
						if (ValidatePath(path))
							return path;
					}
				}
			}
		}
		catch (Exception ex)
		{
			LastError += $"Firewall Rules Error: {ex.Message}\n";
		}
		return null;
	}

	private string CheckNativeInstallation()
	{
		string[] registryPaths = {
			@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
			@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
		};

		foreach (var regPath in registryPaths)
		{
			using var key = Registry.LocalMachine.OpenSubKey(regPath);
			foreach (var subKeyName in key.GetSubKeyNames())
			{
				using var subKey = key.OpenSubKey(subKeyName);
				if (subKey.GetValue("DisplayName") is string displayName &&
					displayName.Contains("wuthering") &&
					subKey.GetValue("InstallPath") is string path)
				{
					if (ValidatePath(path))
						return path;
				}
			}
		}
		return null;
	}

	private string CheckCommonPaths()
	{
		foreach (var drive in DriveInfo.GetDrives())
		{
			string[] paths = {
				$@"{drive.Name}Wuthering Waves Game",
				$@"{drive.Name}Wuthering Waves\Wuthering Waves Game",
				$@"{drive.Name}Program Files\Epic Games\WutheringWavesj3oFh",
				$@"{drive.Name}Program Files\Epic Games\WutheringWavesj3oFh\Wuthering Waves Game"
			};

			foreach (var path in paths)
			{
				if (Directory.Exists(path) && ValidatePath(path))
				{
					return path;
				}
			}
		}
		return null;
	}

	private bool ValidatePath(string path)
	{
		if (string.IsNullOrEmpty(path) ||
			path.Contains("OneDrive") ||
			_checkedPaths.Contains(path))
			return false;

		_checkedPaths.Add(path);
		return Directory.Exists(path);
	}
}