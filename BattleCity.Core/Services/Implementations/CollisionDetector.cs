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

			foreach (var brickWall in map.BrickWalls)
			{
				if (tank.GetRectangle().IntersectsWith(brickWall.GetRectangle()))
					return true;
			}

			foreach (var concreteWall in map.ConcreteWalls)
			{
				if (tank.GetRectangle().IntersectsWith(concreteWall.GetRectangle()))
					return true;
			}

			foreach (var river in map.Rivers)
			{
				if (tank.GetRectangle().IntersectsWith(river.GetRectangle()))
					return true;
			}

			return false;
		}
	}
}