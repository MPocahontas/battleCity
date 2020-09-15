using System;
using BattleCity.Core;
using BattleCity.Core.Services.Implementations;

namespace BattleCity.App
{
	class Program
	{
		private const int ConsoleExtraPart = 2;

		static void Main(string[] args)
		{
			Console.SetWindowSize(Constants.MapWidth + ConsoleExtraPart, Constants.MapHeight + ConsoleExtraPart);
			var mapGenerator = new SimpleMapGenerator();
			var map = mapGenerator.Generate();
			var painter = new ConsoleMapPainter();
			painter.Paint(map);
			//Console.WriteLine("Press ESC to stop");
			ConsoleKeyInfo keyInfo;
			do
			{
				keyInfo = Console.ReadKey(true);

				switch (keyInfo.Key)
				{
					case ConsoleKey.UpArrow:
						Console.WriteLine("Up");
						break;
					case ConsoleKey.RightArrow:
						Console.WriteLine("Right");
						break;
					case ConsoleKey.DownArrow:
						Console.WriteLine("Down");
						break;
					case ConsoleKey.LeftArrow:
						Console.WriteLine("Left");
						break;
				}
			} 
			while (keyInfo.Key != ConsoleKey.Escape);
		}
	}
}
