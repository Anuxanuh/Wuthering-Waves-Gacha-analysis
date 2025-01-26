using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using 鸣潮抽卡分析.Enums;
using 鸣潮抽卡分析.Models;
using 鸣潮抽卡分析.Services;
using 鸣潮抽卡分析.Utilities;

namespace 鸣潮抽卡分析;
public partial class MainViewModel : ObservableObject
{
	#region 仓储服务
	private readonly RepositoryServiceForRecord RecordDataService = new();
	private readonly RepositoryServiceForSetting SettingDataService = new();
	#endregion 仓储服务

	#region 双向绑定属性
	[ObservableProperty]
	private ObservableCollection<EnumHelper.EnumItem> _poolTypes;

	private EnumHelper.EnumItem _selectedPoolType;
	public EnumHelper.EnumItem SelectedPoolType
	{
		get => _selectedPoolType;
		set
		{
			SetProperty(ref _selectedPoolType, value);
			LoadGachaData();
		}
	}

	[ObservableProperty]
	private ObservableCollection<GachaRecord> _gachaRecords;
	#endregion 双向绑定属性

	public MainViewModel()
	{
		// 初始化卡池类型列表
		PoolTypes = new ObservableCollection<EnumHelper.EnumItem>(
			EnumHelper.GetEnumItems<PoolType>());

		// 设置默认选择第一个
		SelectedPoolType = PoolTypes.FirstOrDefault();

		// 加载抽卡记录
		GachaRecords = new ObservableCollection<GachaRecord>(
			RecordDataService.LoadGachaRecords(
				SettingDataService.CurrentPlayerId, SelectedPoolType.Value));
	}

	[RelayCommand]
	private async void LoadGachaData()
	{
		// 查找抽卡记录url
		var gachaUrl = new GachaUrlFinder().FindUrl(SettingDataService.GamePath);
		if (gachaUrl == null)
		{
			MessageBox.Show("请打开游戏内抽卡记录!");
			return;
		}
		// 解析URL, 提取参数
		var gachaUrlParser = GachaUrlParser.Parse(gachaUrl);
		// 构造请求参数
		var requestParams = new RequestParams(gachaUrlParser, SelectedPoolType.Value);
		// 获取抽卡记录
		var gachaApiResponse = await GachaDataService.GetRecordsAsync(requestParams);
		// 错误处理
		// 官方的Api返回Code不为0时, 说明有错误
		if (gachaApiResponse?.Code != 0)
		{
			MessageBox.Show("请打开游戏内抽卡记录并重试\n" +
				$"错误信息: {gachaApiResponse?.Message}\n" +
				$"Code={gachaApiResponse?.Code}");
			return;
		}
		// 赋值, 显示抽卡记录
		GachaRecords = new ObservableCollection<GachaRecord>(gachaApiResponse.Data);
		// 仓储服务更新数据
		RecordDataService.SaveGachaRecords(gachaUrlParser.PlayerId, SelectedPoolType.Value, gachaApiResponse.Data);// 保存抽卡记录
		SettingDataService.CurrentPlayerId = gachaUrlParser.PlayerId;// 保存当前玩家ID
	}
}