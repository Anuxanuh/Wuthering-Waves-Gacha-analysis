﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace 鸣潮抽卡分析.GachaService.Models;

public class GachaRecord
{
	[JsonPropertyName("cardPoolType")]
	public string CardPoolType { get; set; }

	[JsonPropertyName("resourceId")]
	public long ResourceId { get; set; }

	[JsonPropertyName("qualityLevel")]
	public int QualityLevel { get; set; }

	[JsonPropertyName("resourceType")]
	public string ResourceType { get; set; }

	[JsonPropertyName("name")]
	public string Name { get; set; }

	[JsonPropertyName("count")]
	public int Count { get; set; }

	[JsonPropertyName("time")]
	public DateTime Time { get; set; }
}
