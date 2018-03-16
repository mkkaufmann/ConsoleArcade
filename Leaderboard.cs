using System;
using System.IO;

public class Leaderboard:Menu
{
    private string gameName;
    private int[] scores;
	public Leaderboard(string _gameName)
	{
        gameName = _gameName;
	}
    public void Add(int score)
    {
        StreamReader reader = new StreamReader("Snake.txt");
        for(int i = 0; i<10; i++)
        {
            scores[i] = int.Parse(reader.ReadLine().Split(',')[1]);
        }
    }
}
