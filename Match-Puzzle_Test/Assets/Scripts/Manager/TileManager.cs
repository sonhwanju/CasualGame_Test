using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance { get; private set; }

    private Dictionary<int, Color> tileDic = new Dictionary<int, Color>();


    private void Awake()
    {
        Instance = this;
        string[] name = System.Enum.GetNames(typeof(TileType));
        for (int i = 1; i <= (int)TileType.Magenta; i++)
        {
            tileDic.Add(i, StringToColor(name[i]));
        }
    }

    public Color StringToColor(string color)
    {
        return (Color)typeof(Color).GetProperty(color.ToLower()).GetValue(null);
    }

    public Color GetColor(int idx)
    {
        if (idx == 0) return Color.clear;

        return tileDic[idx];
    }

    public TileType GetRandomTileType()
    {
        int idx = Random.Range(1, Enum.GetValues(typeof(TileType)).Length);

        return (TileType)idx;
    }
}
