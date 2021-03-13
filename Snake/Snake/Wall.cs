using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Snake
{
    class Wall : GameObject
    {
        public Wall() : base()
        {

        }
        public Wall(char sign, ConsoleColor color, string path) : base(sign, color)
        {
            createMap(path);
        }

        
        public void createMap(string path)
        {
            Clear();
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    int rowNumber = 0;
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();

                        for (int columnNumber = 0; columnNumber < line.Length; ++columnNumber)
                        {
                            if (line[columnNumber] == '#')
                            {
                                body.Add(new Point { X = columnNumber, Y = rowNumber });
                            }
                        }

                        rowNumber++;
                    }
                }

            }
            Draw();
        }
    }
}
