using System;

namespace BattleCity.App
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Press ESC to stop");
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
