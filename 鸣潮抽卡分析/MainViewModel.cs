using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using 鸣潮抽卡分析.GachaService.Enums;
using 鸣潮抽卡分析.GachaService.Models;
using 鸣潮抽卡分析.GachaService.Services;
using 鸣潮抽卡分析.GachaService.Utilities;

namespace 鸣潮抽卡分析;
public partial class MainViewModel : ObservableObject
{
	[ObservableProperty]
	private ObservableCollection<EnumHelper.EnumItem> _poolTypes;

	private EnumHelper.EnumItem _selectedPoolType;
	public EnumHelper.EnumItem SelectedPoolType
	{
		get => _selectedPoolType;
		set
		{
			SetProperty(ref _selectedPoolType, value);
		}
	}

	[ObservableProperty]
	private ObservableCollection<GachaRecord> _gachaRecords;

	public MainViewModel()
	{
		// 初始化卡池类型列表
		PoolTypes = new ObservableCollection<EnumHelper.EnumItem>(
			EnumHelper.GetEnumItems<PoolType>());

		// 设置默认选择第一个
		SelectedPoolType = PoolTypes.FirstOrDefault();
	}

	// 使用特性定义命令
	[RelayCommand]
	private async void GetGachaData()
	{
		var gamePath = new GamePathFinder().FindGamePath();
		var gachaUrl = new GachaUrlFinder().FindUrl(gamePath);// 查找抽卡记录url
		if (gachaUrl == null)
		{
			MessageBox.Show("请打开游戏内抽卡记录!");
			return;
		}
		var gachaUrlParser = GachaUrlParser.Parse(gachaUrl);// 解析URL, 提取参数
		var requestParams = new RequestParams(gachaUrlParser, SelectedPoolType.Value);// 构造请求参数
		var gachaApiResponse = await GachaDataService.GetRecordsAsync(requestParams);// 获取抽卡记录
		if (gachaApiResponse.Code != 0)// 如果有错误信息, 则显示错误信息
		{
			MessageBox.Show(gachaApiResponse.Message);
			return;
		}
		GachaRecords = new ObservableCollection<GachaRecord>(gachaApiResponse.Data);// 显示抽卡记录
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