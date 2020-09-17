using BattleCity.Core.Models.Base;

namespace BattleCity.Core.Models.Bonuses
{
	/// <summary>
	/// Bonus that gives extra speed to a tank
	/// </summary>
	public class AttackBonus: BaseMapObject, IBonus
	{
		public const int Width = 4;
		public const int Height = 2;

		public AttackBonus(int x, int y) 
			: base(x, y, Width, Height) { }
	}
}