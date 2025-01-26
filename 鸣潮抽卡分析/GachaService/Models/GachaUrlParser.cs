using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace 鸣潮抽卡分析.GachaService.Models;

/// <summary>
/// 抽卡URL解析器
/// </summary>
public class GachaUrlParser
{
	/// <summary>
	/// 服务器ID（32位哈希）
	/// </summary>
	public string ServerId { get; private set; }

	/// <summary>
	/// 玩家ID（纯数字）
	/// </summary>
	public long PlayerId { get; private set; }

	/// <summary>
	/// 语言代码（符合BCP47标准）
	/// </summary>
	public string Language { get; private set; }

	/// <summary>
	/// 卡池ID（业务标识）
	/// </summary>
	public string GachaId { get; private set; }

	/// <summary>
	/// 卡池类型（1-7）
	/// </summary>
	public int GachaType { get; private set; }

	/// <summary>
	/// 服务器区域代码
	/// </summary>
	public string ServerArea { get; private set; }

	/// <summary>
	/// 记录ID（32位哈希）
	/// </summary>
	public string RecordId { get; private set; }

	/// <summary>
	/// 资源ID（32位哈希）
	/// </summary>
	public string ResourcesId { get; private set; }

	/// <summary>
	/// 平台标识（大写）
	/// </summary>
	public string Platform { get; private set; }

	/// <summary>
	/// 原始参数集合
	/// </summary>
	public Dictionary<string, string> RawParameters { get; } = new();

	/// <summary>
	/// 从完整URL解析参数
	/// </summary>
	public static GachaUrlParser Parse(string fullUrl)
	{
		var parser = new GachaUrlParser();
		var uri = new Uri(fullUrl);

		// 提取哈希部分参数
		var hashParts = uri.Fragment.Split('?');
		if (hashParts.Length < 2)
			return parser;

		var queryString = hashParts[1];
		var parameters = HttpUtility.ParseQueryString(queryString);

		// 存储原始参数
		foreach (string key in parameters.AllKeys)
		{
			if (!string.IsNullOrEmpty(key))
			{
				var value = parameters[key];
				parser.RawParameters[key] = value;
			}
		}

		// 解析具体参数
		parser.ServerId = parameters["svr_id"];
		parser.PlayerId = long.TryParse(parameters["player_id"], out var pid) ? pid : 0;
		parser.Language = parameters["lang"];
		parser.GachaId = parameters["gacha_id"];
		parser.GachaType = int.TryParse(parameters["gacha_type"], out var gtype) ? gtype : 0;
		parser.ServerArea = parameters["svr_area"]?.ToUpper();
		parser.RecordId = parameters["record_id"];
		parser.ResourcesId = parameters["resources_id"];
		parser.Platform = parameters["platform"]?.ToUpper();

		return parser;
	}

	/// <summary>
	/// 参数验证（基础格式检查）
	/// </summary>
	public bool Validate()
	{
		return !string.IsNullOrEmpty(ServerId)
			&& ServerId.Length == 32
			&& PlayerId > 0
			&& !string.IsNullOrEmpty(Language)
			&& !string.IsNullOrEmpty(GachaId)
			&& GachaType > 0
			&& !string.IsNullOrEmpty(ServerArea)
			&& ServerArea.Length == 2
			&& !string.IsNullOrEmpty(RecordId)
			&& RecordId.Length == 32
			&& !string.IsNullOrEmpty(ResourcesId)
			&& ResourcesId.Length == 32
			&& !string.IsNullOrEmpty(Platform);
	}

	/// <summary>
	/// 输出调试信息
	/// </summary>
	public override string ToString()
	{
		return $"Player {PlayerId} on {ServerArea} server, GachaType {GachaType} ({GachaId})";
	}
}

