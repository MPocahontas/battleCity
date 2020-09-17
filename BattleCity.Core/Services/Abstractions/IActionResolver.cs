using BattleCity.Core.Enums;
using BattleCity.Core.Models;
using BattleCity.Core.Models.Base;

namespace BattleCity.Core.Services.Abstractions
{
	public interface IActionResolver
	{
		void Initialize(Map map);

		void Add(IBonus bonus);

		void Remove(Bullet bullet, Position position);

		void Remove(BrickWall brickWall);

		void Hit(Tank tank);

		void RespawnTank(Team team);
	}
}