using BattleCity.Core.Models.Base;

namespace BattleCity.Core.Models
{
	public class Flag : BaseMapObject
	{
		public const int Width = 4;
		public const int Height = 2;

		public Flag(int x, int y) 
			: base(x, y, Width, Height) { }
	}
}