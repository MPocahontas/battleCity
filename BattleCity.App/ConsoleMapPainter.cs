using System;
using System.Drawing;
using BattleCity.Core;
using BattleCity.Core.Enums;
using BattleCity.Core.Models;
using BattleCity.Core.Models.Base;
using BattleCity.Core.Models.Bonuses;
using BattleCity.Core.Services.Abstractions;

namespace BattleCity.App
{
	public class ConsoleMapPainter : IMapPainter
	{
		private static readonly Object Locker = new object();

		public void Draw(Map map)
		{
			Console.Clear();
			PaintBorders();

			foreach (var brickWall in map.BrickWalls)
			{
				Paint(brickWall, ConsoleColor.DarkRed);
			}

			foreach (var concreteWall in map.ConcreteWalls)
			{
				Paint(concreteWall, ConsoleColor.White);
			}

			foreach (var river in map.Rivers)
			{
				Paint(river, ConsoleColor.Blue);
			}

			Paint(map.TankA, ConsoleColor.Red);
			Paint(map.TankB, ConsoleColor.DarkBlue);
			
			Console.SetCursorPosition(0, Constants.MapHeight);
		}

		public void Draw(IBonus bonus)
		{
			lock (Locker)
			{
				if (bonus is ArmorBonus)
					Paint(bonus, ConsoleColor.DarkCyan);
				else if (bonus is AttackBonus)
					Paint(bonus, ConsoleColor.DarkGray);
			}
		}

		public void Redraw(Bullet bullet)
		{
			lock (Locker)
			{
				ClearUnsafe(bullet.GetOldRectangle());
				Paint(bullet, ConsoleColor.Gray);
				Console.SetCursorPosition(0, Constants.MapHeight);
			}
		}

		public void Redraw(Tank tank, Map map)
		{
			lock (Locker)
			{
				ClearUnsafe(tank.GetOldRectangle());
				Paint(tank, tank.Equals(map.TankA) ? ConsoleColor.Red : ConsoleColor.DarkBlue);
				Console.SetCursorPosition(0, Constants.MapHeight);
			}
		}

		public void Clear(Rectangle rectangle)
		{
			lock (Locker)
			{
				ClearUnsafe(rectangle);
			}
		}

		public void ClearUnsafe(Rectangle rectangle)
		{
			for (int i = rectangle.Left; i <= rectangle.Right; i++)
			{
				for (int j = rectangle.Top; j <= rectangle.Bottom; j++)
				{
					Console.SetCursorPosition(i, j);
					Console.Write(" ");
				}
			}

			Console.SetCursorPosition(0, Constants.MapHeight);
		}

		private void Paint(Tank tank, ConsoleColor color)
		{
			Paint((BaseMapObject)tank, color);
			Console.ForegroundColor = color;
			var centeredX = tank.X + Tank.Width / 2;
			var centeredY = tank.Y + Tank.Height / 2;
			Console.SetCursorPosition(centeredX, centeredY);
			Console.Write("[]");

			switch (tank.GunDirection)
			{
				case Direction.Up:
					Console.SetCursorPosition(centeredX, centeredY - 1);
					Console.Write("|");
					break;
				case Direction.Right:
					Console.SetCursorPosition(centeredX + 2, centeredY);
					Console.Write("-");
					break;
				case Direction.Down:
					Console.SetCursorPosition(centeredX, centeredY + 1);
					Console.Write("|");
					break;
				case Direction.Left:
					Console.SetCursorPosition(centeredX - 1, centeredY);
					Console.Write("-");
					break;
			}

			Console.ResetColor();
		}

		private void PaintBorders()
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

		private void Paint(IDrawable src, ConsoleColor color)
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