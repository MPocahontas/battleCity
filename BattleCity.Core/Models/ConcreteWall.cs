using BattleCity.Core.Models.Base;

namespace BattleCity.Core.Models
{
	public class ConcreteWall : BaseMapObject
	{
		private const int Width = 6;
		private const int Height = 6;

		public ConcreteWall(int x, int y) 
			: base(x, y, Width, Height) { }
	}
}