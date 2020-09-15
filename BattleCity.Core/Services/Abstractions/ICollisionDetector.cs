using BattleCity.Core.Models;

namespace BattleCity.Core.Services.Abstractions
{
	public interface ICollisionDetector
	{
		bool IsDetected(Tank tank, Map map);
	}
}