using Godot;
using System;

public partial class TileTuple : Node
{
    public Vector2I coords;
    public double heuristic;
    public int checkedAdj;
    public TileTuple(Vector2I c, double h, int a)
    {
        coords = c;
        heuristic = h;
        checkedAdj = a;
    }
}
