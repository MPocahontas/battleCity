using System.Drawing;
using BattleCity.Core.Models;
using BattleCity.Core.Models.Base;

namespace BattleCity.Core.Services.Abstractions
{
	public interface IMapPainter
	{
		void Draw(Map map);

		void Draw(IBonus bonus);

		void Redraw(Bullet bullet);

		void RedrawTankA(Tank tank);

		void RedrawTankB(Tank tank);

		void Clear(Rectangle rectangle);
	}
}