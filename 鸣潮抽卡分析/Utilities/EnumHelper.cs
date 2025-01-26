using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 鸣潮抽卡分析.Utilities;

public static class EnumHelper
{
	public static IEnumerable<EnumItem> GetEnumItems<T>() where T : Enum
	{
		return Enum.GetValues(typeof(T))
			.Cast<T>()
			.Select(e => new EnumItem
			{
				Value = Convert.ToInt32(e),
				Description = GetEnumDescription(e)
			});
	}

	private static string GetEnumDescription(Enum value)
	{
		var field = value.GetType().GetField(value.ToString());
		var attribute = (DescriptionAttribute) Attribute.GetCustomAttribute(
			field,
			typeof(DescriptionAttribute));
		return attribute?.Description ?? value.ToString();
	}

	public class EnumItem
	{
		public int Value { get; set; }
		public string Description { get; set; }
	}
}
