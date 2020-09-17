using System;
using System.Drawing;
using BattleCity.Core.Enums;
using BattleCity.Core.Models.Base;

namespace BattleCity.Core.Models
{
	public class Bullet : BaseMapObject, IRedrawable
	{
		public const int Width = 1;
		public const int Height = 1;
		private const int XSpeed = 1;
		private const int YSpeed = 1;

		public Direction Direction { get; }

		/// <summary>
		/// an integer that indicates how fast the bullet will move
		/// </summary>
		public int Speed { get; }

		public Bullet(int x, int y, Direction direction, int speed)
			: base(x, y, Width, Height)
		{
			Direction = direction;
			Speed = speed;
		}

		public void Move()
		{
			switch (Direction)
			{
				case Direction.Up:
					Y -= YSpeed;
					break;
				case Direction.Right:
					X += XSpeed;
					break;
				case Direction.Down:
					Y += YSpeed;
					break;
				case Direction.Left:
					X -= XSpeed;
					break;
			}
		}

		/// <summary>
		/// Returns rectangle for previous map position
		/// </summary>
		/// <returns></returns>
		public Rectangle GetOldRectangle()
		{
			switch (Direction)
			{
				case Direction.Up:
					return new Rectangle(X, Y + YSpeed, Width, Height);
				case Direction.Right:
					return new Rectangle(X - XSpeed, Y, Width, Height);
				case Direction.Down:
					return new Rectangle(X, Y - YSpeed, Width, Height);
				case Direction.Left:
					return new Rectangle(X + XSpeed, Y, Width, Height);
				default:
					throw new ArgumentOutOfRangeException($"Invalid value {Direction} for enum {nameof(Direction)}");
			}
		}
	}
}