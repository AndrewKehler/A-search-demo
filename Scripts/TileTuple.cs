using Godot;
using System;

public partial class TileTuple : Node
{
    public Vector2I atlas;
    public int weight;
    public TileTuple(Vector2I coords, int w)
    {
        atlas = coords;
        weight = w;
    }
}
