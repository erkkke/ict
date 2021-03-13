using System;

namespace Snake
{
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            while (game.IsRunning)
            {
                game.KeyPressed(Console.ReadKey(true));
            }
        }
    }
}
