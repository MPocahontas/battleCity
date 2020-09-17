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
			if (_map.TankA != null)
			{
				MoveTank(_map.TankA, direction, Team.A);
			}
		}

		public void ShootTankA()
		{
			var bullet = CreateBullet(_map.TankA);
			_map.Add(bullet);
			Task.Run(() => MoveBullet(bullet));
		}

		public void MoveTankB(Direction direction)
		{
			if (_map.TankB != null)
			{
				MoveTank(_map.TankB, direction, Team.B);
			}
		}

		public void ShootTankB()
		{
			var bullet = CreateBullet(_map.TankB);
			_map.Add(bullet);
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

		private void MoveTank(Tank tank, Direction direction, Team team)
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

				_painter.Redraw(tank, team);
			}
		}

		private void MoveBullet(Bullet bullet)
		{
			while (true)
			{
				lock (MapLocker)
				{
					bullet.Move();
					if (_collisionDetector.IsOutOfTheMap(bullet, Bullet.Width, Bullet.Height))
					{
						_map.Remove(bullet);
						_painter.Clear(bullet.GetOldRectangle());
						break;
					}

					var brickWall =
						_map.BrickWalls.FirstOrDefault(_ => _.GetRectangle().IntersectsWith(bullet.GetRectangle()));
					if (brickWall != null)
					{
						_actionResolver.Remove(bullet, Position.Old);
						_actionResolver.Remove(brickWall);
						break;
					}

					if (_map.ConcreteWalls.Any(_ => _.GetRectangle().IntersectsWith(bullet.GetRectangle())))
					{
						_actionResolver.Remove(bullet, Position.Old);
						break;
					}

					var otherBullet = _map.Bullets.FirstOrDefault(_ => _.GetRectangle().IntersectsWith(bullet.GetRectangle()) && !_.Equals(bullet));
					if (otherBullet != null)
					{
						_actionResolver.Remove(bullet, Position.Old);
						_actionResolver.Remove(bullet, Position.Current);
						break;
					}


					if (_map.TankA != null && bullet.GetRectangle().IntersectsWith(_map.TankA.GetRectangle()))
					{
						HitTank(_map.TankA, Team.A, bullet);
						break;
					}

					if (_map.TankB != null && bullet.GetRectangle().IntersectsWith(_map.TankB.GetRectangle()))
					{
						HitTank(_map.TankB, Team.B, bullet);
						break;
					}

					if (bullet.GetRectangle().IntersectsWith(_map.FlagA.GetRectangle()))
					{
						// game over
						break;
					}

					if (bullet.GetRectangle().IntersectsWith(_map.FlagB.GetRectangle()))
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

		private void HitTank(Tank tank, Team team, Bullet bullet)
		{
			_actionResolver.Hit(tank, team);

			if (tank == null)
			{
				RunRespawn(team);
			}

			_actionResolver.Remove(bullet, Position.Old);
		}

		private void RunRespawn(Team team)
		{
			Task.Delay(TimeSpan.FromSeconds(2)).ContinueWith(t =>
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