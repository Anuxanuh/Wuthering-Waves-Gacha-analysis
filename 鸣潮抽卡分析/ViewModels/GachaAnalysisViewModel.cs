using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 鸣潮抽卡分析.Models;
using 鸣潮抽卡分析.Services;

namespace 鸣潮抽卡分析.ViewModels;

public partial class GachaAnalysisViewModel : ObservableObject
{
	/// <summary>
	/// 是否显示三星
	/// </summary>
	private bool _notShowThreeStar = false;
	public bool NotShowThreeStar
	{
		get => _notShowThreeStar;
		set
		{
			SetProperty(ref _notShowThreeStar, value);
			OnPropertyChanged(nameof(GachaRecords));
		}
	}

	/// <summary>
	/// 抽卡记录
	/// </summary>
	private ObservableCollection<GachaRecord> _gachaRecords;
	public ObservableCollection<GachaRecord> GachaRecords
	{
		get => NotShowThreeStar
			? new ObservableCollection<GachaRecord>(_gachaRecords.Where(x => x.QualityLevel > 3))
			: _gachaRecords;
		set
		{
			SetProperty(ref _gachaRecords, value);
			OnPropertyChanged(nameof(AwayFromFiveStar));
			OnPropertyChanged(nameof(FiveStarRate));
			OnPropertyChanged(nameof(AwayFromFourStar));
			OnPropertyChanged(nameof(FourStarRate));
		}
	}

	/// <summary>
	/// 离上次出五星的抽数
	/// </summary>
	public int AwayFromFiveStar => GachaRecords.TakeWhile(x => x.QualityLevel != 5).Count();

	/// <summary>
	/// 平均多少发出一个五星
	/// </summary>
	public string FiveStarRate => ((double) GachaRecords.Count / GachaRecords.Count(x => x.QualityLevel == 5)).ToString("F2");

	/// <summary>
	///离上次出出四星的抽数
	/// </summary>
	public int AwayFromFourStar => GachaRecords.TakeWhile(x => x.QualityLevel != 4).Count();

	/// <summary>
	/// 平均多少发出一个四星
	/// </summary>
	public string FourStarRate => ((double) GachaRecords.Count / GachaRecords.Count(x => x.QualityLevel == 4)).ToString("F2");
}
