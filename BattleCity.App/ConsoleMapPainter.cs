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
			lock (Locker)
			{
				Console.Clear();
				DrawBorders();

				foreach (var brickWall in map.BrickWalls)
				{
					Draw(brickWall, Colors.BrickWall);
				}

				foreach (var concreteWall in map.ConcreteWalls)
				{
					Draw(concreteWall, Colors.ConcreteWall);
				}

				foreach (var river in map.Rivers)
				{
					Draw(river, Colors.River);
				}

				foreach (var bullet in map.Bullets)
				{
					Draw(bullet, Colors.Bullet);
				}

				foreach (var bonus in map.Bonuses)
				{
					DrawUnsafe(bonus);
				}

				Draw(map.FlagA, Colors.TeamA);
				Draw(map.TankA, Colors.TeamA);
				Draw(map.FlagB, Colors.TeamB);
				Draw(map.TankB, Colors.TeamB);

				Console.SetCursorPosition(0, Constants.MapHeight);
			}
		}

		public void Draw(IBonus bonus)
		{
			lock (Locker)
			{
				DrawUnsafe(bonus);
				Console.SetCursorPosition(0, Constants.MapHeight);
			}
		}

		public void Draw(Bullet bullet)
		{
			lock (Locker)
			{
				Draw(bullet, Colors.Bullet);
				Console.SetCursorPosition(0, Constants.MapHeight);
			}
		}

		public void Redraw(Bullet bullet)
		{
			lock (Locker)
			{
				ClearUnsafe(bullet.GetOldRectangle());
				Draw(bullet, Colors.Bullet);
				Console.SetCursorPosition(0, Constants.MapHeight);
			}
		}

		public void Redraw(Tank tank)
		{
			lock (Locker)
			{
				ClearUnsafe(tank.GetOldRectangle());
				Draw(tank, tank.Team == Team.A ? Colors.TeamA : Colors.TeamB);
				Console.SetCursorPosition(0, Constants.MapHeight);
			}
		}

		public void Clear(Rectangle rectangle)
		{
			lock (Locker)
			{
				ClearUnsafe(rectangle);
				Console.SetCursorPosition(0, Constants.MapHeight);
			}
		}

		public void DrawWinMessage(Team winner)
		{
			lock (Locker)
			{
				Console.Clear();
				Console.WriteLine($"Team {winner} victory!");
			}
		}

		private void ClearUnsafe(Rectangle rectangle)
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

		private void DrawUnsafe(IBonus bonus)
		{
			if (bonus is ArmorBonus)
				Draw(bonus, Colors.ArmorBonus);
			else if (bonus is AttackBonus)
				Draw(bonus, Colors.AttackBonus);
		}

		private void Draw(Tank tank, ConsoleColor color)
		{
			Draw((BaseMapObject)tank, color);
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

		private void DrawBorders()
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

		private void Draw(IDrawable src, ConsoleColor color)
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