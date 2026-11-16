using Godot;
using System;
using System.Collections.Generic;

public partial class PathTuple : Node
{
    public List<Vector2I> path;
    public int cost;
    public PathTuple(List<Vector2I> tiles, int c)
    {
        path = tiles;
        cost = c;
    }
}
