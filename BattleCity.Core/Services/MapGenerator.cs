using System.Collections.Generic;
using BattleCity.Core.Models;

namespace BattleCity.Core.Services
{
	public class MapGenerator
	{
		public Map Generate()
		{
			return new Map(
				new List<BrickWall> {new BrickWall(1, 1)},
				new List<ConcreteWall> {new ConcreteWall(2, 2)},
				new List<River> { new River(5, 5)});
		}
	}
}