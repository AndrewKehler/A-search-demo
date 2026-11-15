using Godot;
using System;

public partial class PathTuple : Node
{
    public Vector2I[] path;
    public int cost;
    public PathTuple(Vector2I[] tiles, int c)
    {
        path = tiles;
        cost = c;
    }
}
