﻿using BattleCity.Core.Models;
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
	}
}