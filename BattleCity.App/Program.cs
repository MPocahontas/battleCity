using System;
using System.Threading;
using BattleCity.Core;
using BattleCity.Core.Enums;
using BattleCity.Core.Services.Abstractions;
using BattleCity.Core.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace BattleCity.App
{
	class Program
	{
		private const int ConsoleExtraPart = 2;

		static void Main(string[] args)
		{
			Console.SetWindowSize(Constants.MapWidth + ConsoleExtraPart, Constants.MapHeight + ConsoleExtraPart);

			var serviceProvider = new ServiceCollection()
				.AddSingleton<IMapGenerator, SimpleMapGenerator>()
				.AddSingleton<IMapPainter, ConsoleMapPainter>()
				.AddSingleton<IMapAnalyzer, MapAnalyzer>()
				.AddSingleton<IActionResolver, ActionResolver>()
				.AddSingleton<GameEngine>()
				.BuildServiceProvider();

			var gameEngine = serviceProvider.GetService<GameEngine>();
			
			Run(gameEngine);
		}

		private static void Run(GameEngine gameEngine)
		{
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
					case ConsoleKey.Enter:
						gameEngine.ShootTankB();
						break;
					case ConsoleKey.W:
						gameEngine.MoveTankA(Direction.Up);
						break;
					case ConsoleKey.D:
						gameEngine.MoveTankA(Direction.Right);
						break;
					case ConsoleKey.S:
						gameEngine.MoveTankA(Direction.Down);
						break;
					case ConsoleKey.A:
						gameEngine.MoveTankA(Direction.Left);
						break;
					case ConsoleKey.Spacebar:
						gameEngine.ShootTankA();
						break;
				}
			}
			while (keyInfo.Key != ConsoleKey.Escape);
		}
	}
}
