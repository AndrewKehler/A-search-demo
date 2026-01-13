using Godot;
using System;

public partial class TilesMain : TileMapLayer
{
	public static Vector2I emptyTile = new Vector2I(-1,-1);

	public static Vector2I baseTile = new Vector2I(0,0);
	public static Vector2I wallTile = new Vector2I(1,0);
	public static Vector2I startTile = new Vector2I(0,1);
	public static Vector2I endTile = new Vector2I(1,1);




}
