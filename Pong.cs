using System;
using System.Timers;
public class Pong:Game
{
    class Ball:Point
    {
        public int xV;
        public int yV;
        public Ball(int _x, int _y, int _xV,int _yV):base(_x,_y)
        {
            xV = _xV;
            yV = _yV;
        }
    }
    class Paddle 
    {
        public int topY;
        public int bottomY;
        public int x;
        public Paddle(int _topY, int _bottomY, int _x)
        {
            topY = _topY;
            bottomY = _bottomY;
            x = _x;
        }
    }
    Timer timer = new Timer();
    Ball ball;
    Paddle player;
    Paddle enemy;
    Random rng;
    bool canChange = true;
    public Pong()
    {
        ball = new Ball(Console.WindowWidth / 4, Console.WindowHeight / 2, 1, 0);
        player = new Paddle(Console.WindowHeight / 2 - 3, Console.WindowHeight / 2 + 3, 2);
        enemy = new Paddle(Console.WindowHeight / 2 - 3, Console.WindowHeight / 2 + 3, (Console.WindowWidth/2)-2);
        rng = new Random();
        drawFirst();
        timer.Interval = 100;
        timer.Elapsed += timerElapsed;
        timer.Enabled = true;
        Console.ReadLine();
	}
    private void drawFirst()
    {
        Console.BackgroundColor = ConsoleColor.Green;
        Console.SetCursorPosition(ball.x * 2, ball.y);
        Console.Write("  ");
        Console.BackgroundColor = ConsoleColor.DarkRed;
        for (int i = player.topY; i < player.bottomY; i++)
        {
            Console.SetCursorPosition(player.x*2+1, i);
            Console.Write(" ");
        }
        for (int i = enemy.topY; i < enemy.bottomY; i++)
        {
            Console.SetCursorPosition(enemy.x*2, i);
            Console.Write(" ");
        }
    }
    private void draw()
    {
        canChange = true;
        Console.BackgroundColor = ConsoleColor.Black;
        Console.SetCursorPosition(ball.x * 2, ball.y);
        Console.Write("  ");
        if ((ball.x == player.x && ball.y <= player.bottomY && ball.y >= player.topY))
        {
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.SetCursorPosition(ball.x * 2+1, ball.y);
            Console.Write(" ");
        }
        if(ball.x == enemy.x && ball.y <= enemy.bottomY && ball.y >= enemy.topY)
        {
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.Write(" ");
        }
        ball.x += ball.xV;
        ball.y += ball.yV;
        if ((ball.x == enemy.x - 1 && ball.y <= enemy.bottomY && ball.y >= enemy.topY)||(ball.x == player.x + 1 && ball.y <= player.bottomY && ball.y >= player.topY))
        {
            ball.xV *= -1;
            int dir = rng.Next(0, 3);
            switch (dir)
            {
                case 0:
                    ball.yV = 0;
                    break;
                case 1:
                    ball.yV = -1;
                    break;
                case 2:
                    ball.yV = 1;
                    break;
            }
        }
        if (ball.x * 2 >= Console.WindowWidth)
        {
            score++;
            ball.x = Console.WindowWidth / 4;
            ball.y = Console.WindowHeight / 2;
            ball.xV = -1;
            ball.yV = 0;
        }
        if (ball.y == Console.WindowHeight-1||ball.y == 0)
        {
            ball.yV *= -1;
        }
        if (ball.x * 2 <=0)
        {
            Stop();
            return;
        }
        Console.BackgroundColor = ConsoleColor.Green;
        Console.SetCursorPosition(ball.x * 2, ball.y);
        Console.Write("  ");
    }
    public override void Stop()
    {
        base.Stop();
    }
    private void timerElapsed(object sender, ElapsedEventArgs e)
    {
        draw();
    }
    public override void HandleKey(ConsoleKeyInfo? cki2)
    {
        if (cki2 == null)
        {
            Run();
            return;
        }
        ConsoleKeyInfo cki = (ConsoleKeyInfo)cki2;
        if (canChange)
        {
            if (cki.Key == ConsoleKey.W || cki.Key == ConsoleKey.UpArrow)
            {
                int newTY = player.topY - 1;
                int newBY = player.bottomY - 1;
            }
            else if (cki.Key == ConsoleKey.S || cki.Key == ConsoleKey.DownArrow)
            {
            }
            if (cki.Key == ConsoleKey.Escape)
            {
                if (timer.Enabled)
                {
                    Pause();
                }
                else
                {
                    Resume();
                }
            }
            canChange = false;
        }
    }
}
