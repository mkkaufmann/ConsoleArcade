using System;
using System.Threading.Tasks;

public class Game
{
    public int score = 0;
	public Game()
	{

	}
    public virtual void Run()
    {
    }
    public virtual void Stop()
    {

    }
    public virtual void Resume()
    {

    }
    public virtual void Pause()
    {

    }
    public void AddToLeaderboard()
    {

    }
    public virtual void ReadKey()
    {
        var task = Task.Run(() => Console.ReadKey(true));
        bool read = task.Wait(3000);
        if (read)
            HandleKey(task.Result);
    }
    public virtual void HandleKey(ConsoleKeyInfo? cki)
    {

    }
}
