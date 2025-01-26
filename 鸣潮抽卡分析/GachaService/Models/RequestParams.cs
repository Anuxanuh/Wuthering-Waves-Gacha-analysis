using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 鸣潮抽卡分析.GachaService.Models;

public class RequestParams
{
	public string PlayerId { get; set; }
	public string CardPoolId { get; set; }
	public int CardPoolType { get; set; }
	public string ServerId { get; set; }
	public string LanguageCode => "zh-Hans";
	public string RecordId { get; set; }

	public RequestParams(GachaUrlParser parser, int cardPoolType)
	{
		PlayerId = parser.PlayerId.ToString();
		CardPoolId = parser.GachaId;
		CardPoolType = cardPoolType;
		ServerId = parser.ServerId;
		RecordId = parser.RecordId;
	}
}
