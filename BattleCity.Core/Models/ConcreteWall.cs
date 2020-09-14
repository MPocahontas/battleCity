using BattleCity.Core.Models.Geometry;

namespace BattleCity.Core.Models
{
	public class ConcreteWall
	{
		public Point Point { get; set; }

		public ConcreteWall(int x, int y)
		{
			Point = new Point(x, y);
		}
	}
}