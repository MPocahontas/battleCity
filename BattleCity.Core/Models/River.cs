using BattleCity.Core.Models.Geometry;

namespace BattleCity.Core.Models
{
	public class River
	{
		public Point Point { get; set; }

		public River(int x, int y)
		{
			Point = new Point(x, y);
		}
	}
}