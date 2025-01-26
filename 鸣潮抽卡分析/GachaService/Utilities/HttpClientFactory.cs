using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace 鸣潮抽卡分析.GachaService.Utilities
{
	public static class HttpClientFactory
	{
		public static HttpClient CreateClient()
		{
			var handler = new HttpClientHandler
			{
				AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
				ServerCertificateCustomValidationCallback = (_, _, _, _) => true
			};

			return new HttpClient(handler)
			{
				Timeout = TimeSpan.FromSeconds(30),
				BaseAddress = new System.Uri("https://gmserver-api.aki-game2.com/")
			};
		}
	}
}
