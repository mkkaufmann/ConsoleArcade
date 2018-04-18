using System;
using System.Timers;
using System.Collections.Generic;

public class Tetris : Game
{
    Timer timer = new Timer();
    Random rng;
    bool stopped;
    public Menu gameOver;
    public MainMenu menu;
    bool canChange = true;
    bool blockDropping = false;
    int prevX = -1;
    int prevY = -1;
    private int x;
    private int y;
    Rotation prevR = Rotation.r0;
    const int Y_OFFSET = 5;
    bool hardDrop = false;
    Block droppingBlock;
    List<Block> blocks;
    enum Rotation
    {
        r0,
        r90,
        r180,
        r270
    }
    private class Block
    {
        public int xPos;
        public int yPos;
        public int[] xOffsets;
        public int[] yOffsets;
        public int minX;
        public int maxX;
        public ConsoleColor c;
        public Rotation r;
        public virtual int xByR(int i)
        {
            switch (r)
            {
                case Rotation.r90:
                    return xPos + yOffsets[i];
                case Rotation.r180:
                    return xPos - xOffsets[i];
                case Rotation.r270:
                    return xPos - yOffsets[i];
                default:
                    return xPos + xOffsets[i];
            }
        }
        public virtual int yByR(int i)
        {
            switch (r)
            {
                case Rotation.r90:
                    return yPos - xOffsets[i];
                case Rotation.r180:
                    return yPos - yOffsets[i];
                case Rotation.r270:
                    return yPos + xOffsets[i];
                default:
                    return yPos + yOffsets[i];
            }
        }
        public virtual int xByR(int i, Rotation r)
        {
            switch (r)
            {
                case Rotation.r90:
                    return xPos + yOffsets[i];
                case Rotation.r180:
                    return xPos - xOffsets[i];
                case Rotation.r270:
                    return xPos - yOffsets[i];
                default:
                    return xPos + xOffsets[i];
            }
        }
        public virtual int yByR(int i,Rotation r)
        {
            switch (r)
            {
                case Rotation.r90:
                    return yPos - xOffsets[i];
                case Rotation.r180:
                    return yPos - yOffsets[i];
                case Rotation.r270:
                    return yPos + xOffsets[i];
                default:
                    return yPos + yOffsets[i];
            }
        }
    }
    //4/18 Moved pivots toward the center for better rotation
    private class BlockI : Block
    {
        /*
         * O
         * O
         * X
         * O
         */
        public BlockI()
        {
            xOffsets = new int[4] { 0, 0, 0, 0 };
            yOffsets = new int[4] { 1, 0, -1, -2 };
            minX = 0;
            maxX = 10;
        }
    }
    private class BlockJ : Block
    {
        /*
         * O
         * O X O
         */
        public BlockJ()
        {
            xOffsets = new int[4] { 0, -1, 1, -1 };
            yOffsets = new int[4] { 0, -1, 0, 0 };
            minX = 1;
            maxX = 9;
        }
    }
    private class BlockL : Block
    {
        /*
         *     O
         * O X O
         */
        public BlockL()
        {
            xOffsets = new int[] { -1, 0, 1, 1 };
            yOffsets = new int[] { 0, 0, 0, -1 };
            minX = 1;
            maxX = 9;
        }
    }
    private class BlockO : Block
    {
        /*
         * O O
         * X O
         */
        public BlockO()
        {
            xOffsets = new int[] { 0, 0, 1, 1 };
            yOffsets = new int[] { 0, -1, 0, -1 };
            minX = 0;
            maxX = 9;
        }
        public override int xByR(int i)
        {
            return xPos + xOffsets[i];
        }
        public override int yByR(int i)
        {
            return yPos + yOffsets[i];
        }
        public override int xByR(int i,Rotation r)
        {
            return xPos + xOffsets[i];
        }
        public override int yByR(int i, Rotation r)
        {
            return yPos + yOffsets[i];
        }
    }
    private class BlockS : Block
    {
        /*
         *   O O
         * O X
         */
        public BlockS()
        {
            xOffsets = new int[] { -1, 0, 0, 1 };
            yOffsets = new int[] { 0, 0, -1, -1 };
            minX = 1;
            maxX = 9;
        }
    }
    private class BlockT : Block
    {
        /*
         *   O
         * O X O
         */
        public BlockT()
        {
            xOffsets = new int[] { -1, 0, 1, 0 };
            yOffsets = new int[] { 0, 0, 0, -1 };
            minX = 1;
            maxX = 9;
        }
    }
    private class BlockZ : Block
    {
        /*
         * O O
         *   X O
         */
        public BlockZ()
        {
            xOffsets = new int[] { 0, 1, 0, -1 };
            yOffsets = new int[] { 0, 0, -1, -1 };
            minX = 1;
            maxX = 9;
        }
    }
    public Tetris()
    {

        menu = new MainMenu();
        stopped = false;
        gameOver = new Menu("gameOver.txt");
        rng = new Random();
        DrawFirst();
        timer.Interval = 100;
        timer.Elapsed += timerElapsed;
        timer.Enabled = true;
        blocks = new List<Block>();
        Run();
        Console.ReadKey(true);
    }
    private ConsoleColor GetRandomColor()
    {
        ConsoleColor[] colors = new ConsoleColor[] { ConsoleColor.Blue, ConsoleColor.Cyan, ConsoleColor.Green, ConsoleColor.Magenta, ConsoleColor.Red, ConsoleColor.Yellow };
        return colors[rng.Next(0, colors.Length)];
    }
    public override void Pause()
    {
        timer.Stop();
        canChange = false;
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

    private void DrawFirst()
    {
        Console.Clear();
        for (int i = 1; i < 21; i++)
        {
            Console.SetCursorPosition(Console.WindowWidth / 2 - 12, i + Y_OFFSET);
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.Write("  ");
            Console.SetCursorPosition(Console.WindowWidth / 2 + 10, i + Y_OFFSET);
            Console.Write("  ");
        }
        for (int i = 0; i < 10; i++)
        {
            Console.SetCursorPosition(Console.WindowWidth / 2 - 10 + i * 2, 20 + Y_OFFSET);
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.Write("  ");
        }
    }
    private void Draw()
    {
        canChange = true;
        if (!blockDropping)
        {
            blockDropping = true;
            switch (rng.Next(0, 7))
            {
                case 0:
                    droppingBlock = new BlockI();
                    break;
                case 1:
                    droppingBlock = new BlockJ();
                    break;
                case 2:
                    droppingBlock = new BlockS();
                    break;
                case 3:
                    droppingBlock = new BlockT();
                    break;
                case 4:
                    droppingBlock = new BlockZ();
                    break;
                case 5:
                    droppingBlock = new BlockO();
                    break;
                default:
                    droppingBlock = new BlockL();
                    break;
            }
            blocks.Add(droppingBlock);
            droppingBlock.c = GetRandomColor();
            droppingBlock.r = Rotation.r0;
            droppingBlock.xPos = rng.Next(droppingBlock.minX, droppingBlock.maxX);
            droppingBlock.yPos = Y_OFFSET;
        }
        for (int i = 0; i < 4; i++)
        {
            x = droppingBlock.xByR(i);
            y = droppingBlock.yByR(i)+1;
            if (y == 21 + Y_OFFSET)
            {
                prevX = -1;
                prevY = -1;
                prevR = Rotation.r0;
                blockDropping = false;
                hardDrop = false;
                timer.Interval = 100;
                return;
            }
            foreach (Block b in blocks)
            {
                if (b == droppingBlock)
                    continue;
                for (int k = 0; k < 4; k++)
                {
                    if ((b.xByR(k) == x && b.yByR(k) == y))
                    {
                        blockDropping = false;
                        hardDrop = false;
                        prevX = -1;
                        prevY = -1;
                        prevR = Rotation.r0;
                        timer.Interval = 100;
                        for (int g = 0; g < 4; g++)
                        {
                            y = droppingBlock.yByR(g);
                            if (y <= Y_OFFSET)
                            {
                                Stop();
                                break;
                            }
                        }
                        return;
                    }
                }
            }
        }
        if (prevX != -1 && prevY != -1)
        {
            for (int i = 0; i < 4; i++)
            {
                if(droppingBlock.GetType() != new BlockO().GetType())
                {
                    switch (prevR)
                    {
                        case Rotation.r90:
                            x = prevX + droppingBlock.yOffsets[i];
                            y = prevY - droppingBlock.xOffsets[i];
                            break;
                        case Rotation.r180:
                            x = prevX - droppingBlock.xOffsets[i];
                            y = prevY - droppingBlock.yOffsets[i];
                            break;
                        case Rotation.r270:
                            x = prevX - droppingBlock.yOffsets[i];
                            y = prevY + droppingBlock.xOffsets[i];
                            break;
                        default:
                            x = prevX + droppingBlock.xOffsets[i];
                            y = prevY + droppingBlock.yOffsets[i];
                            break;
                    }
                }else
                {
                    x = prevX + droppingBlock.xOffsets[i];
                    y = prevY + droppingBlock.yOffsets[i];
                }
                
                if (y >= 0)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.SetCursorPosition(x * 2 + (Console.WindowWidth / 2 - 10), y);
                    Console.Write("  ");
                }
            }
        }
        for (int i = 0; i < 4; i++)
        {
            x = droppingBlock.xByR(i);
            y = droppingBlock.yByR(i);
            if (y >= 0)
            {
                Console.BackgroundColor = droppingBlock.c;
                Console.SetCursorPosition(x * 2 + (Console.WindowWidth / 2 - 10), y);
                Console.Write("  ");
            }
        }
        prevX = droppingBlock.xPos;
        prevY = droppingBlock.yPos;
        prevR = droppingBlock.r;
        droppingBlock.yPos += 1;
    }
    public override void Stop()
    {
        timer.Stop();
        Console.BackgroundColor = ConsoleColor.Black;
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
        if (canChange && !hardDrop && blockDropping)
        {
            int x;
            int y;
            if (cki.Key == ConsoleKey.LeftArrow || cki.Key == ConsoleKey.A)
            {
                bool canMove = true;
                for (int i = 0; i < 4; i++)
                {
                    x = droppingBlock.xByR(i) - 1;
                    y = droppingBlock.yByR(i);
                    foreach (Block b in blocks)
                    {
                        if (b == droppingBlock)
                            continue;
                        for (int k = 0; k < 4; k++)
                        {
                            if ((b.xByR(k) == x && b.yByR(k) == y))
                            {
                                canMove = false;
                            }
                        }
                    }
                }
                if (blockDropping && droppingBlock.xPos > droppingBlock.minX && canMove)
                {
                    droppingBlock.xPos -= 1;
                }
            }
            else if (cki.Key == ConsoleKey.RightArrow || cki.Key == ConsoleKey.D)
            {
                bool canMove = true;
                for (int i = 0; i < 4; i++)
                {
                    x = droppingBlock.xByR(i) + 1;
                    y = droppingBlock.yByR(i);
                    foreach (Block b in blocks)
                    {
                        if (b == droppingBlock)
                            continue;
                        for (int k = 0; k < 4; k++)
                        {
                            if ((b.xByR(k) == x && b.yPos + b.yByR(k) == y))
                            {
                                canMove = false;
                            }
                        }
                    }
                }
                if (blockDropping && droppingBlock.xPos < droppingBlock.maxX - 1 && canMove)
                {
                    droppingBlock.xPos += 1;
                }
            }
            else if (cki.Key == ConsoleKey.UpArrow || cki.Key == ConsoleKey.W)
            {
                Rotation desiredR;
                switch (droppingBlock.r)
                {
                    case Rotation.r90:
                        desiredR = Rotation.r180;
                        break;
                    case Rotation.r180:
                        desiredR = Rotation.r270;
                        break;
                    case Rotation.r270:
                        desiredR = Rotation.r0;
                        break;
                    default:
                        desiredR = Rotation.r90;
                        break;
                }
                bool canMove = true;
                for (int i = 0; i < 4; i++)
                {
                    x = droppingBlock.xByR(i, desiredR);
                    y = droppingBlock.yByR(i, desiredR);
                    foreach (Block b in blocks)
                    {
                        if (b == droppingBlock)
                            continue;
                        for (int k = 0; k < 4; k++)
                        {
                            if ((b.xByR(k) == x && b.yPos + b.yByR(k) == y))
                            {
                                canMove = false;
                            }
                        }
                    }
                }
                if (canMove)
                {
                    droppingBlock.r = desiredR;
                }
            }
            /*HELP THERE'S A BUG IN MY CODE!!           
                         ()I()
                    "==.__:-:__.=="
                   "==.__/~|~\__.=="
                   "==._(  Y  )_.=="
        .-'~~""~=--...,__\/|\/__,...--=~""~~'-.
       (               ..=\=/=..               )
        `'-.        ,.-"`;/=\ ;"-.,_        .-'`
            `~"-=-~` .-~` |=| `~-. `~-=-"~`
                 .-~`    /|=|\    `~-.
              .~`       / |=| \       `~.
          .-~`        .'  |=|  `.        `~-.
        (`     _,.-="`    |=|    `"=-.,_     `)
         `~"~"`           |=|           `"~"~`
                          |=|
                          |=|
                          |=|
                          /=\
            */
            else if (cki.Key == ConsoleKey.DownArrow || cki.Key == ConsoleKey.S)
            {
                Rotation desiredR;
                switch (droppingBlock.r)
                {
                    case Rotation.r90:
                        desiredR = Rotation.r0;
                        break;
                    case Rotation.r180:
                        desiredR = Rotation.r90;
                        break;
                    case Rotation.r270:
                        desiredR = Rotation.r180;
                        break;
                    default:
                        desiredR = Rotation.r270;
                        break;
                }
                bool canMove = true;
                for (int i = 0; i < 4; i++)
                {
                    x = droppingBlock.xByR(i,desiredR);
                    y = droppingBlock.yByR(i,desiredR);
                    foreach (Block b in blocks)
                    {
                        if (b == droppingBlock)
                            continue;
                        for (int k = 0; k < 4; k++)
                        {
                            if ((b.xByR(k) == x && b.yPos + b.yByR(k) == y))
                            {
                                canMove = false;
                            }
                        }
                    }
                }
                if (canMove)
                {
                    droppingBlock.r = desiredR;
                }
            }
        }
        else if (cki.Key == ConsoleKey.Spacebar)
        {
            hardDrop = true;
            timer.Interval = 50;
        }
        canChange = false;
    }

}

