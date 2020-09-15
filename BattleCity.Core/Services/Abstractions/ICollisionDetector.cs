using BattleCity.Core.Models;
using BattleCity.Core.Models.Base;

namespace BattleCity.Core.Services.Abstractions
{
	public interface ICollisionDetector
	{
		bool IsDetected(Tank tank, Map map);

		bool IsOutOfTheMap(BaseMapObject src, int width, int height);
	}
}