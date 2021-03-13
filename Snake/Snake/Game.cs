using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace Snake
{
    class Game
    {
        Timer wormTimer = new Timer(120);
        Timer scoreTimer = new Timer(1000);
        int time = 0;
        bool isGameOver = false;
        bool isWon = false;
        public static int Width { get { return 40; } }
        public static int Height { get { return 40; } }
        static List<string> levels = new List<string>() { @"Levels/1.txt", @"Levels/2.txt", @"Levels/3.txt" };
        int level = 0;

        public Worm worm;
        public Wall wall;
        public Food food;

        public bool IsRunning { get; set; }
        Direction direction = Direction.UP;

        public Game()
        {
            worm = new Worm('@', ConsoleColor.Green);
            wall = new Wall('#', ConsoleColor.DarkYellow, levels[level]);
            food = new Food('$', ConsoleColor.Yellow, worm.body, wall.body);

            scoreTimer.Elapsed += GameTimer_Elapsed;
            scoreTimer.Start();
            wormTimer.Elapsed += Main;
            wormTimer.Start();

            IsRunning = true;
            Console.CursorVisible = false;
            Console.SetWindowSize(Height, Width);
            Console.SetBufferSize(Height, Width);
        }

        private void GameTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            time += 1;
        }

        bool CheckCollisionWormWithFood()
        {
            return worm.body[0].X == food.body[0].X && worm.body[0].Y == food.body[0].Y;
        }

        bool CheckCollisionWormWithWall()
        {
            foreach (Point i in wall.body)
            {
                if (i.X == worm.body[0].X && i.Y == worm.body[0].Y)
                    return true;
            }
            return false;
        }

        bool CheckWormCollision()
        {
            if (worm.body.Count > 1)
                for (int i = 1; i < worm.body.Count; i++)
                    if (worm.body[0].X == worm.body[i].X && worm.body[0].Y == worm.body[i].Y)
                        return true;
            return false;
        }

   

        void Main(object sender, ElapsedEventArgs e)
        {
            DisplayStatusBar();
            worm.Move();
            if (CheckCollisionWormWithFood())
            {
                worm.Increase(worm.body[0]);
                food.Generate(worm.body, wall.body);
            }
            else if (CheckCollisionWormWithWall())
            {
                isGameOver = true;
                isWon = false;
            }
            else if (CheckWormCollision())
            {
                isGameOver = true;
                isWon = false;
            }
            newLevel();
            GameOverScene();

        }

        void DisplayStatusBar()
        {
            Console.SetCursorPosition(2, 1);
            Console.Write($"Score: {worm.body.Count - 1}");
            Console.SetCursorPosition(2, 2);
            Console.WriteLine($"Time: {time}sec");
        }

        void newLevel()
        {
            if (worm.body.Count == 4 && level < 2)
            {
                time = 0;
                level++;
                Console.Clear();
                worm = new Worm('@', ConsoleColor.Green);
                wall = new Wall('#', ConsoleColor.DarkYellow, levels[level]);
                food = new Food('$', ConsoleColor.Yellow, worm.body, wall.body);
            }
            else if (worm.body.Count > 4)
            {
                isGameOver = true;
                isWon = true;
            }  
        }

        void restartGame()
        {
            Console.Clear();
            level = 0;
            time = 0;
            isGameOver = false;
            worm = new Worm('@', ConsoleColor.Green);
            wall = new Wall('#', ConsoleColor.DarkYellow, levels[level]);
            food = new Food('$', ConsoleColor.Yellow, worm.body, wall.body);
            wormTimer.Start();
        }

        void GameOverScene()
        {
            if (isGameOver)
            {
                if (isWon)
                {
                    wormTimer.Stop();
                    Console.Clear();
                    Console.SetCursorPosition(16, 18);
                    Console.Write("You WON!");
                    Console.SetCursorPosition(11, 19);
                    Console.Write("Press R to restart");
                }
                else
                {
                    wormTimer.Stop();
                    Console.Clear();
                    Console.SetCursorPosition(16, 18);
                    Console.Write("You LOST!");
                    Console.SetCursorPosition(11, 19);
                    Console.Write("Press R to restart");
                }
            }
        }

        void SaveData()
        {
            worm.Save("worm");
        }

        void LoadData()
        {
            wormTimer.Stop();
            Console.Clear();
            wall = new Wall('#', ConsoleColor.DarkYellow, levels[level]);
            worm = Worm.Load("worm");
            food = new Food('$', ConsoleColor.Yellow, worm.body, wall.body);
            wormTimer.Start();
        }
        public void KeyPressed(ConsoleKeyInfo pressedKey)
        {
            switch (pressedKey.Key)
            {
                case ConsoleKey.UpArrow:
                    if (!direction.Equals(Direction.DOWN))
                        direction = Direction.UP;
                    break;
                case ConsoleKey.DownArrow:
                    if (!direction.Equals(Direction.UP))
                        direction = Direction.DOWN;
                    break;
                case ConsoleKey.LeftArrow:
                    if (!direction.Equals(Direction.RIGHT))
                        direction = Direction.LEFT;
                    break;
                case ConsoleKey.RightArrow:
                    if (!direction.Equals(Direction.LEFT))
                        direction = Direction.RIGHT;
                    break;
                case ConsoleKey.Escape:
                    IsRunning = false;
                    break;
                case ConsoleKey.Spacebar:
                    if (wormTimer.Enabled)
                        wormTimer.Stop();
                    else
                        wormTimer.Start();
                    break;
                case ConsoleKey.R:
                    if (isGameOver)
                        restartGame();
                    break;
                case ConsoleKey.S:
                    SaveData();
                    break;
                case ConsoleKey.L:
                    LoadData();
                    break;
            }
            worm.ChangeDirection(direction);
        }

    }
}
