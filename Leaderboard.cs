using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class Leaderboard:Menu
{
    private string gameName;
    private struct Entry
    {
        public int score;
        public string name;
        public Entry(string _name, int _score) {
            name = _name;
            score = _score;
        }
    }
    private LinkedList<Entry> scores;
	public Leaderboard(string _gameName)
	{
        gameName = _gameName;
        scores = new LinkedList<Entry>();
	}
    public void Add(int score)
    {
        StreamReader reader = new StreamReader("Snake.txt");
        try
        {
            for (int i = 0; i < 10; i++)
            {
                string[] line = reader.ReadLine().Split(',');
                scores.AddLast(new Entry(line[0], int.Parse(line[1])));
            }
        }
        catch
        {
        }
        
        LinkedListNode<Entry> node = scores.First;
        while(node.Value.score > score)
        {
            try
            {
                node = node.Next;
            }catch
            {
                
                MainMenu menu1 = new MainMenu();
                menu1.Show();
                return;
            }
        }
        Menu nameMenu = new Menu("enterName.txt");
        nameMenu.Show();
        Console.SetCursorPosition(0,0);
        List<char> playerName = new List<char>();
        ConsoleKeyInfo cki;
        Point cursor=new Point(0,0);
        while (true)
        {
            cki = Console.ReadKey(true);
            if (cki.Key == ConsoleKey.Enter)
            {
                if (playerName.Count == 0)
                {
                    cursor = new Point(Console.CursorLeft, Console.CursorTop);
                    Console.SetCursorPosition(0, 0);
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.Write("Please enter your name");
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.SetCursorPosition(cursor.x, cursor.y);
                } else
                {
                    break;
                }
            }else if(cki.Key == ConsoleKey.Backspace&&playerName.Count > 0)
            {
                Console.SetCursorPosition(cursor.x-1, cursor.y);
                Console.Write(" ");
                playerName.RemoveAt(playerName.Count - 1);
            }
            else
            {
                playerName.Add(cki.KeyChar);
                Console.Write(cki.KeyChar);
            }
            cursor = new Point(Console.CursorLeft, Console.CursorTop);
        };
        scores.AddBefore(node,new Entry(new string(playerName.ToArray()),score));
        scores.RemoveLast();
        MainMenu menu = new MainMenu();
        menu.Show();
    }
}
