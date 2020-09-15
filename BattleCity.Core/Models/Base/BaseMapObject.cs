using System;
using System.Drawing;

namespace BattleCity.Core.Models.Base
{
	public abstract class BaseMapObject : IEquatable<BaseMapObject> 
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

		public bool Equals(BaseMapObject other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			if (other.GetType() != this.GetType()) return false;
			return X == other.X && Y == other.Y;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((BaseMapObject) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return X.GetHashCode() ^ Y.GetHashCode();
			}
		}
	}
}