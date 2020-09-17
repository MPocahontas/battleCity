using BattleCity.Core.Enums;
using BattleCity.Core.Models;

namespace BattleCity.Core.Services.Abstractions
{
	public interface IActionResolver
	{
		void Initialize(Map map);

		void Remove(Bullet bullet, Position position);

		void Remove(BrickWall brickWall);

		void Hit(Tank tank, Team team);

		void RespawnTank(Team team);
	}
}