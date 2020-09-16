using System;
using System.Drawing;
using System.Linq;
using BattleCity.Core.Models;
using BattleCity.Core.Models.Base;
using BattleCity.Core.Services.Abstractions;

namespace BattleCity.Core.Services.Implementations
{
	public class CollisionDetector : ICollisionDetector
	{
		public bool IsDetected(Tank tank, Map map)
		{
			if (IsOutOfTheMap(tank, Tank.Width, Tank.Height))
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

			if (tank.Equals(map.TankA))
			{
				if (tank.GetRectangle().IntersectsWith(map.TankB.GetRectangle()))
					return true;
			}
			else
			{
				if (tank.GetRectangle().IntersectsWith(map.TankA.GetRectangle()))
					return true;
			}

			return false;
		}

		public bool IsOutOfTheMap(BaseMapObject src, int width, int height)
		{
			if (src.X < 0 || src.X + width >= Constants.MapWidth)
				return true;

			if (src.Y < 0 || src.Y + height >= Constants.MapHeight)
				return true;

			return false;
		}

		public Point GetFreeSpacePoint(int width, int height, Map map)
		{
			int x;
			int y;
			do
			{
				x = new Random().Next(0, Constants.MapWidth - width);
				y = new Random().Next(0, Constants.MapHeight - height);
			} 
			while (!IsRectangleOnFreeSpace(new Rectangle(x, y, width, height), map));

			return new Point(x, y);
		}

		private bool IsRectangleOnFreeSpace(Rectangle rectangle, Map map)
		{
			if (map.TankA != null && map.TankA.GetRectangle().IntersectsWith(rectangle))
				return false;

			if (map.TankB != null && map.TankB.GetRectangle().IntersectsWith(rectangle))
				return false;

			if (map.FlagA.GetRectangle().IntersectsWith(rectangle))
				return false;

			if (map.FlagB.GetRectangle().IntersectsWith(rectangle))
				return false;

			if (map.ConcreteWalls.Any(_ => _.GetRectangle().IntersectsWith(rectangle)))
				return false;

			if (map.BrickWalls.Any(_ => _.GetRectangle().IntersectsWith(rectangle)))
				return false;

			if (map.Rivers.Any(_ => _.GetRectangle().IntersectsWith(rectangle)))
				return false;

			if (map.Bonuses.Any(_ => _.GetRectangle().IntersectsWith(rectangle)))
				return false;

			if (map.Bullets.Any(_ => _.GetRectangle().IntersectsWith(rectangle)))
				return false;

			return true;
		}
	}
}