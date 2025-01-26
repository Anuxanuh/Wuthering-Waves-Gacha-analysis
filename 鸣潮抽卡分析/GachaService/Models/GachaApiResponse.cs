using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace 鸣潮抽卡分析.GachaService.Models
{
	public class GachaApiResponse
	{
		[JsonPropertyName("code")]
		public int Code { get; set; }

		[JsonPropertyName("data")]
		public GachaData Data { get; set; }

		public class GachaData
		{
			[JsonPropertyName("records")]
			public List<GachaRecord> Records { get; set; }
		}
	}
}
