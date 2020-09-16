using System.Collections.Generic;
using System.Drawing;
using BattleCity.Core.Models;
using BattleCity.Core.Services.Abstractions;

namespace BattleCity.Core.Services.Implementations
{
	public class SimpleMapGenerator : IMapGenerator
	{
		public Map Generate()
		{
			return new Map(
				new List<BrickWall> {new BrickWall(1, 1)},
				new List<ConcreteWall> {new ConcreteWall(11, 11)},
				new List<River> { new River(21, 21)},
				new Flag(0, 30),
				new Flag(Constants.MapWidth - Flag.Width - 1, 30),
				new Point(0, Constants.MapHeight / 2),
				new Point(Constants.MapWidth - Tank.Width - 1, Constants.MapHeight / 2));
		}
	}
}