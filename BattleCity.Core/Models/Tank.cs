using BattleCity.Core.Enums;
namespace BattleCity.Core.Models
{
	public class Tank
	{
		private Direction _gunDirection;
		private int _x;
		private int _y;

		public Tank(int x, int y, Direction gunDirection)
		{
			_x = x;
			_y = y;
			_gunDirection = gunDirection;
		}
	}
}