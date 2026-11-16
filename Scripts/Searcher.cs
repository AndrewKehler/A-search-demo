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
	private Stack<Vector2I> toVisit = new Stack<Vector2I>();
	private PathTuple curMinPath = new PathTuple(new List<Vector2I>(), -1);
	// private TileTuple[] path = new TileTuple[256];
	

	public override void _Ready()
	{   
		canvasLayer = GetNode<TileMapLayer>(canvasPath);
		palletteLayer = GetNode<TilesPallette>(palettePath);
		palletteLayer.StartSearch += StartSearch;

	}


	private void StartSearch(Vector2I startPos, Vector2I endPos)
	{
		bool foundEnd = false;
		// while (toVisit.Count > 0)
		// {
			
		// }
		findPath(startPos, endPos, new PathTuple(new List<Vector2I>(), 0));
		// while (!foundEnd){
			
		// }
		
		
		
	}

	private PathTuple findPath(Vector2I curTile, Vector2I endPos, PathTuple curPath)
	{	
		//get the travel weight of our tile.
		int curTileWeight = Globals.getWeight(canvasLayer.GetCellAtlasCoords(curTile));

		//(BASE CASE) if we are on the end tile:
		if(canvasLayer.GetCellAtlasCoords(curTile) == Globals.ENDTILE)
		{	
			//and there is no current minimum path, 
			// or the old path is worse than our current one and there is more to visit:
			if(curMinPath.cost == -1 || (curMinPath.cost > curPath.cost && toVisit.Count > 0))
			{
				//copy the curPath to the curMinPath, then add the end tile to the end of the min path
				curMinPath = new PathTuple(new List<Vector2I>(curPath.path), curPath.cost);
				curMinPath.path.Add(curTile);
				//Go to a previous adjacent tile to continue to check all possible paths
				return findPath(toVisit.Pop(), endPos, curPath);
			}
			//the old path is better than our current one
			else if(curMinPath.cost < curPath.cost)
			{
				//if theres more places to go
				if(toVisit.Count > 0)
				{
					//dont update the path and go back to a new adjacent tile
					return findPath(toVisit.Pop(), endPos, curPath);
				}
				// if there isnt any places to go
				else
				{	
					//give us the minimum path
					return curMinPath;
				}
			}
			// the minimum path is worse than out current one, and there is no more places to go.
			else
			{
				//copy the curPath to the curMinPath, then add the end tile to the end of the min path
				curMinPath = new PathTuple(new List<Vector2I>(curPath.path), curPath.cost);
				curMinPath.path.Add(curTile);
				//give us the minimum path
				return curMinPath;
			}
			
		}

		//If we've moved to a tile, a minimum path exists, and our path cost will be greater after travelling to this tile:
		else if(curMinPath.cost != -1 && curPath.cost + curTileWeight > curMinPath.cost)
		{
			// We ignore it and go to a different adjacent tile instead.
			findPath(toVisit.Pop(), endPos, curPath);
		}


		//add current tile to path along with its travel cost
		curPath.path.Add(curTile);
		curPath.cost += Globals.getWeight(canvasLayer.GetCellAtlasCoords(curTile));
		
		//call helper to check adjacent tiles of current tile.
		TileTuple[] curAdj = checkAdjacent(curTile, endPos);

		//iterate backwards pushing each adjacent tile to a stack ignoring non-existent tiles.
		//Starts with lowest heuristic value.
		for(int i=5; i>-1; i--)
		{	
			if(canvasLayer.GetCellAtlasCoords(curAdj[i].coords) != Globals.VOIDTILE)
			{
				toVisit.Push(curAdj[i].coords);
			}
		}
		return null;
	}

	// Checks each adjacent tile to the current tile and calculates its heuristic value. 
	// Hexagonal grid checks change depending on X value. 
	private TileTuple[] checkAdjacent(Vector2I curTile, Vector2I endPos)
	{
		TileTuple[] adjacent = new TileTuple[6];
		adjacent[0] = new TileTuple(new Vector2I(curTile.X, curTile.Y+1), Globals.evalH(new Vector2I(curTile.X, curTile.Y+1), endPos));
		adjacent[1] = new TileTuple(new Vector2I(curTile.X, curTile.Y-1), Globals.evalH(new Vector2I(curTile.X, curTile.Y-1), endPos));
		adjacent[2] = new TileTuple(new Vector2I(curTile.X+1, curTile.Y), Globals.evalH(new Vector2I(curTile.X+1, curTile.Y), endPos));
		adjacent[3] = new TileTuple(new Vector2I(curTile.X-1, curTile.Y), Globals.evalH(new Vector2I(curTile.X-1, curTile.Y), endPos));
		if(curTile.X%2 == 0)
		{
			adjacent[4] = new TileTuple(new Vector2I(curTile.X-1, curTile.Y-1), Globals.evalH(new Vector2I(curTile.X-1, curTile.Y-1), endPos));;
			adjacent[5] = new TileTuple(new Vector2I(curTile.X+1, curTile.Y-1), Globals.evalH(new Vector2I(curTile.X+1, curTile.Y-1), endPos));
		}
		else
		{
			adjacent[4] = new TileTuple(new Vector2I(curTile.X-1, curTile.Y+1), Globals.evalH(new Vector2I(curTile.X-1, curTile.Y+1), endPos));
			adjacent[5] = new TileTuple(new Vector2I(curTile.X+1, curTile.Y+1), Globals.evalH(new Vector2I(curTile.X+1, curTile.Y+1), endPos));
		}
		//sort by lowest to highest heuristic value
		Array.Sort(adjacent, (x, y) => x.heuristic.CompareTo(y.heuristic));

		return adjacent;

	}
}

