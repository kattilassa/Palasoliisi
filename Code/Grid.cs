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


		// Vector2I on integeriä kullekin koordinaatille yksikkönä käyttävä vektorityyppi.
		[Export] private Vector2I _cellSize = Vector2I.Zero;

		public int Width
		{
			get {return _width;}
		}
		public int Height
		{
			get {return _height;}
		}

		// Tähän 2-uloitteiseen taulukkoon on tallennettu gridin solut. Alussa taulukkoa ei ole, vaan
		// muuttujassa on tyhjä viittaus (null). Taulukko pitää luoda pelin alussa (esim. _Ready-metodissa).
		private Cell[,] _cells = null;
		private List<Cell> _freeCells = new List<Cell>();

		public Vector2 CellSize => new Vector2(_cellSize.X, _cellSize.Y);

		public Vector2 Offset
		{
			get
			{
				// Lasketaan offset samalla tavalla kuin _Ready-metodissa
				Vector2 offset = new Vector2((_width * _cellSize.X) / 2f, (_height * _cellSize.Y) / 2f);
				return offset;
			}
		}

		public override void _Ready()
		{
			_cells = new Cell[Width, Height];

			// Laske se piste, josta taulukon rakentaminen aloitetaan. Koska 1. solu luodaan gridin vasempaan
			// yläkulmaan, on meidän laskettava sitä koordinaattia vastaava piste. Oletetaan Gridin pivot-pisteen
			// olevan kameran keskellä (https://en.wikipedia.org/wiki/Pivot_point).
			Vector2 offset = new Vector2((_width * _cellSize.X) / 2, (_height * _cellSize.Y) / 2);

			Vector2 halfNode = new Vector2(_cellSize.X / 2f, _cellSize.Y / 2f);

			offset.X -= halfNode.X;
			offset.Y -= halfNode.Y;

			// Lataa Cell-scene. Luomme tästä uuden olion kutakin ruutua kohden.
			PackedScene cellScene = ResourceLoader.Load<PackedScene>(_cellScenePath);
			if (cellScene == null)
			{
				GD.PrintErr("Cell sceneä ei löydy! Gridiä ei voi luoda!");
				return;
			}

			// Alustetaan Grid kahdella sisäkkäisellä for-silmukalla.
			for (int x = 0; x < _width; ++x)
			{
				for (int y = 0; y < _height; ++y)
				{
					// Luo uusi olio Cell-scenestä.
					Cell cell = cellScene.Instantiate<Cell>();
					// Lisää juuri luotu Cell-olio gridin Nodepuuhun.
					AddChild(cell);
					// Vector2 worldPosition = new Vector2(x * _cellSize.X, y * _cellSize.Y) - offset;
					// cell.Position = worldPosition;

					cell.Position = new Vector2(x * _cellSize.X, y * _cellSize.Y) - offset;
					cell.GridPosition = new Vector2I(x, y);

					_cells[x, y] = cell;
					_freeCells.Add(cell);
				}
			}
		}

		public bool GetWorldPosition(Vector2I gridPosition, out Vector2 worldPosition)
		{
			if (IsValidCoordinate(gridPosition))
    		{
				worldPosition = _cells[gridPosition.X, gridPosition.Y].Position;

				return true; // Sijainti on laillinen
			}

    		worldPosition = Vector2.Zero; // Jos sijainti ei ole laillinen, palautetaan (0,0)
    		return false;

		}

		public bool IsValidCoordinate(Vector2I gridPosition)
		{
			if (gridPosition.X >= 0 && gridPosition.X < _width &&
        		gridPosition.Y >= 0 && gridPosition.Y < _height)
			{
				return true;
			}
			else {return false;}
		}

		public bool OccupyCell(ICellOccupier occupier, Vector2I gridPosition)
		{
			if (!IsValidCoordinate(gridPosition))
			{
				return false;
			}

			Cell cell = _cells[gridPosition.X, gridPosition.Y];
			bool canOccupy = cell.Occupy(occupier);
			if (canOccupy)
			{
				// Solu on varattu, poista se _freeCells-listalta.
				_freeCells.Remove(cell);
			}

			return canOccupy;
		}

		/// <summary>
		/// Vapauttaa solun.
		/// </summary>
		/// <param name="gridPosition">Solun sijainti gridin koordinaatistossa.</param>
		/// <returns>True, jos vapautus onnistuu. False muuten.</returns>
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
		/// Palauttaa satunnaisen vapaan solun gridiltä.
		/// </summary>
		/// <returns>Satunnainen vapaa solu</returns>
		public Cell GetRandomFreeCell()
		{
			// Koska lista indeksoidaan C#-kielessä nollasta alkaen, pitää listan
			// pituudesta vähentää yksi, jotta saadaan oikeat luvut mukaan randomiin.
			int randomIndex = GD.RandRange(0, _freeCells.Count - 1);
			return _freeCells[randomIndex];
		}

		public bool HasCollectable(Vector2I gridPosition)
		{
			if (!IsValidCoordinate(gridPosition))
			{
				return false;
			}

			Cell cell = _cells[gridPosition.X, gridPosition.Y];
			return cell.Occupier.Type == CellOccupierType.Collectable;
		}

		public Collectable GetCollectable(Vector2I gridPosition)
		{
			if (!IsValidCoordinate(gridPosition))
			{
				return null;
			}

			Cell cell = _cells[gridPosition.X, gridPosition.Y];

			if (cell.Occupier is Collectable)
			{
				return cell.Occupier as Collectable;
			}

			return null;
		}

		public bool IsCellClicked(Vector2 clickPosition, out Vector2I gridCoordinate)
		{
			// Oletetaan, että näitä propertyjä on saatavilla
			Vector2 cellSize = Level.Current.Grid.CellSize;
			Vector2 gridOffset = Level.Current.Grid.Offset;

			// Muunna klikkauspaikka ruudukon koordinaateiksi
			int cellX = (int)Math.Floor((clickPosition.X + gridOffset.X) / cellSize.X);
			int cellY = (int)Math.Floor((clickPosition.Y + gridOffset.Y) / cellSize.Y);
			gridCoordinate = new Vector2I(cellX, cellY);

			// Tarkista, että ruudun koordinaatit ovat kelvolliset
			return Level.Current.Grid.IsValidCoordinate(gridCoordinate);
		}

	}
}
