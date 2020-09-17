using BattleCity.Core.Enums;
using BattleCity.Core.Models;
using BattleCity.Core.Models.Base;
using BattleCity.Core.Services.Abstractions;

namespace BattleCity.Core.Services.Implementations
{
	public class ActionResolver : IActionResolver
	{
		private readonly IMapPainter _painter;
		private Map _map;

		public ActionResolver(IMapPainter painter)
		{
			_painter = painter;
		}

		public void Initialize(Map map)
			=> _map = map;

		public void Add(IBonus bonus)
		{
			_map.Add(bonus);
			_painter.Draw(bonus);
		}

		public void Add(Bullet bullet)
		{
			_map.Add(bullet);
			_painter.Draw(bullet);
		}

		public void Apply(Tank tank, IBonus bonus)
		{
			tank.Apply(bonus);
			_map.Remove(bonus);
			_painter.Clear(bonus.GetRectangle());
		}

		public void Remove(Bullet bullet, Position position)
		{
			_map.Remove(bullet);
			
			if (position == Position.Old)
				_painter.Clear(bullet.GetOldRectangle());
			else if (position == Position.Current) 
				_painter.Clear(bullet.GetRectangle());
		}

		public void Remove(BrickWall brickWall)
		{
			_map.Remove(brickWall);
			_painter.Clear(brickWall.GetRectangle());
		}

		public void Hit(Tank tank)
		{
			tank.Hit();
			if (!tank.IsAlive)
			{
				_map.KillTank(tank.Team);
				_painter.Clear(tank.GetRectangle());
			}
		}

		public void RespawnTank(Team team)
		{
			_map.RespawnTank(team);
			if (team == Team.A)
				_painter.Redraw(_map.TankA);
			else if (team == Team.B) 
				_painter.Redraw(_map.TankB);
		}
	}
}