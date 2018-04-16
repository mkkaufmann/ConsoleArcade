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
    }
    private class BlockI : Block
    {
        /*
         * O
         * O
         * O
         * X
         */
        public BlockI()
        {
            xOffsets = new int[4] { 0, 0, 0, 0 };
            yOffsets = new int[4] { 0, -1, -2, -3 };
            minX = 0;
            maxX = 10;
        }
    }
    private class BlockJ : Block
    {
        /*
         * O
         * X O O
         */
        public BlockJ()
        {
            xOffsets = new int[4] { 0, 0, 1, 2 };
            yOffsets = new int[4] { 0, -1, 0, 0 };
            minX = 0;
            maxX = 8;
        }
    }
    private class BlockL : Block
    {
        /*
         *     O
         * X O O
         */
        public BlockL()
        {
            xOffsets = new int[] { 0, 1, 2, 2 };
            yOffsets = new int[] { 0, 0, 0, -1 };
            minX = 0;
            maxX = 8;
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
    }
    private class BlockS : Block
    {
        /*
         *   O O
         * X O
         */
        public BlockS()
        {
            xOffsets = new int[] { 0, 1, 1, 2 };
            yOffsets = new int[] { 0, 0, -1, -1 };
            minX = 0;
            maxX = 8;
        }
    }
    private class BlockT : Block
    {
        /*
         *   O
         * X O O
         */ 
        public BlockT()
        {
            xOffsets = new int[] { 0, 1, 2, 1 };
            yOffsets = new int[] { 0, 0, 0, -1 };
            minX = 0;
            maxX = 8;
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
        ConsoleColor[] colors = new ConsoleColor[] {ConsoleColor.Blue,ConsoleColor.Cyan,ConsoleColor.DarkCyan,ConsoleColor.DarkGreen,ConsoleColor.DarkMagenta,ConsoleColor.DarkRed,ConsoleColor.Green,ConsoleColor.Magenta,ConsoleColor.Red,ConsoleColor.Yellow};
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
        for(int i = 1; i<21; i++)
        {
            Console.SetCursorPosition(Console.WindowWidth / 2 - 12, i+Y_OFFSET);
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.Write("  ");
            Console.SetCursorPosition(Console.WindowWidth / 2 + 10, i+Y_OFFSET);
            Console.Write("  ");
        }
        for (int i = 0; i < 10; i++)
        {
            Console.SetCursorPosition(Console.WindowWidth / 2 - 10 + i*2, 20+Y_OFFSET);
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
            droppingBlock.xPos = rng.Next(droppingBlock.minX, droppingBlock.maxX);
            droppingBlock.yPos = Y_OFFSET;
        }
        for (int i = 0; i < 4; i++)
        {
            int x = droppingBlock.xPos + droppingBlock.xOffsets[i];
            int y = droppingBlock.yPos + droppingBlock.yOffsets[i] + 1;
            if (y == 21+Y_OFFSET)
            {
                prevX = -1;
                prevY = -1;
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
                    if ((b.xPos + b.xOffsets[k] == x && b.yPos + b.yOffsets[k] == y))
                    {
                        blockDropping = false;
                        hardDrop = false;
                        prevX = -1;
                        prevY = -1;
                        timer.Interval = 100;
                        for (int g = 0; g < 4; g++)
                        {
                            y = droppingBlock.yPos + droppingBlock.yOffsets[g];
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
        for (int i = 0; i < 4; i++)
        {
            int x = droppingBlock.xPos + droppingBlock.xOffsets[i];
            int y = droppingBlock.yPos + droppingBlock.yOffsets[i];
            if (prevX != -1 && prevY != -1)
            {

                x = prevX + droppingBlock.xOffsets[i];
                y = prevY + droppingBlock.yOffsets[i];
                if (y >= 0)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.SetCursorPosition(x * 2 + (Console.WindowWidth / 2 - 10), y);
                    Console.Write("  ");
                }
            }
            x = droppingBlock.xPos + droppingBlock.xOffsets[i];
            y = droppingBlock.yPos + droppingBlock.yOffsets[i];
            if (y >= 0)
            {
                Console.BackgroundColor = droppingBlock.c;
                Console.SetCursorPosition(x * 2 + (Console.WindowWidth / 2 - 10), y);
                Console.Write("  ");
            }
        }
        prevX = droppingBlock.xPos;
        prevY = droppingBlock.yPos;
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
        if (canChange&&!hardDrop)
        {
            if (cki.Key == ConsoleKey.LeftArrow || cki.Key == ConsoleKey.A)
            {
                if (blockDropping&&droppingBlock.xPos>droppingBlock.minX) {
                    droppingBlock.xPos -= 1;
                }
            }else if (cki.Key == ConsoleKey.RightArrow || cki.Key == ConsoleKey.D)
            {
                if (blockDropping && droppingBlock.xPos < droppingBlock.maxX-1)
                {
                    droppingBlock.xPos += 1;
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
}
