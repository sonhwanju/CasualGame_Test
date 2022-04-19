using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    [SerializeField]
    private Camera mainCam;

    private Tile[,] tileLayout;
    private List<Point> pList = new List<Point>();
    private Tile firstTile = null;

    private int[,] dir = { { 1, 0 }, { 0, 1 }, { -1, 0 }, { 0, -1 } };

    private const int MAP_SIZE = 9;

    private bool isDragEnd = false;

    [SerializeField]
    private GameObject tilePrefab;

    private void Awake()
    {
        PoolManager.CreatePool<Tile>(tilePrefab, transform, 10);
    }

    private void Start()
    {
        tileLayout = new Tile[MAP_SIZE, MAP_SIZE];

        for (int i = 0; i < MAP_SIZE; i++)
        {
            for (int j = 0; j < MAP_SIZE; j++)
            {
                Tile t = PoolManager.GetItem<Tile>();
                t.TileType = TileManager.Instance.GetRandomTileType();
                t.ChangeColor((int)t.TileType);
                t.transform.position = new Vector2(j, -i);
                tileLayout[j, i] = t;
            }
        }
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            (int x, int y) = GetXY();

            if (NotInBoard(x, y)) return;

            Point p = new Point(x, y);
            firstTile = tileLayout[x, y];
            print(x + ", " + y);

            TryAddPoint(p);
        }
        else if(Input.GetMouseButton(0))
        {
            if (isDragEnd) return;

            (int x, int y) = GetXY();

            if (NotInBoard(x, y)) return;

            Point p = new Point(x, y);
            print(x + ", " + y);

            if(EqualPoint(p)) //같은 타일인지 확인한다
            {
                if(!TryAddPoint(p) && !pList[pList.Count - 1].Equals(p)) //이미 있는 놈인지 확인 및 되돌아갔는지 확인
                {
                    isDragEnd = true;
                }
            }
            else
            {
                isDragEnd = true;
            }

        }
        else if(Input.GetMouseButtonUp(0))
        {
            if(pList.Count > 2)
            {
                foreach (Point p in pList)
                {
                    tileLayout[p.x, p.y].TileType = TileType.None;
                }
                DropBlocks();
            }

            isDragEnd = false;
            firstTile = null;
            pList.Clear();
        }
    }

    public bool CanMatchedBlocks()
    {
        int count = 0;

        for (int x = 0; x < MAP_SIZE; x++)
        {
            for (int y = 0; y < MAP_SIZE; y++)
            {
                Tile t = tileLayout[x, y];

                for (int d = 0; d < dir.GetLength(0); d++)
                {
                    int nextX = x + dir[d,0];
                    int nextY = y + dir[d, 1];

                    if (NotInBoard(nextX, nextY)) continue;

                    if (t.TileType == tileLayout[nextX, nextY].TileType) count++;
                }

                if(count > 1)
                {
                    return true;
                }

                count = 0;
            }
        }

        return false;
    }


    public void DropBlocks()
    {
        for (int i = MAP_SIZE - 1; i >= 0; i--)
        {
            DropBlock(i);
        }
        ChangeBlock();
    }

    public void DropBlock(int x)
    {
        for (int y = MAP_SIZE - 1; y >= 0; y--)
        {
            Tile t = tileLayout[x,y];

            if (y - 1 < 0) continue;

            int idx = 0;
            while (t.TileType == TileType.None)
            {
                for (int y2 = y; y2 >= 0; y2--)
                {
                    if(y2 -1 < 0)
                    {
                        tileLayout[x, y2].TileType = TileType.None;
                        break;
                    }

                    tileLayout[x, y2].TileType = tileLayout[x, y2 - 1].TileType;
                    tileLayout[x, y2].ChangeColor((int)tileLayout[x, y2].TileType);
                }
                idx++;

                if (idx >= MAP_SIZE) return;
            }

        }
    }

    public void ChangeBlock()
    {
        for (int x = 0; x < MAP_SIZE; x++)
        {
            for (int y = 0; y < MAP_SIZE; y++)
            {
                Tile t = tileLayout[x,y];

                if(t.TileType == TileType.None)
                {
                    t.TileType = TileManager.Instance.GetRandomTileType();
                    t.ChangeColor((int)t.TileType);
                }
            }
        }

        int cnt = 0;
        while (!CanMatchedBlocks())
        {
            InitBlock();
            cnt++;
            if(cnt >= 10000)
            {
                return;
            }
        }
    }
    public void InitBlock()
    {
        for (int x = 0; x < MAP_SIZE; x++)
        {
            for (int y = 0; y < MAP_SIZE; y++)
            {
                Tile t = tileLayout[x, y];

                t.TileType = TileManager.Instance.GetRandomTileType();
                t.ChangeColor((int)t.TileType);
            }
        }
    }

    public (int,int) GetXY()
    {
        Vector2 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        return (Mathf.RoundToInt(mousePos.x), Mathf.RoundToInt(-mousePos.y));
    }

    public bool TryAddPoint(Point p)
    {
        foreach (Point point in pList)
        {
            if(p.Equals(point))
            {
                return false;
            }
        }

        pList.Add(p);

        return true;
    }

    public bool EqualPoint(Point p)
    {
        return (firstTile != null || tileLayout[p.x,p.y] != null) && firstTile.TileType == tileLayout[p.x, p.y].TileType;
    }

    public bool NotInBoard(int x, int y)
    {
        return x < 0 || y < 0 || x >= MAP_SIZE || y >= MAP_SIZE;
    }
}
