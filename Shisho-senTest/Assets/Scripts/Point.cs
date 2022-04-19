using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point
{
    public int x;
    public int y;
    public int dir;
    public int turnCount;

    public Point before = null;

    public Point()
    {

    }

    public Point(int x, int y, int dir, int turnCount)
    {
        this.x = x;
        this.y = y;
        this.dir = dir;
        this.turnCount = turnCount;
    }
}
