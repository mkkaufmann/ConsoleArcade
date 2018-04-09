using System;
using System.IO;
public class Menu
{
    public string[] lines;
    public bool visible = false;
	public Menu()
	{
        lines = new string[15];
	}
    public Menu(string path)
    {
        lines = new string[15];
        StreamReader reader = new StreamReader(path);
        for (int i = 0; i < 15; i++)
        {
            lines[i] = reader.ReadLine();
        }
    }
    public virtual void Show()
    {
        visible = true;
        Console.BackgroundColor = ConsoleColor.Black;
        for (int i = 0; i < 15; i++)
        {
            Console.SetCursorPosition(45, i + 7);
            Console.Write(lines[i]);
        }
        
    }
    public virtual void Hide()
    {
        visible = false;
        for (int i = 0; i < 15; i++)
        {
            Console.SetCursorPosition(45, i + 7);
            for (int j = 0; j < 30; j++)
                Console.Write(" ");
        }
    }
}
