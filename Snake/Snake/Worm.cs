using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Snake
{

    public class Worm : GameObject
    {
        public int Dx { get; set; }
        public int Dy { get; set; }

        public Worm() : base()
        {
        }
        public Worm(char sign, ConsoleColor color) : base(sign, color)
        {
            Clear();
            Point head = new Point { X = 10, Y = 20 };
            body.Add(head);
            Draw();
        }


        public void ChangeDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.UP:
                    Dx = 0; Dy = -1;
                    break;
                case Direction.DOWN:
                    Dx = 0; Dy = 1;
                    break;
                case Direction.LEFT:
                    Dx = -1; Dy = 0;
                    break;
                case Direction.RIGHT:
                    Dx = 1; Dy = 0;
                    break;
            }
        }
        public void Move()
        {
            Clear();

            for (int i = body.Count - 1; i > 0; --i)
            {
                body[i].X = body[i - 1].X;
                body[i].Y = body[i - 1].Y;
            }

            body[0].X += Dx;
            body[0].Y += Dy;


            Draw();
        }
        public void Increase(Point point)
        {
            body.Add(new Point { X = point.X, Y = point.Y });
        }

        public void Save(string title)
        {
            using (FileStream fs = new FileStream(title + ".xml", FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                XmlSerializer xs = new XmlSerializer(typeof(Worm));
                xs.Serialize(fs, this);
            }
        }

        public static Worm Load(string title)
        {
            Worm res = null;
            using (FileStream fs = new FileStream(title + ".xml", FileMode.Open, FileAccess.Read))
            {
                XmlSerializer xs = new XmlSerializer(typeof(Worm));
                res = xs.Deserialize(fs) as Worm;
            }
            return res;
        }
    }
}
