using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

public class Snake : Game
{
    public List<Point> body;//the body of the snake
    public ConsoleColor defaultbg = ConsoleColor.Black;//default console background
    public ConsoleColor snakeclr = ConsoleColor.DarkGreen;//snake color
    public ConsoleColor foodclr = ConsoleColor.Red;//food color
    public Point currentFood;//current food object
    private Random rand;//rng
    private System.Timers.Timer timer;
    private Direction dir;
    private bool stopped = false;
    public Menu menu;
    private Menu gameOver;
    private Thread keys;
    private bool canChange = true;
    public Snake(Menu _menu)
    {
        //initialize the body and rng
        rand = new Random();
        body = new List<Point>();
        timer = new System.Timers.Timer();
        timer.Interval = 150;
        timer.AutoReset = true;
        timer.Elapsed += timerElapsed;
        dir = Direction.Up;
        name = "Snake";
        leaderboard = new Leaderboard(name);
        //put the snake in the center
        body.Add(new Point(60, 17));
        body.Add(new Point(60, 16));
        body.Add(new Point(60, 15));
        //draw the snake and food
        drawSnake();
        generateFood();

        menu = _menu;
        keys = new Thread(Run);
        keys.Start();
        gameOver = new Menu("gameOver.txt");
        timer.Enabled = true;
    }
    private void timerElapsed(object sender, ElapsedEventArgs e)
    {
        draw(dir);
    }

    public void generateFood()
    {
        //generate food at a random point
        currentFood = new Point(rand.Next(0, Console.WindowWidth / 2) * 2, rand.Next(Console.WindowHeight));
        //loop through the snake body
        foreach (Point p in body)
        {
            //if the food spawns on the snake, respawn it
            if (currentFood == p)
            {
                generateFood();
                return;
            }
        }
        //draw the food once it's in an acceptable position
        drawFood();
    }
    //draw the snake
    public void drawSnake()
    {
        foreach (Point p in body)
        {
            Console.SetCursorPosition(p.x, p.y);
            Console.BackgroundColor = snakeclr;
            //a square is two spaces
            Console.Write("  ");
        }

    }
    //move the snake
    public void draw(Direction dir)
    {
        canChange = true;
        //new front of the snake
        Point p = null;
        //set the new position based on the current direction
        switch (dir)
        {
            case Direction.Up:
                p = new Point(body[body.Count - 1].x, body[body.Count - 1].y - 1);
                break;
            case Direction.Down:
                p = new Point(body[body.Count - 1].x, body[body.Count - 1].y + 1);
                break;
            case Direction.Left:
                p = new Point(body[body.Count - 1].x - 2, body[body.Count - 1].y);
                break;
            case Direction.Right:
                p = new Point(body[body.Count - 1].x + 2, body[body.Count - 1].y);
                break;
        }
        //if it's not on the food, cover the back with the default bg
        if (p != currentFood)
        {
            Console.SetCursorPosition(body[0].x, body[0].y);
            Console.BackgroundColor = defaultbg;
            Console.Write("  ");
            body.RemoveAt(0);
        }
        else//if it is, generate a new food
        {
            score++;
            generateFood();
        }
        //if you go outside the window, you lose
        if (p.x >= Console.WindowWidth || p.x < 0 || p.y >= Console.WindowHeight || p.y < 0)
        {
            Console.SetCursorPosition(0, 0);
            Stop();
            return;
        }

        //draw the new front snake
        Console.SetCursorPosition(p.x, p.y);
        Console.BackgroundColor = snakeclr;
        Console.Write("  ");
        Console.BackgroundColor = defaultbg;
        body.Add(p);


        //if you hit the body, you lose
        foreach (Point b in body)
        {
            if (!(body[body.Count - 1].Equals(b)))//check that the references aren't equal
            {
                if (p == b)//if they are the same, you lose
                {
                    Console.SetCursorPosition(0, 0);
                    Stop();
                }
            }
        }

    }
    public override void Resume()//draw the snake and the food again
    {
        drawSnake();
        drawFood();
        timer.Start();
    }
    private void drawFood()//draw the food
    {
        Console.SetCursorPosition(currentFood.x, currentFood.y);
        Console.BackgroundColor = foodclr;
        Console.Write("  ");
    }
    public override void HandleKey(ConsoleKeyInfo? cki2) {
        if (cki2 == null)
        {
            Run();
            return;
        }
        ConsoleKeyInfo cki = (ConsoleKeyInfo)cki2;
        if (canChange)
        {
            if ((cki.Key == ConsoleKey.W || cki.Key == ConsoleKey.UpArrow) && dir != Direction.Down)
            {
                dir = Direction.Up;
            }
            else if ((cki.Key == ConsoleKey.A || cki.Key == ConsoleKey.LeftArrow) && dir != Direction.Right)
            {
                dir = Direction.Left;
            }
            else if ((cki.Key == ConsoleKey.S || cki.Key == ConsoleKey.DownArrow) && dir != Direction.Up)
            {
                dir = Direction.Down;
            }
            else if ((cki.Key == ConsoleKey.D || cki.Key == ConsoleKey.RightArrow) && dir != Direction.Left)
            {
                dir = Direction.Right;
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
    public override void Run()
    {
        while (!stopped)
        {
            ReadKey();
        }
    }
    public override void Pause()
    {
        timer.Stop();
    }
    public override void Stop()
    {
        timer.Stop();
        Console.Clear();
        stopped = true;
        gameOver.Show();
        Task leaderboardTask = new Task(()=>leaderboard.Add(score));
        leaderboardTask.Start();
        leaderboardTask.Wait();
    }
    public enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }
}
