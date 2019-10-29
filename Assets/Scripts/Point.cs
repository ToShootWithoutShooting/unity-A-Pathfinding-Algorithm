using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point {
    public Point Parent { get; set; }
    public float F { get; set; }
    public float G { get; set; }
    public float H { get; set; }

    public int X { get; set; }
    public int Y { get; set; }

    public bool IsWall { get; set; }

    public Point(int x,int y , bool isWall)
    {
        this.X = x;
        this.Y = y;
        this.Parent = null;
        IsWall = isWall;
    }
    public void UpdateParent(Point parent,float g)
    {
        this.Parent = parent;
        this.G = g;
        F = G + H;
    }
}
