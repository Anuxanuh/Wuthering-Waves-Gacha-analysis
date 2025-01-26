using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using 鸣潮抽卡分析.GachaService.Models;
using 鸣潮抽卡分析.GachaService.Utilities;

namespace 鸣潮抽卡分析.GachaService.Services;

public class GachaDataService
{
	private static readonly string _baseUrl = "https://gmserver-api.aki-game2.com/gacha/record/query";

	public static async Task<GachaApiResponse?> GetRecordsAsync(
		RequestParams parameters)
	{
		using (HttpClient client = new HttpClient())
		{
			// 要发送的JSON数据
			string jsonData = JsonSerializer.Serialize(parameters);

			// 创建请求内容
			StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
			try
			{
				// 发送POST请求
				HttpResponseMessage response = await client.PostAsync(_baseUrl, content);

				// 确保请求成功
				response.EnsureSuccessStatusCode();

				// 读取响应内容
				string responseBody = await response.Content.ReadAsStringAsync();

				// 将响应内容反序列化为GachaApiResponse对象
				GachaApiResponse? apiResponse = JsonSerializer.Deserialize<GachaApiResponse>(responseBody,
					new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
				return apiResponse;
			}
			catch (HttpRequestException e)
			{
				// 捕获并处理请求异常
				Console.WriteLine("请求异常: " + e.Message);
				return null;
			}
		}
	}
}
