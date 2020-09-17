using System.Drawing;

namespace BattleCity.Core.Models.Base
{
	public interface IDrawable
	{
		Rectangle GetRectangle();

		bool IntersectsWith(BaseMapObject obj);
	}
}