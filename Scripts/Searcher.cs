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
	private Stack<TileTuple> toVisit = new Stack<TileTuple>();
	private PathTuple curMinPath = new PathTuple(new List<TileTuple>(), -1);
	// private TileTuple[] path = new TileTuple[256];
	

	public override void _Ready()
	{   
		this.Points = [];
		canvasLayer = GetNode<TileMapLayer>(canvasPath);
		palletteLayer = GetNode<TilesPallette>(palettePath);
		palletteLayer.StartSearch += StartSearch;

	}


	private void StartSearch(Vector2I startPos, Vector2I endPos)
	{	
		GD.Print("started!");
		PathTuple path;
		TileTuple startTile = new TileTuple(startPos, 0, 0);
		path = findPath(startTile, endPos, new PathTuple(new List<TileTuple>(), 0));
		for(int i=0; i<path.path.Count; i++)
		{
			this.AddPoint(canvasLayer.MapToLocal(path.path[i].coords));
			GD.Print(path.path[i].coords, ", ", canvasLayer.MapToLocal(path.path[i].coords));
		}

	}

	//this currently needs a system in place to check whether all of a tiles adjacent tiles have been checked on the current path order...
	private PathTuple findPath(TileTuple curTile, Vector2I endPos, PathTuple curPath)
	{	
		//get the travel weight of our tile.
		int curTileWeight = Globals.getWeight(canvasLayer.GetCellAtlasCoords(curTile.coords));
		
		//update cost of current path each pass.
		int totalCost = 0;
		for(int i=0; i < curPath.path.Count; i++)
		{
			totalCost +=  Globals.getWeight(canvasLayer.GetCellAtlasCoords(curPath.path[i].coords));
		}
		curPath.cost = totalCost;

	//(BASE CASE): 
		// if we are on the end tile:
		if(canvasLayer.GetCellAtlasCoords(curTile.coords) == Globals.ENDTILE)
		{	
			//and there is no current minimum path, 
			// or the old path is worse than our current one and there is more to visit:
			if(curMinPath.cost == -1 || (curMinPath.cost > curPath.cost && toVisit.Count > 0))
			{
				//copy the curPath to the curMinPath, then add the end tile to the end of the min path
				curMinPath = new PathTuple(new List<TileTuple>(curPath.path), curPath.cost);
				curMinPath.path.Add(curTile);
				//Go to a previous adjacent tile to continue to check all possible paths
				curPath.path[curPath.path.Count - 1].checkedAdj--;
				return findPath(toVisit.Pop(), endPos, curPath);
			}
			//the old path is better or equal to our current one
			else if(curMinPath.cost <= curPath.cost)
			{
				//if theres more places to go
				if(toVisit.Count > 0)
				{
					//dont update the path and go back to a new adjacent tile
					curPath.path[0].checkedAdj--;
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
				curMinPath = new PathTuple(new List<TileTuple>(curPath.path), curPath.cost);
				curMinPath.path.Add(curTile);
				//give us the minimum path
				return curMinPath;
			}
			
		}
		//If we've moved to a tile, a minimum path exists, and our path cost will be greater after travelling to this tile:
		else if(curMinPath.cost != -1 && curPath.cost + curTileWeight > curMinPath.cost)
		{
			//check if all adjacent tiles have been checked for the head of the path
			if(curPath.path[curPath.path.Count - 1].checkedAdj -1 <= 0)
			{	
				//if so, remove that tile from the current path and update the weight of the current path
				curPath.cost -= Globals.getWeight(curPath.path[curPath.path.Count - 1].coords);
				curPath.path.RemoveAt(curPath.path.Count - 1);
			}
			else
			{
				curPath.path[curPath.path.Count - 1].checkedAdj--;
			}
			if(toVisit.Count > 0)
			{
				return findPath(toVisit.Pop(), endPos, curPath);
			}else{
				return curMinPath;
			}
		}


		
		
		//call helper to check adjacent tiles of current tile.
		TileTuple[] curAdj = checkAdjacent(curTile.coords, endPos);

		//iterate backwards pushing each adjacent tile to a stack ignoring non-existent tiles, or already traversed tiles.
		//Starts with lowest heuristic value.
		for(int i=5; i>-1; i--)
		{	
			if(canvasLayer.GetCellAtlasCoords(curAdj[i].coords) != Globals.VOIDTILE && !containsCoords(curPath, curAdj[i].coords))
			{
				toVisit.Push(curAdj[i]);
				curTile.checkedAdj++;
			}
		}

		//add current tile to path along with its travel cost

		curPath.cost = curTileWeight;
		curPath.path.Add(curTile);
		// GD.Print(curPath.path[curPath.path.Count - 1].coords);
		// curPath.path[curPath.path.Count - 1].checkedAdj--;
		return findPath(toVisit.Pop(), endPos, curPath);
	}

	// Checks each adjacent tile to the current tile and calculates its heuristic value. 
	// Hexagonal grid checks change depending on X value. 
	private TileTuple[] checkAdjacent(Vector2I curTile, Vector2I endPos)
	{
		TileTuple[] adjacent = new TileTuple[6];
		adjacent[0] = new TileTuple(new Vector2I(curTile.X, curTile.Y+1), Globals.evalH(new Vector2I(curTile.X, curTile.Y+1), endPos), 0);
		adjacent[1] = new TileTuple(new Vector2I(curTile.X, curTile.Y-1), Globals.evalH(new Vector2I(curTile.X, curTile.Y-1), endPos), 0);
		adjacent[2] = new TileTuple(new Vector2I(curTile.X+1, curTile.Y), Globals.evalH(new Vector2I(curTile.X+1, curTile.Y), endPos), 0);
		adjacent[3] = new TileTuple(new Vector2I(curTile.X-1, curTile.Y), Globals.evalH(new Vector2I(curTile.X-1, curTile.Y), endPos), 0);
		if(curTile.X%2 == 0)
		{
			adjacent[4] = new TileTuple(new Vector2I(curTile.X-1, curTile.Y-1), Globals.evalH(new Vector2I(curTile.X-1, curTile.Y-1), endPos), 0);;
			adjacent[5] = new TileTuple(new Vector2I(curTile.X+1, curTile.Y-1), Globals.evalH(new Vector2I(curTile.X+1, curTile.Y-1), endPos), 0);
		}
		else
		{
			adjacent[4] = new TileTuple(new Vector2I(curTile.X-1, curTile.Y+1), Globals.evalH(new Vector2I(curTile.X-1, curTile.Y+1), endPos), 0);
			adjacent[5] = new TileTuple(new Vector2I(curTile.X+1, curTile.Y+1), Globals.evalH(new Vector2I(curTile.X+1, curTile.Y+1), endPos), 0);
		}
		//sort by lowest to highest heuristic value
		Array.Sort(adjacent, (x, y) => x.heuristic.CompareTo(y.heuristic));

		return adjacent;

	}

	private bool containsCoords(PathTuple path, Vector2I key)
	{	
		for(int i=0; i < path.path.Count; i++)
		{
			if(path.path[i].coords == key)
			{
				return true;
			}
		}
		return false;
	}
}
