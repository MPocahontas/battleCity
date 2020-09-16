using BattleCity.Core.Models.Base;

namespace BattleCity.Core.Models.Bonuses
{
	public class ArmorBonus : BaseMapObject, IBonus
	{
		private const int Width = 4;
		private const int Height = 2;

		public ArmorBonus(int x, int y) 
			: base(x, y, Width, Height) { }
	}
}