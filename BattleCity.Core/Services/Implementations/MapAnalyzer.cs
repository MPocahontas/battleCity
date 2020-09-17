using System;
using System.Drawing;
using System.Linq;
using BattleCity.Core.Enums;
using BattleCity.Core.Models;
using BattleCity.Core.Models.Base;
using BattleCity.Core.Services.Abstractions;

namespace BattleCity.Core.Services.Implementations
{
	/// <summary>
	/// Class contains help methods for working with map
	/// </summary>
	public class MapAnalyzer : IMapAnalyzer
	{
		/// <summary>
		/// Returns true if tank collised with any other object on map or left the map
		/// </summary>
		public bool IsCollisionDetected(Tank tank, Map map)
		{
			if (IsOutOfTheMapBorders(tank, Tank.Width, Tank.Height))
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

			if (tank.Team == Team.A && map.TankB != null)
			{
				if (tank.IntersectsWith(map.TankB))
					return true;
			}

			if (tank.Team == Team.B && map.TankA != null)
			{
				if (tank.IntersectsWith(map.TankA))
					return true;
			}

			return false;
		}


		/// <summary>
		/// Returns true if object out of map borders
		/// </summary>
		/// <returns></returns>
		public bool IsOutOfTheMapBorders(BaseMapObject src, int width, int height)
		{
			if (src.X < 0 || src.X + width >= Constants.MapWidth)
				return true;

			if (src.Y < 0 || src.Y + height >= Constants.MapHeight)
				return true;

			return false;
		}


		/// <summary>
		/// Looking for a rectangle on the map that can be placed without collisions
		/// </summary>
		/// <param name="width">Width of required rectangle</param>
		/// <param name="height">Heigh of required rectangle</param>
		/// <returns>Left-Top rectangle point</returns>
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