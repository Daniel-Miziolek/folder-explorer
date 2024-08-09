using System.Diagnostics;

namespace FolderExplorer
{
    class Program
    {
        static void Main()
        {
            string currentPath = @"C:\";
            Stack<string> pathHistory = new Stack<string>();

            string[] entries = Directory.GetFileSystemEntries(currentPath);
            int currentLine = 0;
            int topLine = 0;
            int windowHeight = Console.WindowHeight;

            Console.Clear();

            DisplayEntries(entries, currentLine, topLine, windowHeight);

            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);

                if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    if (currentLine < entries.Length - 1)
                    {
                        currentLine++;
                        if (currentLine >= topLine + windowHeight)
                        {
                            topLine++;
                        }
                        DisplayEntries(entries, currentLine, topLine, windowHeight);
                    }
                }
                else if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    if (currentLine > 0)
                    {
                        currentLine--;
                        if (currentLine < topLine)
                        {
                            topLine--;
                        }
                        DisplayEntries(entries, currentLine, topLine, windowHeight);
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    string path = entries[currentLine];
                    if (Directory.Exists(path))
                    {
                        pathHistory.Push(currentPath);
                        currentPath = path;
                        entries = Directory.GetFileSystemEntries(currentPath);
                        Console.Clear();
                        currentLine = 0;
                        topLine = 0;
                        DisplayEntries(entries, currentLine, topLine, windowHeight);
                    }
                    else if (File.Exists(path))
                    {
                        OpenFile(path);
                    }
                }
                else if (keyInfo.Key == ConsoleKey.LeftArrow)
                {
                    if (pathHistory.Count > 0)
                    {
                        currentPath = pathHistory.Pop();
                        entries = Directory.GetFileSystemEntries(currentPath);
                        Console.Clear();
                        currentLine = 0;
                        topLine = 0;
                        DisplayEntries(entries, currentLine, topLine, windowHeight);
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Escape)
                {
                    break;
                }
            }
        }

        static void DisplayEntries(string[] entries, int currentLine, int topLine, int windowHeight)
        {
            Console.Clear();

            for (int i = topLine; i < Math.Min(topLine + windowHeight, entries.Length); i++)
            {
                bool accessDenied = false;

                if (Directory.Exists(entries[i]))
                {
                    try
                    {
                        Directory.GetFileSystemEntries(entries[i]);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        accessDenied = true;
                    }
                }

                if (i == currentLine)
                {
                    HighlightLine(i - topLine, entries[i], accessDenied);
                }
                else
                {
                    if (accessDenied)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    Console.SetCursorPosition(0, i - topLine);
                    Console.WriteLine(entries[i]);
                    Console.ResetColor();
                }
            }
        }

        static void HighlightLine(int lineIndex, string entry, bool accessDenied)
        {
            Console.SetCursorPosition(0, lineIndex);
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = accessDenied ? ConsoleColor.Red : ConsoleColor.White;
            Console.WriteLine(entry);
            Console.ResetColor();
        }

        static void OpenFile(string path)
        {
            try
            {
                Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not open the file: {ex.Message}");
            }
        }
    }
}
