using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using 鸣潮抽卡分析.GachaService.Models;
using 鸣潮抽卡分析.GachaService.Utilities;

namespace 鸣潮抽卡分析.GachaService.Services
{
	public class GachaDataService : IGachaDataService
	{
		private readonly HttpClient _client;

		public GachaDataService()
		{
			_client = HttpClientFactory.CreateClient();
		}

		public async Task<(List<GachaRecord> records, string error)> GetRecordsAsync(
			RequestParams parameters,
			int poolType)
		{
			try
			{
				var requestBody = new
				{
					playerId = parameters.PlayerId,
					cardPoolId = parameters.GachaId,
					cardPoolType = poolType,
					serverId = parameters.ServerId,
					languageCode = parameters.LanguageCode,
					recordId = parameters.RecordId
				};

				var content = new StringContent(
					JsonSerializer.Serialize(requestBody, new JsonSerializerOptions
					{
						PropertyNamingPolicy = JsonNamingPolicy.CamelCase
					}),
					Encoding.UTF8,
					"application/json");

				var response = await _client.PostAsync("gacha/record/query", content);
				response.EnsureSuccessStatusCode();

				using var responseStream = await response.Content.ReadAsStreamAsync();
				var apiResponse = await JsonSerializer.DeserializeAsync<GachaApiResponse>(
					responseStream,
					new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

				return HandleApiResponse(apiResponse);
			}
			catch (Exception ex)
			{
				return (new List<GachaRecord>(), $"请求失败: {ex.Message}");
			}
		}

		private (List<GachaRecord>, string) HandleApiResponse(GachaApiResponse response)
		{
			if (response?.Code != 0 || response.Data?.Records == null)
			{
				return (new List<GachaRecord>(),
					response == null ? "无效响应" : $"错误代码: {response.Code}");
			}
			return (response.Data.Records, null);
		}
	}
}
