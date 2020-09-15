using System.Collections.Generic;
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
				new List<River> { new River(21, 21)});
		}
	}
}