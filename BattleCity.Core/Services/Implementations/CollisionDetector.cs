using BattleCity.Core.Models;
using BattleCity.Core.Services.Abstractions;

namespace BattleCity.Core.Services.Implementations
{
	public class CollisionDetector : ICollisionDetector
	{
		public bool IsDetected(Tank tank, Map map)
		{
			if (tank.X < 0 || tank.X + Tank.Width >= Constants.MapWidth)
				return true;

			if (tank.Y < 0 || tank.Y + Tank.Height >= Constants.MapHeight)
				return true;

			return false;
		}
	}
}