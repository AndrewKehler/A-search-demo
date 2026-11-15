using Godot;
using System;

public partial class Globals : Node
{
    public static bool startSet = false;
    public static bool endSet = false;
    public static Vector2I startPos = new Vector2I(-1, -1);
    public static Vector2I endPos = new Vector2I(-1,-1);

    public static int VOIDT = 0;
    public static int BASET = 1;
    public static int WALLT = 2;
    public static int STARTT = 3;
    public static int ENDT = 4;

    public static (string, Vector2I, int)[] tilesDic = [
        ("voidTile",new Vector2I(-1, -1), 0),
        ("baseTile",new Vector2I(0,0), 1),
        ("wallTile",new Vector2I(1,0), 100), 
        ("startTile",new Vector2I(0,1), 0), 
        ("endTIle",new Vector2I(1,1), -1)
    ];


    public static int getWeight(Vector2I atlas)
    {   
        int weight = 1;
        for(int i =0; i< tilesDic.Length; i++)
        {
            if(atlas == tilesDic[i].Item2)
            {
                weight = tilesDic[i].Item3;
            }
        }
        return weight;
    }
    //Evaluates the heuristic value of a given tile to the end tile.
    public static double evalH(Vector2I tile, Vector2I end)
    {
        return Math.Pow((Math.Pow(tile.X - end.X, 2)+ Math.Pow(tile.Y - end.Y, 2)), .5);
    }
}
