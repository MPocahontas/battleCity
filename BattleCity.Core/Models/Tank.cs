using BattleCity.Core.Enums;
namespace BattleCity.Core.Models
{
	public class Tank
	{
		private const int Height = 5;
		private const int Width = 5;

		private const int XSpeed = 2;
		private const int YSpeed = 2;

		private Direction _gunDirection;
		private int _oldX;
		private int _oldY;
		private int _x;
		private int _y;

		public Tank(int x, int y, Direction gunDirection)
		{
			_oldX = x;
			_oldY = y;
			_x = x;
			_y = y;
			_gunDirection = gunDirection;
		}

		public void Move(Direction direction)
		{
			_oldX = _x;
			_oldY = _y;

			_gunDirection = direction;

			switch (direction)
			{
				case Direction.Up:
					_y += YSpeed;
					break;
				case Direction.Right:
					_x += XSpeed;
					break;
				case Direction.Down:
					_y -= YSpeed;
					break;
				case Direction.Left:
					_x -= XSpeed;
					break;
			}
		}

		public void RollbackState()
		{
			_x = _oldX;
			_y = _oldY;
		}
	}
}