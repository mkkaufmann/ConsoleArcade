using System;

public class Point
{
    public int x;
    public int y;
    public Point(int _x, int _y)
    {
        x = _x;
        y = _y;
    }

    public static bool operator ==(Point p1, Point p2)
    {
        if (p1.x == p2.x && p1.y == p2.y)
            return true;
        return false;
    }
    public static bool operator !=(Point p1, Point p2)
    {
        if (!(p1.x == p2.x && p1.y == p2.y))
            return true;
        return false;
    }
}
