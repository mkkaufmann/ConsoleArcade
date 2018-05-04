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
    public override void Pause()
    {
        timer.Stop();
    }
    public override void Resume()//draw the snake and the food again
    {
        timer.Start();
    }
    public override void Run()
    {
        while (!stopped)
        {
            ReadKey();
        }
        gameOver.Hide();
        menu.Show();
    }
    Timer timer = new Timer();
    Ball ball;
    Paddle player;
    Paddle enemy;
    Random rng;
    bool stopped;
    public Menu gameOver;
    public MainMenu menu;
    bool canChange = true;
    bool canChange2 = true;
    private bool aiMove = false;
    private bool two_players;
    public Pong(bool two_players)
    {
        this.two_players = two_players;
        ball = new Ball(Console.WindowWidth / 4, Console.WindowHeight / 2, 1, 0);
        player = new Paddle(Console.WindowHeight / 2 - 3, Console.WindowHeight / 2 + 2, 2);
        enemy = new Paddle(Console.WindowHeight / 2 - 3, Console.WindowHeight / 2 + 2, (Console.WindowWidth/2)-2);
        menu = new MainMenu();
        stopped = false;
        gameOver = new Menu("gameOver.txt");
        rng = new Random();
        DrawFirst();
        timer.Interval = 100;
        timer.Elapsed += timerElapsed;
        timer.Enabled = true;
        Run();
        Console.ReadKey(true);

	}
    private void DrawFirst()
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
    private void Draw()
    {
        if (!two_players)
        {
            aiMove = !aiMove;
            if (aiMove)
            {
                if (ball.y < enemy.topY)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.SetCursorPosition(enemy.x * 2, enemy.bottomY);
                    Console.Write(" ");
                    enemy.bottomY -= 1;
                    enemy.topY -= 1;
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.SetCursorPosition(enemy.x * 2, enemy.topY);
                    Console.Write(" ");
                }
                else if (ball.y > enemy.bottomY)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.SetCursorPosition(enemy.x * 2, enemy.topY);
                    Console.Write(" ");
                    enemy.bottomY += 1;
                    enemy.topY += 1;
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.SetCursorPosition(enemy.x * 2, enemy.bottomY);
                    Console.Write(" ");
                }
            }
        }
        canChange = true;
        canChange2 = true;
        Console.BackgroundColor = ConsoleColor.Black;
        try
        {
            Console.SetCursorPosition(ball.x * 2, ball.y);
        }catch//ball goes below zero
        {
            Stop();//game over
        }
        Console.Write("  ");
        if ((ball.x == player.x && ball.y < player.bottomY && ball.y > player.topY))
        {
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.SetCursorPosition(ball.x * 2+1, ball.y);
            Console.Write(" ");
        }
        if(ball.x == enemy.x && ball.y < enemy.bottomY && ball.y > enemy.topY)
        {
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.SetCursorPosition(ball.x * 2, ball.y);
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
            if (!two_players)
            {
                Stop();
            }
            else
            {
                ball.x = Console.WindowWidth / 4;
                ball.y = Console.WindowHeight / 2;
                ball.xV = 1;
                ball.yV = 0;
            }

        }
        Console.BackgroundColor = ConsoleColor.Green;
        Console.SetCursorPosition(ball.x * 2, ball.y);
        Console.Write("  ");

    }
    public override void Stop()
    {
        timer.Stop();
        Console.Clear();
        gameOver.Show();
        stopped = true;
    }
    private void timerElapsed(object sender, ElapsedEventArgs e)
    {
        Draw();
    }
    public override void HandleKey(ConsoleKeyInfo? cki2)
    {
        if (stopped)
        {
            return;
        }
        if (cki2 == null)
        {
            Run();
            return;
        }
        ConsoleKeyInfo cki = (ConsoleKeyInfo)cki2;
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
        if (!two_players)
        {
            if (canChange)
            {
                if ((cki.Key == ConsoleKey.W || cki.Key == ConsoleKey.UpArrow) && player.topY > 0)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.SetCursorPosition(player.x * 2 + 1, player.bottomY);
                    Console.Write(" ");
                    player.bottomY -= 1;
                    player.topY -= 1;
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.SetCursorPosition(player.x * 2 + 1, player.topY);
                    Console.Write(" ");
                }
                else if ((cki.Key == ConsoleKey.S || cki.Key == ConsoleKey.DownArrow) && player.bottomY < Console.WindowHeight)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.SetCursorPosition(player.x * 2 + 1, player.topY);
                    Console.Write(" ");
                    player.bottomY += 1;
                    player.topY += 1;
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.SetCursorPosition(player.x * 2 + 1, player.bottomY);
                    Console.Write(" ");
                }

                canChange = false;
            }
        }else
        {
            if (canChange)
            {
                if ((cki.Key == ConsoleKey.W) && player.topY > 0)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.SetCursorPosition(player.x * 2 + 1, player.bottomY);
                    Console.Write(" ");
                    player.bottomY -= 1;
                    player.topY -= 1;
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.SetCursorPosition(player.x * 2 + 1, player.topY);
                    Console.Write(" ");
                }
                else if ((cki.Key == ConsoleKey.S) && player.bottomY < Console.WindowHeight)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.SetCursorPosition(player.x * 2 + 1, player.topY);
                    Console.Write(" ");
                    player.bottomY += 1;
                    player.topY += 1;
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.SetCursorPosition(player.x * 2 + 1, player.bottomY);
                    Console.Write(" ");
                }

                canChange = false;
            }
            if (canChange2)
            {
                if ((cki.Key == ConsoleKey.UpArrow) && enemy.topY > 0)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.SetCursorPosition(enemy.x * 2, enemy.bottomY);
                    Console.Write(" ");
                    enemy.bottomY -= 1;
                    enemy.topY -= 1;
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.SetCursorPosition(enemy.x * 2, enemy.topY);
                    Console.Write(" ");
                }
                else if ((cki.Key == ConsoleKey.DownArrow) && enemy.bottomY < Console.WindowHeight)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.SetCursorPosition(enemy.x * 2, enemy.topY);
                    Console.Write(" ");
                    enemy.bottomY += 1;
                    enemy.topY += 1;
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.SetCursorPosition(enemy.x * 2, enemy.bottomY);
                    Console.Write(" ");
                }

                canChange2 = false;
            }
        }
    }
}
