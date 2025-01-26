using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 鸣潮抽卡分析.Enums;

/// <summary>
/// 卡池类型
/// </summary>
public enum PoolType
{
	[Description("角色活动唤取")]
	CharacterEvent = 1,

	[Description("武器活动唤取")]
	WeaponEvent = 2,

	[Description("角色常驻唤取")]
	CharacterStandard = 3,

	[Description("武器常驻唤取")]
	WeaponStandard = 4,

	[Description("新手唤取")]
	Beginner = 5,

	[Description("新手自选唤取")]
	BeginnerSelect = 6,

	[Description("新手自选唤取（感恩定向唤取）")]
	BeginnerThanksgiving = 7
}
