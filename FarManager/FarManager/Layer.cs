using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FarManager
{
    class Layer
    {
        public DirectoryInfo dir
        {
            get;
            set;
        }
        public int pos
        {
            get;
            set;
        }
        public List<FileSystemInfo> content
        {
            get;
            set;
        }

        float size = 0.0f;

        public Layer(DirectoryInfo dir, int pos)
        {
            this.dir = dir;
            this.pos = pos;
            refreshDirectoryList();
        }

    

        public void PrintInfo()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.White;
            int cnt = 0;
            foreach (DirectoryInfo d in dir.GetDirectories())
            {
                if (cnt == pos)
                    Console.BackgroundColor = ConsoleColor.Cyan;
                
                else
                    Console.BackgroundColor = ConsoleColor.Black;
                
                Console.WriteLine($"{cnt + 1}. {d.Name}");
                cnt++;
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            foreach (FileInfo f in dir.GetFiles())
            {
                if (cnt == pos)
                {
                    Console.BackgroundColor = ConsoleColor.Cyan;
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                Console.WriteLine($"{cnt + 1}. {f.Name}");
                cnt++;
            }
        }

        void CalculateSize(string path)
        {
            if (GetCurrentObject().GetType() == typeof(DirectoryInfo)) {
                DirectoryInfo tempDir = new DirectoryInfo(path);
                foreach (FileSystemInfo fi in tempDir.GetFileSystemInfos())
                {
                    if (fi.GetType() == typeof(DirectoryInfo))
                        CalculateSize(fi.FullName);
                    else
                    {
                        FileInfo f = fi as FileInfo;
                        this.size += f.Length;
                    }
                }
            }
            else
            {
                FileInfo tempFile = new FileInfo(content[pos].FullName);
                this.size += tempFile.Length;
            }
        }


        public void ShowSize()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.BackgroundColor = ConsoleColor.Black;
            this.size = 0;
            CalculateSize(GetCurrentObject().FullName);
            Console.WriteLine("\nSize of \"" + content[pos].Name + "\" is " + this.size + " Bytes");
        }

        public FileSystemInfo GetCurrentObject()
        {
            return content[pos];
        }

        public void DisplayFileContent(string path)
        {
            Console.Clear();
            using (StreamReader file = new StreamReader(path))
            {
                int counter = 0;
                string ln;

                while ((ln = file.ReadLine()) != null)
                {
                    Console.WriteLine(ln);
                    counter++;
                }
                file.Close();
                Console.WriteLine($"File has {counter} lines.");
            }
        }

        public void SetNewPosition(int d)
        {
            if (d > 0)
                pos++;
            else
                pos--;
            
            if (pos >= content.Count)
                pos = 0;
            else if (pos < 0)
                pos = content.Count - 1;
        }

        public void CreateFolder()
        {
            string newFolder = Console.ReadLine();
            string path = Path.Combine(dir.Parent.FullName, newFolder);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                refreshDirectoryList();
            }
            else
                Console.WriteLine("Directory is already exist");
        }
        public void CreateFile()
        {
            string newFile = Console.ReadLine();
            string path = Path.Combine(dir.FullName, newFile);
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
                refreshDirectoryList();
            }
                
            else
                Console.WriteLine("File is already exist");
        }

        public void DeleteFile()
        {
            if (GetCurrentObject() is FileInfo)
            {
                string path = GetCurrentObject().FullName;
                File.Delete(path);
                pos -= 1;
                refreshDirectoryList();
            }
        }
        void refreshDirectoryList()
        {
            this.content = new List<FileSystemInfo>();

            content.AddRange(this.dir.GetDirectories());
            content.AddRange(this.dir.GetFiles());
        }
    }
}
