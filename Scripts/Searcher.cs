using Godot;
using System;

public partial class Searcher : Line2D
{
	[Export] private NodePath pallettePath;
	private TilesPallette palletteLayer;
	private Vector2I[] nodes = new Vector2I[256];

	public override void _Ready()
	{   
		palletteLayer = GetNode<TilesPallette>(pallettePath);
		palletteLayer.StartSearch += StartSearch;

	}


	private void StartSearch(Vector2I startPos, Vector2I endPos)
	{
		this.AddPoint(startPos);
		this.AddPoint(endPos);
		GD.Print(this.GetPointCount());
	}
}
