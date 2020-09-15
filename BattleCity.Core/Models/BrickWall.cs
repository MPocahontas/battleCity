using BattleCity.Core.Models.Base;

namespace BattleCity.Core.Models
{
	public class BrickWall : BaseMapObject
	{
		private const int Width = 6;
		private const int Height = 6;

		public BrickWall(int x, int y) 
			: base(x, y, Width, Height) { }
	}
}