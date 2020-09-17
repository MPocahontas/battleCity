using BattleCity.Core.Models.Base;

namespace BattleCity.Core.Models.Bonuses
{
	/// <summary>
	/// Bonus that gives armor to a tank
	/// </summary>
	public class ArmorBonus : BaseMapObject, IBonus
	{
		public const int Width = 4;
		public const int Height = 2;

		public ArmorBonus(int x, int y) 
			: base(x, y, Width, Height) { }
	}
}