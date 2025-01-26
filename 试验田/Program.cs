namespace 试验田;

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Win32;

public class Program
{
	static async Task Main(string[] args)
	{
		using (HttpClient client = new HttpClient())
		{
			// 请求的URL
			string url = "https://example.com/api/endpoint";

			// 要发送的JSON数据
			string jsonData = "{\"playerId\":\"104454918\",\"cardPoolId\":\"d2df87a27a8a868caa148c76395b6037\",\"cardPoolType\":1,\"serverId\":\"76402e5b20be2c39f095a152090afddc\",\"languageCode\":\"zh-Hans\",\"recordId\":\"a07c48782259c72890b6d03626b9286e\"}";

			// 创建请求内容
			StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

			try
			{
				// 发送POST请求
				HttpResponseMessage response = await client.PostAsync(url, content);

				// 确保请求成功
				response.EnsureSuccessStatusCode();

				// 读取响应内容
				string responseBody = await response.Content.ReadAsStringAsync();
				Console.WriteLine("响应内容: " + responseBody);
			}
			catch (HttpRequestException e)
			{
				// 捕获并处理请求异常
				Console.WriteLine("请求异常: " + e.Message);
			}
		}
	}
}
