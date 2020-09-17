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
		private static readonly object MapLocker = new object();

		private readonly IMapPainter _painter;
		private readonly ICollisionDetector _collisionDetector;
		private readonly IActionResolver _actionResolver;
		private readonly Map _map;
		private readonly Timer _bonusGeneratorTimer;

		public GameEngine(
			IMapGenerator mapGenerator,
			IMapPainter painter,
			ICollisionDetector collisionDetector,
			IActionResolver actionResolver)
		{
			_painter = painter;
			_collisionDetector = collisionDetector;
			_actionResolver = actionResolver;
			_map = mapGenerator.Generate();
			_map.RespawnTank(Team.A);
			_map.RespawnTank(Team.B);
			_painter.Draw(_map);
			_actionResolver.Initialize(_map);
			_bonusGeneratorTimer = new Timer(GenerateBonusCallback, null, GetNextBonusGenerateTime(), Timeout.Infinite );
		}

		public void MoveTankA(Direction direction)
		{
			lock (MapLocker)
			{
				if (_map.TankA != null)
				{
					MoveTank(_map.TankA, direction);
				}
			}
		}

		public void ShootTankA()
		{
			lock (MapLocker)
			{
				var bullet = CreateBulletModel(_map.TankA);
				_actionResolver.Add(bullet);
				Task.Run(() => MoveBullet(bullet));
			}
		}

		public void MoveTankB(Direction direction)
		{
			lock (MapLocker)
			{
				if (_map.TankB != null)
				{
					MoveTank(_map.TankB, direction);
				}
			}
		}

		public void ShootTankB()
		{
			lock (MapLocker)
			{
				var bullet = CreateBulletModel(_map.TankB);
				_actionResolver.Add(bullet);
				Task.Run(() => MoveBullet(bullet));
			}
		}

		private Bullet CreateBulletModel(Tank tank)
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
				var bonus = _map.Bonuses.FirstOrDefault(_ => _.IntersectsWith(tank));
				if (bonus != null) 
					_actionResolver.Apply(tank, bonus);

				_painter.Redraw(tank);
			}
		}

		private void MoveBullet(Bullet bullet)
		{
			while (true)
			{
				lock (MapLocker)
				{
					if (bullet == null)
						break;

					bullet.Move();
					if (_collisionDetector.IsOutOfTheMap(bullet, Bullet.Width, Bullet.Height))
					{
						_actionResolver.Remove(bullet, Position.Old);
						break;
					}

					var brickWall = _map.BrickWalls.FirstOrDefault(_ => _.IntersectsWith(bullet));
					if (brickWall != null)
					{
						_actionResolver.Remove(bullet, Position.Old);
						_actionResolver.Remove(brickWall);
						break;
					}

					if (_map.ConcreteWalls.Any(_ => _.IntersectsWith(bullet)))
					{
						_actionResolver.Remove(bullet, Position.Old);
						break;
					}

					var otherBullet = _map.Bullets.FirstOrDefault(_ => _.IntersectsWith(bullet) && !_.Equals(bullet));
					if (otherBullet != null)
					{
						_actionResolver.Remove(bullet, Position.Old);
						_actionResolver.Remove(otherBullet, Position.Current);
						break;
					}

					if (_map.TankA != null && bullet.IntersectsWith(_map.TankA))
					{
						HitTank(_map.TankA, bullet);
						break;
					}

					if (_map.TankB != null && bullet.IntersectsWith(_map.TankB))
					{
						HitTank(_map.TankB, bullet);
						break;
					}

					if (bullet.IntersectsWith(_map.FlagA))
					{
						// game over
						break;
					}

					if (bullet.IntersectsWith(_map.FlagB))
					{
						// game over
						break;
					}

					if (!_map.Bullets.Any(_ => _.Equals(bullet)))
					{
						break;
					}

					_painter.Redraw(bullet);
				}

				Thread.Sleep(8000 / bullet.Speed);
			}
		}

		private void HitTank(Tank tank, Bullet bullet)
		{
			_actionResolver.Hit(tank);

			if (!tank.IsAlive)
			{
				RunRespawn(tank.Team);
			}

			_actionResolver.Remove(bullet, Position.Old);
		}

		private void RunRespawn(Team team)
		{
			Task.Delay(TimeSpan.FromSeconds(Constants.RespawnDeltaInSeconds)).ContinueWith(t =>
			{
				lock (MapLocker)
				{
					_actionResolver.RespawnTank(team);
				}
			});
		}

		private void GenerateBonusCallback(object state)
		{
			try
			{
				lock (MapLocker)
				{
					if (new Random().Next(0, 100) % 2 == 0)
					{
						var freeSpacePoint =
							_collisionDetector.GetFreeSpacePoint(ArmorBonus.Width, ArmorBonus.Height, _map);
						var bonus = new ArmorBonus(freeSpacePoint.X, freeSpacePoint.Y);
						_actionResolver.Add(bonus);
					}
					else
					{
						var freeSpacePoint =
							_collisionDetector.GetFreeSpacePoint(AttackBonus.Width, AttackBonus.Height, _map);
						var bonus = new AttackBonus(freeSpacePoint.X, freeSpacePoint.Y);
						_actionResolver.Add(bonus);
					}
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