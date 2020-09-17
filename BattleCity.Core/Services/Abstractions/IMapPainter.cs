using System.Drawing;
using BattleCity.Core.Models;
using BattleCity.Core.Models.Base;

namespace BattleCity.Core.Services.Abstractions
{
	public interface IMapPainter
	{
		void Draw(Map map);

		void Draw(IBonus bonus);

		void Draw(Bullet bullet);

		void Redraw(Bullet bullet);

		void Redraw(Tank tank);

		void Clear(Rectangle rectangle);
	}
}