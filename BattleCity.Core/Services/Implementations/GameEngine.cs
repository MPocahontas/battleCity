using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BattleCity.Core.Enums;
using BattleCity.Core.Models;
using BattleCity.Core.Services.Abstractions;

namespace BattleCity.Core.Services.Implementations
{
	public class GameEngine
	{
		private readonly IMapPainter _painter;
		private readonly ICollisionDetector _collisionDetector;
		private readonly Map _map;

		public GameEngine(
			IMapGenerator mapGenerator,
			IMapPainter painter,
			ICollisionDetector collisionDetector)
		{
			_painter = painter;
			_collisionDetector = collisionDetector;
			_map = mapGenerator.Generate();
			_map.RespawnTankA();
			_map.RespawnTankB();
			_painter.Draw(_map);
		}

		public void MoveTankA(Direction direction)
		{
			MoveTank(_map.TankA, direction);
		}

		public void ShootTankA()
		{
			var bullet = CreateBullet(_map.TankA);
			Task.Run(() => MoveBullet(bullet));
		}

		public void MoveTankB(Direction direction)
		{
			MoveTank(_map.TankB, direction);
		}

		public void ShootTankB()
		{

		}

		private Bullet CreateBullet(Tank tank)
		{
			switch (tank.GunDirection)
			{
				case Direction.Up:
					return new Bullet(_map.TankA.X + Tank.Width / 2, _map.TankA.Y, _map.TankA.GunDirection, Constants.DefaultBulletSpeed);
				case Direction.Right:
					return new Bullet(_map.TankA.X + Tank.Width, _map.TankA.Y + Tank.Width / 2, _map.TankA.GunDirection, Constants.DefaultBulletSpeed);
				case Direction.Down:
					return new Bullet(_map.TankA.X + Tank.Width / 2, _map.TankA.Y + Tank.Height, _map.TankA.GunDirection, Constants.DefaultBulletSpeed);
				case Direction.Left:
					return new Bullet(_map.TankA.X, _map.TankA.Y + Tank.Height / 2, _map.TankA.GunDirection, Constants.DefaultBulletSpeed);
				default:
					throw new ArgumentOutOfRangeException($"Invalid value {tank.GunDirection} for enum {nameof(Direction)}");
			}
		}

		private void MoveTank(Tank tank, Direction direction)
		{
			tank.Move(direction);
			if (_collisionDetector.IsDetected(tank, _map))
			{
				tank.RollbackState();
			}
			else
			{
				var bonuses = _map.Bonuses.Where(_ => _.GetRectangle().IntersectsWith(tank.GetRectangle()));
				foreach (var bonus in bonuses)
				{
					
				}
				_painter.Redraw(tank, _map);
			}
		}

		private void MoveBullet(Bullet bullet)
		{
			while (true)
			{
				bullet.Move();
				if (_collisionDetector.IsOutOfTheMap(bullet, Bullet.Width, Bullet.Height))
				{
					_painter.Clear(bullet.GetOldRectangle());
					break;
				}

				if (_map.ConcreteWalls.Any(_ => _.GetRectangle().IntersectsWith(bullet.GetRectangle())))
				{
					_painter.Clear(bullet.GetOldRectangle());
					break;
				}

				_painter.Redraw(bullet);

				Thread.Sleep(8000 / bullet.Speed);
			}
		}

	}
}