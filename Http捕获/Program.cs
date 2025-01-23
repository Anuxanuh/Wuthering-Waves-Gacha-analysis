using System.IO;
using System.Net;
using System.Text;
using System;

namespace Http捕获;

using Microsoft.Win32;
using System;

class Program
{
	static void Main(string[] args)
	{
		string programName = "Wuthering Waves"; // 替换为你要查找的程序名称
		string installPath = GetProgramInstallPath(programName);

		if (installPath != null)
		{
			Console.WriteLine($"安装目录: {installPath}");
		}
		else
		{
			Console.WriteLine("未找到指定程序的安装目录。");
		}
	}

	static string GetProgramInstallPath(string programName)
	{
		string registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
		using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKey))
		{
			if (key != null)
			{
				foreach (string subKeyName in key.GetSubKeyNames())
				{
					using (RegistryKey subKey = key.OpenSubKey(subKeyName))
					{
						if (subKey != null)
						{
							object displayName = subKey.GetValue("DisplayName");
							object installLocation = subKey.GetValue("InstallLocation");

							if (displayName != null && installLocation != null && displayName.ToString().Contains(programName))
							{
								return installLocation.ToString();
							}
						}
					}
				}
			}
		}

		return null;
	}
}
