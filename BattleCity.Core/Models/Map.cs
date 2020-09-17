using System.Collections.Generic;
using System.Drawing;
using BattleCity.Core.Enums;
using BattleCity.Core.Models.Base;

namespace BattleCity.Core.Models
{
	/// <summary>
	/// Model for storing all objects in the game
	/// </summary>
	public class Map
	{
		private readonly Point _respawnA;

		private readonly Point _respawnB;

		public Tank TankA { get; private set; }

		public Tank TankB { get; private set; }

		public Flag FlagA { get; }

		public Flag FlagB { get; }

		public IList<BrickWall> BrickWalls { get; }

		public IList<ConcreteWall> ConcreteWalls { get; }

		public IList<River> Rivers { get; }

		public IList<Bullet> Bullets { get; }

		public IList<IBonus> Bonuses { get; set; }

		public Map(
			IList<BrickWall> brickWalls,
			IList<ConcreteWall> concreteWalls,
			IList<River> rivers,
			Flag flagA,
			Flag flagB,
			Point respawnA,
			Point respawnB)
		{
			BrickWalls = brickWalls;
			ConcreteWalls = concreteWalls;
			Rivers = rivers;
			Bullets = new List<Bullet>();
			Bonuses = new List<IBonus>();

			FlagA = flagA;
			FlagB = flagB;
			_respawnA = respawnA;
			_respawnB = respawnB;
		}

		public void Add(Bullet bullet)
		{
			Bullets.Add(bullet);
		}

		public void Add(IBonus bonus)
		{
			Bonuses.Add(bonus);
		}

		public void Remove(BrickWall brickWall)
		{
			BrickWalls.Remove(brickWall);
		}

		public void Remove(IBonus bonus)
		{
			Bonuses.Remove(bonus);
		}

		public void Remove(Bullet bullet)
		{
			Bullets.Remove(bullet);
		}

		public void KillTank(Team team)
		{
			if (team == Team.A)
				TankA = null;
			else if (team == Team.B) 
				TankB = null;
		}

		public void RespawnTank(Team team)
		{
			if (team == Team.A)
				TankA = new Tank(_respawnA.X, _respawnA.Y, Direction.Right, Team.A);
			else if (team == Team.B) 
				TankB = new Tank(_respawnB.X, _respawnB.Y, Direction.Left, Team.B);
		}
	}
}