﻿using System;

public class MainMenu : Menu
{
	public MainMenu():base()
    {
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
        lines[10] = "|                            |";
        lines[11] = "|    Press 5 for Hi-Scores   |";
        lines[12] = "|                            |";
        lines[13] = "|      Press Q to Quit       |";
        lines[14] = "\\============================/";
    }
}