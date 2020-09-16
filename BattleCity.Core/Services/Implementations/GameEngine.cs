using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BattleCity.Core.Enums;
using BattleCity.Core.Models;
using BattleCity.Core.Models.Bonuses;
using BattleCity.Core.Services.Abstractions;

namespace BattleCity.Core.Services.Implementations
{
	public class GameEngine : IDisposable
	{
		private readonly IMapPainter _painter;
		private readonly ICollisionDetector _collisionDetector;
		private readonly Map _map;
		private readonly Timer _bonusGeneratorTimer;

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
			_bonusGeneratorTimer = new Timer(GenerateBonusCallback, null, GetNextBonusGenerateTime(), Timeout.Infinite );
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
			var bullet = CreateBullet(_map.TankB);
			Task.Run(() => MoveBullet(bullet));
		}

		private Bullet CreateBullet(Tank tank)
		{
			switch (tank.GunDirection)
			{
				case Direction.Up:
					return new Bullet(tank.X + Tank.Width / 2, tank.Y - 2, tank.GunDirection, tank.BulletSpeed);
				case Direction.Right:
					return new Bullet(tank.X + Tank.Width + 1, tank.Y + Tank.Width / 2, tank.GunDirection, tank.BulletSpeed);
				case Direction.Down:
					return new Bullet(tank.X + Tank.Width / 2, tank.Y + Tank.Height + 1, tank.GunDirection, tank.BulletSpeed);
				case Direction.Left:
					return new Bullet(tank.X - 2, tank.Y + Tank.Height / 2, tank.GunDirection, tank.BulletSpeed);
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
					tank.Apply(bonus);
					_painter.Clear(bonus.GetRectangle());
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

		private void GenerateBonusCallback(object state)
		{
			try
			{
				if (new Random().Next(0, 100) % 2 == 0)
				{
					var freeSpacePoint =
						_collisionDetector.GetFreeSpacePoint(ArmorBonus.Width, ArmorBonus.Height, _map);
					var bonus = new ArmorBonus(freeSpacePoint.X, freeSpacePoint.Y);
					_map.Add(bonus);
					_painter.Draw(bonus);
				}
				else
				{
					var freeSpacePoint =
						_collisionDetector.GetFreeSpacePoint(AttackBonus.Width, AttackBonus.Height, _map);
					var bonus = new AttackBonus(freeSpacePoint.X, freeSpacePoint.Y);
					_map.Add(bonus);
					_painter.Draw(bonus);
				}
			}
			finally 
			{
				_bonusGeneratorTimer.Change(GetNextBonusGenerateTime(), Timeout.Infinite);
			}
		}

		private int GetNextBonusGenerateTime() 
			=> new Random().Next(
				Constants.MinimumNextBonusGenerateTimeInSeconds * 1000,
				(Constants.MinimumNextBonusGenerateTimeInSeconds + Constants.NextBonusGenerateTimeDeltaInSeconds) * 1000);

		public void Dispose()
		{
			_bonusGeneratorTimer?.Dispose();
		}
	}
}