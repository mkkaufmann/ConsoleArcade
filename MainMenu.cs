using System;
using System.Threading.Tasks;

public class MainMenu : Menu
{
	public MainMenu():base()
    {
        lines[0] = "/============================\\";
        lines[1] = "|           Welcome          |";
        lines[2] = "|                            |";
        lines[3] = "|     Press 1 for Snake      |";
        lines[4] = "|                            |";
        lines[5] = "|     Press 2 for Tetris     |";
        lines[6] = "|                            |";
        lines[7] = "|     Press 3 for Pacman     |";
        lines[8] = "|                            |";
        lines[9] = "|      Press 4 for Pong      |";
        lines[10] = "|                            |";
        lines[11] = "|    Press 5 for Hi-Scores   |";
        lines[12] = "|                            |";
        lines[13] = "|      Press Q to Quit       |";
        lines[14] = "\\============================/";
    }
    public override void Show()
    {
        base.Show();
        getKey();
    }
    public void getKey()
    {
        var task = Task.Run(() => Console.ReadKey(true));
        bool read = task.Wait(3000);
        if (read)
            HandleKey(task.Result);
        else if(visible)
            getKey();
    }
    public void  HandleKey(ConsoleKeyInfo? cki2)
    {
        ConsoleKeyInfo cki = (ConsoleKeyInfo)cki2;
        if (cki.Key == ConsoleKey.D1)
        {
            Hide();
            Snake snake = new Snake(this);
            return;
        }
        else if (cki.Key == ConsoleKey.D2)
        {
        }
        else if (cki.Key == ConsoleKey.D3)
        {
        }
        else if (cki.Key == ConsoleKey.D4)
        {
        }
        else if (cki.Key == ConsoleKey.D5)
        {
        }
        else if (cki.Key == ConsoleKey.Q)
        {
            Environment.Exit(0);
        }
        getKey();
    }
}
