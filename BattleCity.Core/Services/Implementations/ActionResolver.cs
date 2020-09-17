using System.Security.Cryptography.X509Certificates;
using BattleCity.Core.Enums;
using BattleCity.Core.Models;
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

		public void Hit(Tank tank, Team team)
		{
			tank.Hit();
			if (!tank.IsAlive)
			{
				_map.KillTank(team);
				_painter.Clear(tank.GetRectangle());
			}
		}

		public void RespawnTank(Team team)
		{
			_map.RespawnTank(team);
			if (team == Team.A)
				_painter.Redraw(_map.TankA, team);
			else if (team == Team.B) 
				_painter.Redraw(_map.TankB, team);
		}
	}
}