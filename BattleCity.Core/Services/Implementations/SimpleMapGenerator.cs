using System.Collections.Generic;
using System.Drawing;
using BattleCity.Core.Models;
using BattleCity.Core.Services.Abstractions;

namespace BattleCity.Core.Services.Implementations
{
	/// <summary>
	/// Class responsible for map generation
	/// </summary>
	public class SimpleMapGenerator : IMapGenerator
	{
		public Map Generate()
		{
			return new Map(
				new List<BrickWall>
				{
					new BrickWall(0, 22),
					new BrickWall(5, 22),
					new BrickWall(5, 25),
					new BrickWall(5, 28),
					new BrickWall(0, 28),
					new BrickWall(Constants.MapWidth - 5, 22),
					new BrickWall(Constants.MapWidth - 10, 22),
					new BrickWall(Constants.MapWidth - 10, 25),
					new BrickWall(Constants.MapWidth - 10, 28),
					new BrickWall(Constants.MapWidth - 5, 28),
				},
				new List<ConcreteWall>
				{
					new ConcreteWall(11, 11),
					new ConcreteWall(15, 11),
					new ConcreteWall(11, 13),
					new ConcreteWall(15, 13),
					new ConcreteWall(71, 11),
					new ConcreteWall(75, 11),
					new ConcreteWall(71, 13),
					new ConcreteWall(75, 13),
					new ConcreteWall(11, 31),
					new ConcreteWall(15, 31),
					new ConcreteWall(11, 33),
					new ConcreteWall(15, 33),
					new ConcreteWall(71, 31),
					new ConcreteWall(75, 31),
					new ConcreteWall(71, 33),
					new ConcreteWall(75, 33)


				},
				new List<River>
				{
					new River(48, 24),
					new River(48, 26),
					new River(48, 28),
					new River(48, 30),
					new River(45, 24),
					new River(45, 26),
					new River(45, 28),
					new River(45, 30),
				},
				new Flag(0, 25),
				new Flag(Constants.MapWidth - 5, 25),
				new Point(0, Constants.MapHeight / 2 + 10),
				new Point(Constants.MapWidth - Tank.Width - 1, Constants.MapHeight / 2 + 10));
		}
	}
}