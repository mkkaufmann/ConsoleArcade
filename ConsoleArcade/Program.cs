using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleArcade
{
    class Program
    {
        private static void ShowMenu()
        {
            string[] lines = new string[15];
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
            lines[10]= "|                            |";
            lines[11]= "|    Press 5 for Hi-Scores   |";
            lines[12]= "|                            |";
            lines[13]= "|      Press Q to Quit       |";
            lines[14]="\\============================/";
            for (int i = 0; i < 15; i++)
            {
                Console.SetCursorPosition(45, i + 7);
                Console.Write(lines[i]);
            }
            while (true)
            {
                ConsoleKeyInfo cki = Console.ReadKey(true);
                if(cki.Key == ConsoleKey.D1)
                {

                }else if (cki.Key == ConsoleKey.D2)
                {

                }else if (cki.Key == ConsoleKey.D3)
                {

                }else if (cki.Key == ConsoleKey.D4)
                {

                }else if (cki.Key == ConsoleKey.D5)
                {

                }else if (cki.Key == ConsoleKey.Q)
                {
                    return;
                }
            }
        }
        private static void HideMenu()
        {
            for (int i = 0; i < 15; i++)
            {
                Console.SetCursorPosition(45, i + 7);
                for(int j = 0; j<30; j++)
                    Console.Write(" ");
            }
        }
        static void Main(string[] args)
        {
            ShowMenu();
        }
    }
}
