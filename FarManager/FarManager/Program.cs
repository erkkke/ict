using System;
using System.Collections.Generic;
using System.IO;

namespace FarManager
{
    class Program
    {
        static void Main(string[] args)
        {
            FarManager();
        }

        private static void FarManager()
        {

            Stack<Layer> history = new Stack<Layer>();
            history.Push(new Layer(new DirectoryInfo(@"C:\CODE"), 0));

            bool escape = false;
            bool isFileOpened = false;

            while (!escape)
            {
                Console.Clear();

                if (!isFileOpened)
                {
                    history.Peek().PrintInfo();
                    history.Peek().ShowSize();
                }
                else 
                    history.Peek().DisplayFileContent(history.Peek().GetCurrentObject().FullName);

                ConsoleKeyInfo consoleKeyInfo = Console.ReadKey(true);
                Console.CursorVisible = false;

                switch (consoleKeyInfo.Key)
                {
                    case ConsoleKey.Enter:
                        if (history.Peek().GetCurrentObject().GetType() == typeof(DirectoryInfo))
                            history.Push(new Layer(history.Peek().GetCurrentObject() as DirectoryInfo, 0));
                        else
                            isFileOpened = true;
                        break;
                    case ConsoleKey.UpArrow:
                        history.Peek().SetNewPosition(-1);
                        break;
                    case ConsoleKey.DownArrow:
                        history.Peek().SetNewPosition(1);
                        break;
                    case ConsoleKey.Escape:
                        if (history.Count > 1 && !isFileOpened)
                            history.Pop();
                        else if (isFileOpened)
                            isFileOpened = false;
                        else
                            escape = true;
                        break;
                    case ConsoleKey.Spacebar:
                        history.Peek().CreateFile();
                        break;
                    case ConsoleKey.Q:
                        history.Peek().DeleteFile();
                        break;
                }
            }
        }
    }
}