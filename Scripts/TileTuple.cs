using Godot;
using System;

public partial class TileTuple : Node
{
    public Vector2I coords;
    public double heuristic;
    public TileTuple(Vector2I c, double h)
    {
        coords = c;
        heuristic = h;
    }
}
