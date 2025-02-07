﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace 鸣潮抽卡分析.Utilities;

public class GachaDataDateTimeConverter : JsonConverter<DateTime>
{
	private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

	public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return DateTime.ParseExact(reader.GetString(), DateTimeFormat, CultureInfo.InvariantCulture);
	}

	public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
	{
		writer.WriteStringValue(value.ToString(DateTimeFormat));
	}
}
