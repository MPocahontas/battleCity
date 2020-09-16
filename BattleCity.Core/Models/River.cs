using BattleCity.Core.Models.Base;

namespace BattleCity.Core.Models
{
	public class River : BaseMapObject
	{
		private const int Width = 4;
		private const int Height = 2;

		public River(int x, int y) 
			: base(x, y, Width, Height) { }
	}
}