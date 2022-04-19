using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    None = 0,
    Red,
    Blue,
    Green,
    Yellow,
    White,
    Grey,
    Magenta
}

public class Tile : MonoBehaviour
{
    public SpriteRenderer sr;

    [SerializeField]
    private TileType tileType;
    public TileType TileType
    {
        get => tileType;
        set => tileType = value;
    }

    public void ChangeColor(int idx)
    {
        sr.color = TileManager.Instance.GetColor(idx);
    }
}

public class Point
{
    public int x;
    public int y;

    public Point()
    {

    }

    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;

    }

    public bool Equals(Point other)
    {
        return other.x == this.x && other.y == this.y;
    }
}