using System;
using BattleCity.Core;
using BattleCity.Core.Models;
using BattleCity.Core.Models.Base;
using BattleCity.Core.Services.Abstractions;

namespace BattleCity.App
{
	public class ConsoleMapPainter : IMapPainter
	{
		public void Paint(Map map)
		{
			Console.Clear();
			PaintBorder();

			foreach (var brickWall in map.BrickWalls)
			{
				Paint(brickWall, ConsoleColor.Red);
			}

			foreach (var concreteWall in map.ConcreteWalls)
			{
				Paint(concreteWall, ConsoleColor.White);
			}

			foreach (var river in map.Rivers)
			{
				Paint(river, ConsoleColor.Blue);
			}

			Console.SetCursorPosition(0, Constants.MapHeight);
		}

		private void PaintBorder()
		{
			for (int i = 0; i < Constants.MapWidth; i++)
			{
				Console.SetCursorPosition(i, Constants.MapHeight);
				Console.Write("*");
			}

			for (int i = 0; i < Constants.MapHeight; i++)
			{
				Console.SetCursorPosition(Constants.MapWidth, i);
				Console.Write("*");
			}
		}

		private void Paint(BaseMapObject src, ConsoleColor color)
		{
			var rectangle = src.GetRectangle();

			Console.ForegroundColor = color;
			Console.SetCursorPosition(rectangle.Left, rectangle.Top);
			Console.Write("╔");
			Console.SetCursorPosition(rectangle.Right, rectangle.Top);
			Console.Write("╗");
			Console.SetCursorPosition(rectangle.Left, rectangle.Bottom);
			Console.Write("╚");
			Console.SetCursorPosition(rectangle.Right, rectangle.Bottom);
			Console.Write("╝");

			for (int i = rectangle.Left + 1; i < rectangle.Right; i++)
			{
				Console.SetCursorPosition(i, rectangle.Top);
				Console.Write("═");
				Console.SetCursorPosition(i, rectangle.Bottom);
				Console.Write("═");
			}

			for (int i = rectangle.Top + 1; i < rectangle.Bottom; i++)
			{
				Console.SetCursorPosition(rectangle.Left, i);
				Console.Write("║");
				Console.SetCursorPosition(rectangle.Right, i);
				Console.Write("║");
			}

			Console.ResetColor();
		}
	}
}