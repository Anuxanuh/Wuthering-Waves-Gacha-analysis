using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System;
using 鸣潮抽卡分析.GachaService.Utilities;
using 鸣潮抽卡分析.GachaService.Enums;

namespace 鸣潮抽卡分析;
public partial class MainViewModel : ObservableObject
{
	[ObservableProperty]
	private EnumHelper.EnumItem _selectedPoolType;

	[ObservableProperty]
	private ObservableCollection<EnumHelper.EnumItem> _poolTypes;

	public MainViewModel()
	{
		// 初始化卡池类型列表
		PoolTypes = new ObservableCollection<EnumHelper.EnumItem>(
			EnumHelper.GetEnumItems<PoolType>());

		// 设置默认选择第一个
		SelectedPoolType = PoolTypes.FirstOrDefault();
	}
}
public class PoolTypeItem
{
	public int Value { get; }
	public string DisplayName { get; }

	public PoolTypeItem(int value, string displayName)
	{
		Value = value;
		DisplayName = $"{value} - {displayName}";
	}
}