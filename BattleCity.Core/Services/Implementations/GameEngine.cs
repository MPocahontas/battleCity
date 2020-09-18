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
		/// <summary>
		/// Lock for safety work with map
		/// </summary>
		private static readonly object MapLocker = new object();

		private readonly IMapPainter _painter;
		private readonly IMapAnalyzer _mapAnalyzer;
		private readonly IActionResolver _actionResolver;
		private readonly Map _map;
		private readonly Timer _bonusGeneratorTimer;

		private bool _isGameOver = false;

		public GameEngine(
			IMapGenerator mapGenerator,
			IMapPainter painter,
			IMapAnalyzer mapAnalyzer,
			IActionResolver actionResolver)
		{
			_painter = painter;
			_mapAnalyzer = mapAnalyzer;
			_actionResolver = actionResolver;

			// generate map and respawn 2 tanks
			_map = mapGenerator.Generate();
			_map.RespawnTank(Team.A);
			_map.RespawnTank(Team.B);

			_painter.Draw(_map);
			_actionResolver.Initialize(_map);

			// run timer which will generate bonuses according to the given rules
			_bonusGeneratorTimer = new Timer(GenerateBonusCallback, null, GetNextBonusGenerateTime(), Timeout.Infinite );

		}

		public void MoveTankA(Direction direction)
		{
			lock (MapLocker)
			{
				if (!_isGameOver && _map.TankA != null)
				{
					MoveTank(_map.TankA, direction);
				}
			}
		}

		public void ShootTankA()
		{
			lock (MapLocker)
			{
				if (!_isGameOver)
				{
					Shoot(_map.TankA);
				}
			}
		}

		public void MoveTankB(Direction direction)
		{
			lock (MapLocker)
			{
				if (!_isGameOver && _map.TankB != null)
				{
					MoveTank(_map.TankB, direction);
				}
			}
		}

		public void ShootTankB()
		{
			lock (MapLocker)
			{
				if (!_isGameOver)
				{
					Shoot(_map.TankB);
				}
			}
		}

		private void Shoot(Tank tank)
		{
			var bullet = CreateBulletModel(tank);

			if (!_mapAnalyzer.IsOutOfTheMapBorders(bullet, Bullet.Width, Bullet.Height))
			{
				_actionResolver.Add(bullet);

				// start bullet movement in separate thread 
				Task.Factory.StartNew(() => MoveBullet(bullet), TaskCreationOptions.LongRunning);
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

			// if potential next tank move causes collisions - rollback
			if (_mapAnalyzer.IsCollisionDetected(tank, _map))
			{
				tank.RollbackState();
			}
			else
			{
				// looking for bonus that we could intersect
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
					if (_isGameOver)
						break;

					bullet.Move();

					// stop bullet if we it out of the map
					if (_mapAnalyzer.IsOutOfTheMapBorders(bullet, Bullet.Width, Bullet.Height))
					{
						_actionResolver.Remove(bullet, Position.Old);
						break;
					}

					// looking for brick wall that we could intersect
					var brickWall = _map.BrickWalls.FirstOrDefault(_ => _.IntersectsWith(bullet));
					if (brickWall != null)
					{
						// hit and check state
						brickWall.Hit();
						if (!brickWall.IsAlive) 
							_actionResolver.Remove(brickWall);

						_actionResolver.Remove(bullet, Position.Old);
						break;
					}

					// if bullet intersects concrete wall - stop bullet
					if (_map.ConcreteWalls.Any(_ => _.IntersectsWith(bullet)))
					{
						_actionResolver.Remove(bullet, Position.Old);
						break;
					}

					// if bullet intersects other bullet - stop both
					var otherBullet = _map.Bullets.FirstOrDefault(_ => _.IntersectsWith(bullet) && !_.Equals(bullet));
					if (otherBullet != null)
					{
						_actionResolver.Remove(bullet, Position.Old);
						_actionResolver.Remove(otherBullet, Position.Current);
						break;
					}

					// if bullet intersects any tank - hit
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

					// if bullet intersects any flag - game over
					if (bullet.IntersectsWith(_map.FlagA))
					{
						_isGameOver = true;
						_bonusGeneratorTimer.Dispose();
						_painter.DrawWinMessage(Team.B);
						break;
					}

					if (bullet.IntersectsWith(_map.FlagB))
					{
						_isGameOver = true;
						_bonusGeneratorTimer.Dispose();
						_painter.DrawWinMessage(Team.A);
						break;
					}

					// in case another bullet stopped this
					if (!_map.Bullets.Any(_ => _.Equals(bullet)))
					{
						break;
					}

					_painter.Redraw(bullet);
				}

				Thread.Sleep(Constants.BulletMoveDelay / bullet.Speed);
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

		/// <summary>
		/// Creates tank and after a delay prints it on the map
		/// </summary>
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

		// Callback for periodic bonus generation
		private void GenerateBonusCallback(object state)
		{
			try
			{
				lock (MapLocker)
				{
					// in case of random 0 generate Armor Bonus in any free point on the map
					// in case of random 1 - Attack bonus
					if (new Random().Next(0, 100) % 2 == 0)
					{
						var freeSpacePoint =
							_mapAnalyzer.GetFreeSpacePoint(ArmorBonus.Width, ArmorBonus.Height, _map);
						var bonus = new ArmorBonus(freeSpacePoint.X, freeSpacePoint.Y);
						_actionResolver.Add(bonus);
					}
					else
					{
						var freeSpacePoint =
							_mapAnalyzer.GetFreeSpacePoint(AttackBonus.Width, AttackBonus.Height, _map);
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