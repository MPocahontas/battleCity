using System;
using BattleCity.Core;
using BattleCity.Core.Enums;
using BattleCity.Core.Services.Implementations;

namespace BattleCity.App
{
	class Program
	{
		private const int ConsoleExtraPart = 2;

		static void Main(string[] args)
		{
			Console.SetWindowSize(Constants.MapWidth + ConsoleExtraPart, Constants.MapHeight + ConsoleExtraPart);

			var gameEngine = new GameEngine(new SimpleMapGenerator(), new ConsoleMapPainter(), new CollisionDetector());
			
			ConsoleKeyInfo keyInfo;
			do
			{
				keyInfo = Console.ReadKey(true);

				switch (keyInfo.Key)
				{
					case ConsoleKey.UpArrow:
						gameEngine.MoveTankB(Direction.Up);
						break;
					case ConsoleKey.RightArrow:
						gameEngine.MoveTankB(Direction.Right);
						break;
					case ConsoleKey.DownArrow:
						gameEngine.MoveTankB(Direction.Down);
						break;
					case ConsoleKey.LeftArrow:
						gameEngine.MoveTankB(Direction.Left);
						break;
				}
			} 
			while (keyInfo.Key != ConsoleKey.Escape);
		}
	}
}
