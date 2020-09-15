using System;
using BattleCity.Core.Models;
using BattleCity.Core.Services.Abstractions;

namespace BattleCity.App
{
	public class ConsoleMapPainter : IMapPainter
	{
		public void Paint(Map map)
		{
			foreach (var brickWall in map.BrickWalls)
			{
				Console.SetCursorPosition(brickWall.X, brickWall.Y);
				Console.Write("B");
			}

			foreach (var concreteWall in map.ConcreteWalls)
			{
				Console.SetCursorPosition(concreteWall.X, concreteWall.Y);
				Console.Write("C");
			}

			foreach (var river in map.Rivers)
			{
				Console.SetCursorPosition(river.X, river.Y);
				Console.Write("R");
			}
		}
	}
}