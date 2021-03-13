using System;
using System.Collections.Generic;
using System.Text;

namespace Snake
{
    class Food : GameObject
    {
        Random rnd = new Random();
        public Food(char sign, ConsoleColor color, List<Point> worm, List<Point> wall) : base(sign, color)
        {
            Point location = new Point();
            body.Add(location);
            Generate(worm, wall);
        }

        public void Generate(List<Point> worm, List<Point> wall)
        {

            body[0].X = rnd.Next(1, Game.Width);
            body[0].Y = rnd.Next(5, Game.Height);

            foreach (Point i in wall)
            {
                if (i.X == body[0].X && i.Y == body[0].Y)
                    Generate(worm, wall);
            }
            foreach (Point i in worm)
            {
                if (i.X == body[0].X && i.Y == body[0].Y)
                    Generate(worm, wall);
            }

            Draw();
        }
    }
}
