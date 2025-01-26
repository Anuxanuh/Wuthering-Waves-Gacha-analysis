using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 鸣潮抽卡分析.GachaService.Models;

public class GachaRecord
{
	public string CardPoolType { get; set; }
	public long ResourceId { get; set; }
	public int QualityLevel { get; set; }
	public string ResourceType { get; set; }
	public string Name { get; set; }
	public int Count { get; set; }
	public DateTime Time { get; set; }

	public GachaRecord() { }

	/// <summary>
	/// 全参数构造函数
	/// </summary>
	public GachaRecord(
		string cardPoolType,
		long resourceId,
		int qualityLevel,
		string resourceType,
		string name,
		int count,
		DateTime time)
	{
		CardPoolType = cardPoolType ?? throw new ArgumentNullException(nameof(cardPoolType));
		ResourceId = resourceId > 0 ? resourceId : throw new ArgumentException("ResourceId必须为正整数");
		QualityLevel = qualityLevel is >= 1 and <= 5 ? qualityLevel : throw new ArgumentException("QualityLevel应在1-5范围内");
		ResourceType = resourceType ?? throw new ArgumentNullException(nameof(resourceType));
		Name = name ?? throw new ArgumentNullException(nameof(name));
		Count = count > 0 ? count : throw new ArgumentException("Count必须为正整数");
		Time = time;
	}

	/// <summary>
	/// 重写ToString方法，便于调试输出
	/// </summary>
	public override string ToString()
	{
		return $"[{Time:yyyy-MM-dd HH:mm:ss}] {Name} ({ResourceType}) - 品级{QualityLevel}星";
	}
}
