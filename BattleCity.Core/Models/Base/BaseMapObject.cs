using System.Drawing;

namespace BattleCity.Core.Models.Base
{
	public abstract class BaseMapObject : IDrawable
	{
		private readonly int _width;
		private readonly int _height;

		public int X { get; protected set; }

		public int Y { get; protected set; }
		
		protected BaseMapObject(int x, int y, int width, int height)
		{
			X = x;
			Y = y;
			_width = width;
			_height = height;
		}

		public Rectangle GetRectangle() 
			=> new Rectangle(X, Y, _width, _height);

		public bool IntersectsWith(BaseMapObject obj)
			=> GetRectangle().IntersectsWith(obj.GetRectangle());
	}
}