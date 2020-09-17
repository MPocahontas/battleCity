using System.Drawing;
using BattleCity.Core.Models;
using BattleCity.Core.Models.Base;

namespace BattleCity.Core.Services.Abstractions
{
	public interface IMapAnalyzer
	{
		bool IsCollisionDetected(Tank tank, Map map);

		bool IsOutOfTheMapBorders(BaseMapObject src, int width, int height);

		Point GetFreeSpacePoint(int width, int height, Map map);
	}
}