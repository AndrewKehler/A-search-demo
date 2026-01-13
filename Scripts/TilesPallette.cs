using Godot;
using System;
using System.Numerics;

public partial class TilesPallette : TileMapLayer
{
	[Export] private NodePath canvasLayerPath;
	private TileMapLayer canvasLayer;
	private Vector2I selectedCell;
	public override void _Ready()
	{
		canvasLayer = GetNode<TileMapLayer>(canvasLayerPath);
		selectedCell = new Vector2I(0,0);
	}

	public override void _Input(InputEvent @event)
	{
		if(@event is InputEventMouseButton eventMouse)
		{
			if(eventMouse.Pressed && eventMouse.ButtonIndex == MouseButton.Left)
			{
				Godot.Vector2 mousePos = GetLocalMousePosition();
				Vector2I cellPos = LocalToMap(mousePos);
				if(GetCellSourceId(cellPos) != -1)
				{
					selectedCell = GetCellAtlasCoords(cellPos);
					GD.Print(selectedCell);
				}else if(canvasLayer.GetCellSourceId(cellPos) != -1)
				{
					Vector2I hoveredCell = canvasLayer.GetCellAtlasCoords(cellPos);
					GD.Print(hoveredCell);

					if(selectedCell != new Vector2I(0,1) && selectedCell != new Vector2I(1,1)){
						switch(hoveredCell){
							case Vector2I(0,1):
								Globals.startSet = false;
								Globals.startPos = new Vector2I(-1,-1);
								break;
							case Vector2I(1,1):
								Globals.endSet = false;
								break;
						}
						canvasLayer.SetCell(cellPos, 0, selectedCell);
					}else if(selectedCell == new Vector2I(0,1) && !Globals.startSet)
					{	
						if(hoveredCell == new Vector2I(1, 1))
						{
							Globals.endSet = false;
						}
						canvasLayer.SetCell(cellPos, 0, selectedCell);
						Globals.startSet = true;
						Globals.startPos = cellPos;
					}else if(selectedCell == new Vector2I(1,1) && !Globals.endSet)
					{
						if(hoveredCell == new Vector2I(0, 1))
						{
							Globals.startSet = false;
							Globals.startPos = new Vector2I(-1,-1);
						}
						canvasLayer.SetCell(cellPos, 0, selectedCell);
						Globals.endSet = true;
					}

					
				}

			}
		}
	}

	// public override void _Process(double delta)
	// {
	//     Vector2 mousePos = GetLocalMousePosition();
	//     Vector2I cellPos = LocalToMap(mousePos);

	// }

}
