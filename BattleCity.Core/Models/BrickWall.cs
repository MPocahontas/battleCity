using BattleCity.Core.Models.Geometry;

namespace BattleCity.Core.Models
{
	public class BrickWall
	{
		public Point Point { get; set; }

		public BrickWall(int x, int y)
		{
			Point = new Point(x, y);
		}
	}
}