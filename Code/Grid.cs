using System;
using System.Collections.Generic;
using Godot;

namespace PalaSoliisi
{
	public partial class Grid : Node2D
	{
		[Export] private string _cellScenePath = "res://Levels/Cell.tscn";
		[Export] private int _width = 0;
		[Export] private int _height = 0;


		// Size of the cell (clickable area)
		[Export] private Vector2I _cellSize = Vector2I.Zero;

		public int Width
		{
			get {return _width;}
		}
		public int Height
		{
			get {return _height;}
		}

		// Save all cells from grid
		private Cell[,] _cells = null;
		private List<Cell> _freeCells = new List<Cell>();

		public Vector2 CellSize => new Vector2(_cellSize.X, _cellSize.Y);

		public Vector2 Offset
		{
			get
			{
				Vector2 offset = new Vector2((_width * _cellSize.X) / 2f, (_height * _cellSize.Y) / 2f);
				return offset;
			}
		}

		/// <summary>
		/// Initiate grid when scene opened
		/// </summary>
		public override void _Ready()
		{
			_cells = new Cell[Width, Height];

			// Grid offset point to start drawing from
			Vector2 offset = new Vector2((_width * _cellSize.X) / 2, (_height * _cellSize.Y) / 2);

			Vector2 halfNode = new Vector2(_cellSize.X / 2f, _cellSize.Y / 2f);

			offset.X -= halfNode.X;
			offset.Y -= halfNode.Y;

			// Load Cell scene
			PackedScene cellScene = ResourceLoader.Load<PackedScene>(_cellScenePath);
			if (cellScene == null)
			{
				GD.PrintErr("Cell sceneä ei löydy! Gridiä ei voi luoda!");
				return;
			}

			// Draw grid with given values
			for (int x = 0; x < _width; ++x)
			{
				for (int y = 0; y < _height; ++y)
				{
					if (y == 1)
					{
						continue;
					}
					// Initiate cell scene for grid cell that is being drawn
					Cell cell = cellScene.Instantiate<Cell>();
					// Add to node tree
					AddChild(cell);

					cell.Position = new Vector2(x * _cellSize.X, y * _cellSize.Y) - offset;
					cell.GridPosition = new Vector2I(x, y);

					_cells[x, y] = cell;
					_freeCells.Add(cell);
				}
			}
		}

		/// <summary>
		/// Check if coordinates valid in grid and return position
		/// </summary>
		/// <param name="gridPosition">Position in grid</param>
		/// <param name="worldPosition"></param>
		/// <returns>True if coordinates valid</returns>
		public bool GetWorldPosition(Vector2I gridPosition, out Vector2 worldPosition)
		{
			if (IsValidCoordinate(gridPosition))
    		{
				worldPosition = _cells[gridPosition.X, gridPosition.Y].Position;

				// Position valid
				return true;
			}

    		// Position not valid, return (0, 0)
			worldPosition = Vector2.Zero;
    		return false;

		}

		/// <summary>
		/// Check if clicked position valid in grid
		/// </summary>
		/// <param name="gridPosition">Click position in grid coordinates</param>
		/// <returns>True if coordinates valid in grid</returns>
		public bool IsValidCoordinate(Vector2I gridPosition)
		{
			if (gridPosition.X >= 0 && gridPosition.X < _width &&
        		gridPosition.Y >= 0 && gridPosition.Y < _height)
			{
				return true;
			}
			else {return false;}
		}

		/// <summary>
		/// Set cell occupier type for cell
		/// </summary>
		/// <param name="occupier">Cell occupier type</param>
		/// <param name="gridPosition">Position to occupy in grid</param>
		/// <returns>True if cell can be occupied</returns>
		public bool OccupyCell(ICellOccupier occupier, Vector2I gridPosition)
		{
			// Check if coordinates valid in grid
			if (!IsValidCoordinate(gridPosition))
			{
				return false;
			}

			Cell cell = _cells[gridPosition.X, gridPosition.Y];
			bool canOccupy = cell.Occupy(occupier);
			if (canOccupy)
			{
				// Cell already occupied, delete from list of free cells
				_freeCells.Remove(cell);
			}

			return canOccupy;
		}

		/// <summary>
		/// Empty cell of all occupiers
		/// </summary>
		/// <param name="gridPosition">Cell position in grid coordinates</param>
		/// <returns>True if cell successfullt released. False otherwise.</returns>
		public bool ReleaseCell(Vector2I gridPosition)
		{
			if (!IsValidCoordinate(gridPosition))
			{
				return false;
			}

			Cell cell = _cells[gridPosition.X, gridPosition.Y];
			cell.Release();
			_freeCells.Add(cell);

			return true;
		}

		/// <summary>
		/// Return random cell from grid
		/// </summary>
		/// <returns>Random free cell</returns>
		public Cell GetRandomFreeCell()
		{
			int randomIndex = GD.RandRange(0, _freeCells.Count - 1);
			return _freeCells[randomIndex];
		}

		/// <summary>
		/// Convert clicked position to grid coordinates and check if coordinates are valid
		/// </summary>
		/// <param name="clickPosition">Click position</param>
		/// <param name="gridCoordinate">Coordinate in grid</param>
		/// <returns>True if click position valid in grid</returns>
		public bool IsCellClicked(Vector2 clickPosition, out Vector2I gridCoordinate)
		{
			Vector2 cellSize = MiniGame.Current.Grid.CellSize;
			Vector2 gridOffset = MiniGame.Current.Grid.Offset;

			// Convert click position to grid coordinates
			int cellX = (int)Math.Floor((clickPosition.X + gridOffset.X) / cellSize.X);
			int cellY = (int)Math.Floor((clickPosition.Y + gridOffset.Y) / cellSize.Y);
			gridCoordinate = new Vector2I(cellX, cellY);

			// Check if coordinates valid, return true if valid
			return MiniGame.Current.Grid.IsValidCoordinate(gridCoordinate);
		}

	}
}
