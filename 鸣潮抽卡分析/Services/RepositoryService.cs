using System.IO;
using System.Text.Json;
using 鸣潮抽卡分析.Models;

namespace 鸣潮抽卡分析.Services;

/// <summary>
/// 抽卡记录仓库服务
/// </summary>
public class RepositoryServiceForRecord
{
	public RepositoryServiceForRecord()
	{
		// 如果基础目录不存在，创建目录
		if (!Directory.Exists(_baseDirectory))
			Directory.CreateDirectory(_baseDirectory);
	}

	// 基础目录路径
	private static string _baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");

	/// <summary>
	/// 保存/更新抽卡记录
	/// </summary>
	/// <param name="userId">用户ID</param>
	/// <param name="cardPoolType">卡池类型</param>
	/// <param name="records">抽卡记录列表</param>
	/// <returns>保存是否成功</returns>
	public bool SaveGachaRecords(long userId, int cardPoolType, List<GachaRecord> records)
	{
		// 如果文件存在，更新记录，否则保存新记录
		if (File.Exists(Path.Combine(_baseDirectory, $"{userId}_{cardPoolType}.json")))
			return UpdateGachaRecords(userId, cardPoolType, records);
		else
			return SaveGachaRecords2(userId, cardPoolType, records);
	}

	/// <summary>
	/// 加载抽卡记录
	/// </summary>
	/// <param name="userId">用户ID</param>
	/// <param name="cardPoolType">卡池类型</param>
	/// <returns>抽卡记录列表</returns>
	public List<GachaRecord> LoadGachaRecords(long userId, int cardPoolType)
	{
		try
		{
			// 构建文件路径
			var path = Path.Combine(_baseDirectory, $"{userId}_{cardPoolType}.json");
			// 读取文件内容
			var json = File.ReadAllText(path);
			// 反序列化为对象列表
			return JsonSerializer.Deserialize<List<GachaRecord>>(json);
		}
		catch (Exception)
		{
			// 如果发生异常，返回空列表
			return new List<GachaRecord>();
		}
	}

	/// <summary>
	/// 保存抽卡记录(不更新)
	/// </summary>
	/// <param name="userId">用户ID</param>
	/// <param name="cardPoolType">卡池类型</param>
	/// <param name="records">抽卡记录列表</param>
	/// <returns>保存是否成功</returns>
	private bool SaveGachaRecords2(long userId, int cardPoolType, List<GachaRecord> records)
	{
		try
		{
			// 构建文件路径
			var path = Path.Combine(_baseDirectory, $"{userId}_{cardPoolType}.json");
			// 序列化对象列表为JSON字符串
			var json = JsonSerializer.Serialize(records, new JsonSerializerOptions
			{
				Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
				WriteIndented = true
			});
			// 写入文件
			File.WriteAllText(path, json, System.Text.Encoding.UTF8);
			return true;
		}
		catch (Exception)
		{
			// 如果发生异常，返回false
			return false;
		}
	}

	/// <summary>
	/// 更新抽卡记录
	/// </summary>
	/// <param name="userId">用户ID</param>
	/// <param name="cardPoolType">卡池类型</param>
	/// <param name="newRecords">新抽卡记录列表</param>
	/// <returns>更新是否成功</returns>
	private bool UpdateGachaRecords(long userId, int cardPoolType, List<GachaRecord> newRecords)
	{
		try
		{
			// 构建文件路径
			var path = Path.Combine(_baseDirectory, $"{userId}_{cardPoolType}.json");
			// 加载旧记录
			var oldRecords = LoadGachaRecords(userId, cardPoolType);
			// 找到新记录中时间早于旧记录的部分
			var newUniqueRecords = newRecords[0..newRecords.FindIndex(r => r.Time >= oldRecords[0].Time)];
			// 将旧记录添加到新记录前面
			newUniqueRecords.AddRange(oldRecords);
			// 序列化合并后的记录为JSON字符串
			var json = JsonSerializer.Serialize(newUniqueRecords, new JsonSerializerOptions
			{
				Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
				WriteIndented = true
			});
			// 写入文件
			File.WriteAllText(path, json, System.Text.Encoding.UTF8);
			return true;
		}
		catch (Exception)
		{
			// 如果发生异常，返回false
			return false;
		}
	}
}

/// <summary>
/// 程序设置仓库服务
/// </summary>
public class RepositoryServiceForSetting
{
	// 设置数据文件路径
	private static string _settingDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Setting.json");

	// 内部类：设置
	private class Setting
	{
		public long CurrentPlayerId { get; set; }
		public string? GamePath { get; set; }
		public List<long> Players { get; set; } = new List<long>();
	}

	// 设置对象
	private Setting? _setting;

	/// <summary>
	/// 构造函数
	/// </summary>
	public RepositoryServiceForSetting()
	{
		// 获取目标文件夹路径（去掉文件名）
		string directoryPath = Path.GetDirectoryName(_settingDataPath);

		// 如果文件夹不存在，则自动创建（包括所有父级目录）
		if (!Directory.Exists(directoryPath))
		{
			Directory.CreateDirectory(directoryPath);
		}


		// 如果设置文件不存在，创建新设置
		if (!File.Exists(_settingDataPath))
		{
			_setting = new Setting()
			{
				GamePath = new GamePathFinder().FindGamePath()
			};
			// 写入设置文件
			File.WriteAllText(_settingDataPath, JsonSerializer.Serialize(_setting));
		}
		else
		{
			// 读取设置文件
			var json = File.ReadAllText(_settingDataPath);
			// 反序列化为设置对象
			_setting = JsonSerializer.Deserialize<Setting>(json);
		}
	}

	/// <summary>
	/// 当前玩家ID
	/// </summary>
	public long CurrentPlayerId
	{
		get => _setting.CurrentPlayerId;
		set
		{
			_setting.CurrentPlayerId = value;
			// 如果玩家列表中不包含当前玩家ID，添加到列表中
			if (!_setting.Players.Contains(value))
				_setting.Players.Add(value);
			// 更新设置文件
			File.WriteAllText(_settingDataPath, JsonSerializer.Serialize(_setting));
		}
	}

	/// <summary>
	/// 游戏路径
	/// </summary>
	public string GamePath => _setting.GamePath;
}
