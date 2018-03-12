using System;

public class Menu
{
    public string[] lines;
	public Menu()
	{
        lines = new string[15];
	}
    public virtual void Show()
    {
        for (int i = 0; i < 15; i++)
        {
            Console.SetCursorPosition(45, i + 7);
            Console.Write(lines[i]);
        }
        while (true)
        {
            ConsoleKeyInfo cki = Console.ReadKey(true);
            if (cki.Key == ConsoleKey.D1)
            {
                Hide();
                Snake snake = new Snake();
                snake.Run(this);
                break;
            }
            else if (cki.Key == ConsoleKey.D2)
            {
                break;
            }
            else if (cki.Key == ConsoleKey.D3)
            {
                break;
            }
            else if (cki.Key == ConsoleKey.D4)
            {
                break;
            }
            else if (cki.Key == ConsoleKey.D5)
            {
                break;
            }
            else if (cki.Key == ConsoleKey.Q)
            {
                Environment.Exit(0);
            }
        }
    }
    public virtual void Hide()
    {
        for (int i = 0; i < 15; i++)
        {
            Console.SetCursorPosition(45, i + 7);
            for (int j = 0; j < 30; j++)
                Console.Write(" ");
        }
    }
}
