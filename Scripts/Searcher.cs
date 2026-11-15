using Godot;
using System;
using System.Collections.Generic;

public partial class Searcher : Line2D
{
	[Export] private NodePath canvasPath;
	[Export] private NodePath palettePath;
	private TilesPallette palletteLayer;
	private TileMapLayer canvasLayer;
	// private (Vector2I, double)[] curAdj = new (Vector2I, double)[6];
	private Queue<Vector2I> toVisit = new Queue<Vector2I>();
	private (TileTuple[], int) curMinPath;
	private TileTuple[] path = new TileTuple[256];

	public override void _Ready()
	{   
		canvasLayer = GetNode<TileMapLayer>(canvasPath);
		palletteLayer = GetNode<TilesPallette>(palettePath);
		palletteLayer.StartSearch += StartSearch;

	}


	private void StartSearch(Vector2I startPos, Vector2I endPos)
	{
		bool foundEnd = false;
	
		createPath(startPos, endPos);
		// while (!foundEnd){
			
		// }
		
		
		
	}

	private TileTuple[] createPath(Vector2I curTile, Vector2I endPos)
	{
		(Vector2I, double)[] curAdj = new (Vector2I, double)[6];
		curAdj[0] = (new Vector2I(curTile.X, curTile.Y+1), Globals.evalH(new Vector2I(curTile.X, curTile.Y+1), endPos));
		curAdj[1] = (new Vector2I(curTile.X, curTile.Y-1), Globals.evalH(new Vector2I(curTile.X, curTile.Y-1), endPos));
		curAdj[2] = (new Vector2I(curTile.X+1, curTile.Y), Globals.evalH(new Vector2I(curTile.X+1, curTile.Y), endPos));
		curAdj[3] = (new Vector2I(curTile.X-1, curTile.Y), Globals.evalH(new Vector2I(curTile.X-1, curTile.Y), endPos));
		if(curTile.X%2 == 0)
		{
			curAdj[4] = (new Vector2I(curTile.X-1, curTile.Y-1), Globals.evalH(new Vector2I(curTile.X-1, curTile.Y-1), endPos));
			curAdj[5] = (new Vector2I(curTile.X+1, curTile.Y-1), Globals.evalH(new Vector2I(curTile.X+1, curTile.Y-1), endPos));
		}
		else
		{
			curAdj[4] = (new Vector2I(curTile.X-1, curTile.Y+1), Globals.evalH(new Vector2I(curTile.X-1, curTile.Y-1), endPos));
			curAdj[5] = (new Vector2I(curTile.X+1, curTile.Y+1), Globals.evalH(new Vector2I(curTile.X+1, curTile.Y-1), endPos));
		}
		
		
		Array.Sort(curAdj, (x, y) => x.Item2.CompareTo(y.Item2));
		for(int i=0; i<6; i++)
		{
			if(canvasLayer.GetCellAtlasCoords(curAdj[i].Item1) != new Vector2I(-1, -1))
			{
				toVisit.Enqueue(curAdj[i].Item1);
				GD.Print(curAdj[i]);
			}
			
		}

		return null;
	}
}
