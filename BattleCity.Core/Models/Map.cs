using System.Collections.Generic;
using BattleCity.Core.Enums;

namespace BattleCity.Core.Models
{
	public class Map
	{
		public Tank TankA { get; private set; }

		public Tank TankB { get; private set; }

		public IEnumerable<BrickWall> BrickWalls { get; private set; }

		public IEnumerable<ConcreteWall> ConcreteWalls { get; private set; }

		public IEnumerable<River> Rivers { get; private set; }

		public Map(IEnumerable<BrickWall> brickWalls, IEnumerable<ConcreteWall> concreteWalls, IEnumerable<River> rivers)
		{
			BrickWalls = brickWalls;
			ConcreteWalls = concreteWalls;
			Rivers = rivers;

			TankA = new Tank(0, Constants.MapHeight / 2, Direction.Right);
			TankB = new Tank(0, Constants.MapHeight / 2, Direction.Left);
		}
	}
}