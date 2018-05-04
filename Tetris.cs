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
    List<SquareLoc> tetris = new List<SquareLoc>();
    struct SquareLoc
    {
        public Square s;
        public Block b;
        public SquareLoc(Square s, Block b)
        {
            this.s = s;
            this.b = b;
        }
    }
    enum Rotation
    {
        r0,
        r90,
        r180,
        r270
    }
    private class Square
    {
        public int x;
        public int y;
        public Square(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
    private class Block
    {
        public int xPos;
        public int yPos;
        public List<Square> offsets;
        public int minX;
        public int maxX;
        public ConsoleColor c;
        public Rotation r;
        public virtual int xByR(int i)
        {
            switch (r)
            {
                case Rotation.r90:
                    return xPos + offsets[i].y;
                case Rotation.r180:
                    return xPos - offsets[i].x;
                case Rotation.r270:
                    return xPos - offsets[i].y;
                default:
                    return xPos + offsets[i].x;
            }
        }
        public virtual int yByR(int i)
        {
            switch (r)
            {
                case Rotation.r90:
                    return yPos - offsets[i].x;
                case Rotation.r180:
                    return yPos - offsets[i].y;
                case Rotation.r270:
                    return yPos + offsets[i].x;
                default:
                    return yPos + offsets[i].y;
            }
        }
        public virtual int xByR(int i, Rotation r)
        {
            switch (r)
            {
                case Rotation.r90:
                    return xPos + offsets[i].y;
                case Rotation.r180:
                    return xPos - offsets[i].x;
                case Rotation.r270:
                    return xPos - offsets[i].y;
                default:
                    return xPos + offsets[i].x;
            }
        }
        public virtual int yByR(int i,Rotation r)
        {
            switch (r)
            {
                case Rotation.r90:
                    return yPos - offsets[i].x;
                case Rotation.r180:
                    return yPos - offsets[i].y;
                case Rotation.r270:
                    return yPos + offsets[i].x;
                default:
                    return yPos + offsets[i].y;
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
            offsets = new List<Square> { new Square(0,1), new Square(0,0), new Square(0,-1), new Square(0,-2) };
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
            offsets = new List<Square> { new Square(0, 0), new Square(-1, -1), new Square(1, 0), new Square(-1, 0) };
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
            offsets = new List<Square> { new Square(-1, 0), new Square(0, 0), new Square(1, 0), new Square(1, -1) };
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
            offsets = new List<Square> { new Square(1, 0), new Square(0, 0), new Square(0,-1), new Square(1, -1) };
            minX = 0;
            maxX = 9;
        }
        public override int xByR(int i)
        {
            return xPos + offsets[i].x;
        }
        public override int yByR(int i)
        {
            return yPos + offsets[i].y;
        }
        public override int xByR(int i,Rotation r)
        {
            return xPos + offsets[i].x;
        }
        public override int yByR(int i, Rotation r)
        {
            return yPos + offsets[i].y;
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
            offsets = new List<Square> { new Square(-1, 0), new Square(0, 0), new Square(1, -1), new Square(0, -1) };
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
            offsets = new List<Square> { new Square(-1, 0), new Square(0, 0), new Square(1, 0), new Square(0, -1) };
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
            offsets = new List<Square> { new Square(1, 0), new Square(0, 0), new Square(0, -1), new Square(-1, -1) };
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
        timer.Interval = 150;
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
        Console.BackgroundColor = ConsoleColor.Black;
        Console.Clear();
        for (int i = 1; i < 21; i++)
        {
            Console.SetCursorPosition(Console.WindowWidth / 2 - 12, i + Y_OFFSET+1);
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.Write("  ");
            Console.SetCursorPosition(Console.WindowWidth / 2 + 10, i + Y_OFFSET+1);
            Console.Write("  ");
        }
        for (int i = 0; i < 10; i++)
        {
            Console.SetCursorPosition(Console.WindowWidth / 2 - 10 + i * 2, 20 + Y_OFFSET+1);
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.Write("  ");
        }
    }
    private void RedrawBoard()
    {
        DrawFirst();
        foreach(Block b in blocks)
        {
            Console.BackgroundColor = b.c;
            for(int i = 0; i<b.offsets.Count; i++)
            {
                Console.SetCursorPosition(b.xByR(i) * 2 + (Console.WindowWidth / 2 - 10), b.yByR(i));
                Console.Write("  ");
            }
        }
    }
    private void Draw()
    {
        canChange = true;
        if (!blockDropping)
        {
            blockDropping = true;
            switch(rng.Next(0,7))
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
                timer.Interval = 150;
                for(int j = 0; j < droppingBlock.offsets.Count; j++)
                {
                    x = droppingBlock.xByR(j);
                    y = droppingBlock.yByR(j) + 1;
                    tetris.Clear();
                    tetris.Add(new SquareLoc(droppingBlock.offsets[j], droppingBlock));//last change
                    /*FIX WEIRD TETRIS OCCURENCES*/
                    foreach (Block b in blocks)
                    {
                        if(b == droppingBlock)
                        {
                            continue;
                        }
                        for(int k = 0; k < b.offsets.Count; k++)
                        {
                            if(b.yByR(k) == y)
                            {
                                tetris.Add(new SquareLoc(b.offsets[k], b));
                                if(tetris.Count == 10)
                                {
                                    foreach (SquareLoc sl in tetris)
                                    {
                                        sl.b.offsets.Remove(sl.s);
                                        if (sl.b.offsets.Count == 0)
                                        {
                                            blocks.Remove(b);
                                        }
                                    }
                                    foreach(Block b2 in blocks)
                                    {
                                        for(int l = 0; l<b2.offsets.Count; l++)
                                        {
                                            if (b2.yByR(l) < y)
                                            {
                                                b2.offsets[l].y += 1;
                                            }
                                        }
                                    }
                                    RedrawBoard();
                                    break;
                                }
                            }
                        }
                        if (tetris.Count == 10)
                        {
                            break;
                        }
                    }
                }
                return;
            }
            foreach (Block b in blocks)
            {
                if (b == droppingBlock)
                    continue;
                for (int k = 0; k < b.offsets.Count; k++)
                {
                    if ((b.xByR(k) == x && b.yByR(k) == y))
                    {
                        blockDropping = false;
                        hardDrop = false;
                        prevX = -1;
                        prevY = -1;
                        prevR = Rotation.r0;
                        timer.Interval = 100;
                        for (int j = 0; j < 4; j++)
                        {
                            tetris.Clear();
                            foreach (Block b1 in blocks)
                            {
                                for (int g = 0; g < b1.offsets.Count; g++)
                                {
                                    if (b1.yByR(g) == y)
                                    {
                                        tetris.Add(new SquareLoc(b1.offsets[g], b1));
                                        if (tetris.Count == 10)
                                        {
                                            foreach (SquareLoc sl in tetris)
                                            {
                                                sl.b.offsets.Remove(sl.s);
                                                if(sl.b.offsets.Count == 0)
                                                {
                                                    blocks.Remove(b);
                                                }
                                            }
                                            foreach (Block b2 in blocks)
                                            {
                                                for (int l = 0; l < b2.offsets.Count; l++)
                                                {
                                                    if (b2.yByR(l) < y)
                                                    {
                                                        b2.offsets[l].y += 1;
                                                    }
                                                }
                                            }
                                            RedrawBoard();
                                            break;
                                        }
                                    }
                                }
                                if (tetris.Count == 10)
                                {
                                    break;
                                }
                            }
                        }
                        for (int g = 0; g < droppingBlock.offsets.Count; g++)
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
                if (droppingBlock.GetType() != new BlockO().GetType())
                {
                    switch (prevR)
                    {
                        case Rotation.r90:
                            x = prevX + droppingBlock.offsets[i].y;
                            y = prevY - droppingBlock.offsets[i].x;
                            break;
                        case Rotation.r180:
                            x = prevX - droppingBlock.offsets[i].x;
                            y = prevY - droppingBlock.offsets[i].y;
                            break;
                        case Rotation.r270:
                            x = prevX - droppingBlock.offsets[i].y;
                            y = prevY + droppingBlock.offsets[i].x;
                            break;
                        default:
                            x = prevX + droppingBlock.offsets[i].x;
                            y = prevY + droppingBlock.offsets[i].y;
                            break;
                    }
                }
                else
                {
                    x = prevX + droppingBlock.offsets[i].x;
                    y = prevY + droppingBlock.offsets[i].y;
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
        if (blockDropping)
        {
            droppingBlock.yPos += 1;
        }
        RedrawBoard();
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
        if (!hardDrop && blockDropping)
        {
            if (canChange)
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
                            for (int k = 0; k < b.offsets.Count; k++)
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
                            for (int k = 0; k < b.offsets.Count; k++)
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
            }

            if (cki.Key == ConsoleKey.UpArrow || cki.Key == ConsoleKey.W)
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
                        for (int k = 0; k < b.offsets.Count; k++)
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
        `'-.        ,.-"`;/=\;"-.,_        .-'`
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
                        for (int k = 0; k < b.offsets.Count; k++)
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

