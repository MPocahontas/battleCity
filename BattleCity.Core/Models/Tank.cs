using BattleCity.Core.Enums;
using BattleCity.Core.Models.Base;

namespace BattleCity.Core.Models
{
	public class Tank : BaseMapObject
	{
		public const int Height = 5;
		public const int Width = 5;

		private const int XSpeed = 2;
		private const int YSpeed = 1;

		private Direction _gunDirection;
		private int _oldX;
		private int _oldY;

		public Tank(int x, int y, Direction gunDirection) : base(x, y, Width, Height)
		{
			_oldX = x;
			_oldY = y;
			_gunDirection = gunDirection;
		}

		public void Move(Direction direction)
		{
			_oldX = X;
			_oldY = Y;

			_gunDirection = direction;

			switch (direction)
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

		public void RollbackState()
		{
			X = _oldX;
			Y = _oldY;
		}
	}
}