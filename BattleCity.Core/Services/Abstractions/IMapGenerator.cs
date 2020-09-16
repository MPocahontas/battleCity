using BattleCity.Core.Models;

namespace BattleCity.Core.Services.Abstractions
{
	public interface IMapGenerator
	{
		Map Generate();
	}
}