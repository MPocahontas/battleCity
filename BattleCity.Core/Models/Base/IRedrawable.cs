using System.Drawing;

namespace BattleCity.Core.Models.Base
{
	public interface IRedrawable
	{
		Rectangle GetOldRectangle();
	}
}