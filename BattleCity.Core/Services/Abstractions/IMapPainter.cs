using System.Drawing;
using BattleCity.Core.Models;

namespace BattleCity.Core.Services.Abstractions
{
	public interface IMapPainter
	{
		void Paint(Map map);

		void Redraw(Bullet bullet);

		void Redraw(Tank tank, Map map);

		void Clear(Rectangle rectangle);
	}
}