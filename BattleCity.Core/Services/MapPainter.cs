using System;
using BattleCity.Core.Models;

namespace BattleCity.Core.Services
{
	public class MapPainter
	{
		public void Paint(Map map)
		{
			foreach (var brickWall in map.BrickWalls)
			{
				Console.SetCursorPosition(brickWall.Point.X, brickWall.Point.Y);
				Console.Write("B");
			}

			foreach (var concreteWall in map.ConcreteWalls)
			{
				Console.SetCursorPosition(concreteWall.Point.X, concreteWall.Point.Y);
				Console.Write("C");
			}

			foreach (var river in map.Rivers)
			{
				Console.SetCursorPosition(river.Point.X, river.Point.Y);
				Console.Write("R");
			}
		}
	}
}