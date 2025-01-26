using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 鸣潮抽卡分析.GachaService.Models;

namespace 鸣潮抽卡分析.GachaService.Services
{
	public interface IGachaDataService
	{
		Task<(List<GachaRecord> records, string error)> GetRecordsAsync(RequestParams parameters, int poolType);
	}
}
