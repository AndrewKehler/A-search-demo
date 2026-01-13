using Godot;
using System;

public partial class TilesHover : TileMapLayer
{
	[Export] private NodePath mainLayerPath;
	private TileMapLayer mainLayer;
	private Vector2I minCell = new Vector2I(int.MinValue, int.MinValue);
	private Vector2I lastCell;
	public override void _Ready()
	{
		mainLayer = GetNode<TileMapLayer>(mainLayerPath);
		lastCell = minCell;
	}

	public override void _Process(double delta)
	{
		
		Vector2 mousePos = GetLocalMousePosition();
		Vector2I cellPos = LocalToMap(mousePos);
		if(cellPos == lastCell)
		{
			return;
		}
		Clear();
		if(mainLayer.GetCellSourceId(cellPos) != -1){
			this.SetCell(cellPos, 0, new Vector2I(1, 0));
		}
		lastCell = cellPos;
		// clearHover();
	}

	private void clearHover()
	{
		if(lastCell.X != int.MinValue)
		{
			EraseCell(lastCell);
			lastCell = minCell;
		}
	}


}
