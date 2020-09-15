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
			_painter.Paint(_map);
		}

		public void MoveTankA(Direction direction)
		{
			MoveTank(_map.TankA, direction);
		}

		public void ShootTankA()
		{

		}

		public void MoveTankB(Direction direction)
		{
			MoveTank(_map.TankB, direction);
		}

		public void ShootTankB()
		{

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
				_painter.Paint(_map);
			}
		}

	}
}