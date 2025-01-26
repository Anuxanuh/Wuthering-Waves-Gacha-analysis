using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace 鸣潮抽卡分析.GachaService.Models;

public class RequestParams
{
	[JsonPropertyName("playerId")]
	public string PlayerId { get; set; }

	[JsonPropertyName("cardPoolId")]
	public string CardPoolId { get; set; }

	[JsonPropertyName("cardPoolType")]
	public int CardPoolType { get; set; }

	[JsonPropertyName("serverId")]
	public string ServerId { get; set; }

	[JsonPropertyName("languageCode")]
	public string LanguageCode => "zh-Hans";

	[JsonPropertyName("recordId")]
	public string RecordId { get; set; }

	public RequestParams(GachaUrlParser parser, int cardPoolType)
	{
		PlayerId = parser.PlayerId.ToString();
		CardPoolId = parser.ResourcesId;
		CardPoolType = cardPoolType;
		ServerId = parser.ServerId;
		RecordId = parser.RecordId;
	}
}
