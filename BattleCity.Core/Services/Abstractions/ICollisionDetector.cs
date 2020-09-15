using BattleCity.Core.Models;

namespace BattleCity.Core.Services.Abstractions
{
	public interface ICollisionDetector
	{
		bool IsCollisionDetected(Tank tank, Map map);
	}
}