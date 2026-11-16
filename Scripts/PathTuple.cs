using Godot;
using System;
using System.Collections.Generic;

public partial class PathTuple : Node
{
    public List<TileTuple> path;
    public int cost;
    public PathTuple(List<TileTuple> tiles, int c)
    {
        path = tiles;
        cost = c;
    }
}
