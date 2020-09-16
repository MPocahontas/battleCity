using BattleCity.Core.Models.Base;

namespace BattleCity.Core.Models.Bonuses
{
	public class AttackBonus: BaseMapObject, IBonus
	{
		private const int Width = 4;
		private const int Height = 2;

		public AttackBonus(int x, int y) 
			: base(x, y, Width, Height) { }
	}
}