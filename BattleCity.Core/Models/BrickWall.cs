using BattleCity.Core.Models.Base;

namespace BattleCity.Core.Models
{
	public class BrickWall : BaseMapObject
	{
		private const int StartArmor = 1;
		private const int Width = 4;
		private const int Height = 2;

		private int _armor = StartArmor;

		public bool IsAlive { get; private set; } = true;

		public BrickWall(int x, int y) 
			: base(x, y, Width, Height) { }

		public void Hit()
		{
			_armor--;

			if (_armor < default(int))
				IsAlive = false;
		}
	}
}