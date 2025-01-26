using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 鸣潮抽卡分析.ViewModels;

public partial class GachaRecordViewModel : ObservableObject
{
	[ObservableProperty]
	private string cardPoolType;

	[ObservableProperty]
	private long resourceId;

	[ObservableProperty]
	private int qualityLevel;

	[ObservableProperty]
	private string resourceType;

	[ObservableProperty]
	private string name;

	[ObservableProperty]
	private int count;

	[ObservableProperty]
	private DateTime time;
}
