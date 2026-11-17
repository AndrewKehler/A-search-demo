using Godot;
using System;
using System.Numerics;

public partial class TilesPallette : TileMapLayer
{
	[Export] private NodePath canvasLayerPath;
	private TileMapLayer canvasLayer;
	private Vector2I selectedCell;
	private Vector2I BASETILE = new Vector2I(0,0);
	private Vector2I WALLTILE = new Vector2I(1,0);
	private Vector2I STARTTILE = new Vector2I(0,1);
	private Vector2I ENDTILE = new Vector2I(1,1);

	private Vector2I curEndPos;
	
	private Vector2I curStartPos;

	[Signal]
	public delegate void StartSearchEventHandler(Vector2I startPos, Vector2I endPos);
	[Signal]
	public delegate void GoalsRemovedEventHandler();
	public override void _Ready()
	{
		canvasLayer = GetNode<TileMapLayer>(canvasLayerPath);
		selectedCell = BASETILE;
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

					if(selectedCell != STARTTILE && selectedCell != ENDTILE){
						switch(hoveredCell){
							case Vector2I(0,1):
								Globals.startSet = false;
								Globals.startPos = new Vector2I(-1,-1);
								EmitSignal(SignalName.GoalsRemoved);
								
								break;
							case Vector2I(1,1):
								Globals.endSet = false;
								Globals.endPos = new Vector2I(-1,-1);
								EmitSignal(SignalName.GoalsRemoved);

								break;
						}
						canvasLayer.SetCell(cellPos, 0, selectedCell);
					}else if(selectedCell == STARTTILE)
					{	
						if(hoveredCell == ENDTILE)
						{
							Globals.endSet = false;
							Globals.endPos = new Vector2I(-1,-1);
							EmitSignal(SignalName.GoalsRemoved);
						}
						if (Globals.startSet)
						{
							canvasLayer.SetCell(Globals.startPos, 0, BASETILE);
						}
						canvasLayer.SetCell(cellPos, 0, selectedCell);
						Globals.startSet = true;
						Globals.startPos = cellPos;
						if(Globals.startSet && Globals.endSet)
						{
							EmitSignal(SignalName.StartSearch, Globals.startPos, Globals.endPos);
						}
					}
					else if(selectedCell == ENDTILE)
					{
						if(hoveredCell == STARTTILE)
						{
							Globals.startSet = false;
							Globals.startPos = new Vector2I(-1,-1);
							EmitSignal(SignalName.GoalsRemoved);
						}
						if (Globals.endSet)
						{
							canvasLayer.SetCell(Globals.endPos, 0, BASETILE);
						}
						canvasLayer.SetCell(cellPos, 0, selectedCell);
						Globals.endSet = true;
						Globals.endPos =cellPos;
						if(Globals.startSet && Globals.endSet)
						{
							EmitSignal(SignalName.StartSearch, Globals.startPos, Globals.endPos);
						}
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
