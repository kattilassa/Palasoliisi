using Godot;
using System;

namespace PalaSoliisi
{
	public partial class Cell : Sprite2D
	{

		public static EmptyCell Empty = new EmptyCell();

		private Vector2I _gridPosition;

		// Property GridPosition
		// C#-kielessä propertyt ovat jäsenmuuttujan näköisiä tapoja lukea ja kirjoittaa dataa,
		// mutta todellisuudessa Propertyn käyttö suorittaa sen arvon palauttavan tai
		// asettavan metodin. Näissä metodeissa voidaan tehdä mm. arvon validointia, joten
		// propertyt ovat julkisia jäsenmuuttujia turvallisempi tapa lukea ja kirjoittaa dataa,
		// aivan kuten metoditkin.
		// https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/properties
		public Vector2I GridPosition
		{
			get { return _gridPosition; }
			set
			{
				Vector2I newValue = value;
				if (newValue.X < 0)
				{
					newValue.X = 0;
				}

				if (newValue.Y < 0)
				{
					newValue.Y = 0;
				}

				_gridPosition = newValue;
			}
		}

		public ICellOccupier Occupier
		{
			get;
			private set;
		}

		/// <summary>
		/// Palauttaa tiedon siitä, onko solu vapaa vai onko jokin olio siinä.
		/// </summary>
		public bool IsFree
		{
			get { return Occupier == null || Occupier.Type == CellOccupierType.None; }
		}

		public override void _Ready()
		{
			Occupier = Empty;
		}

		public bool Occupy(ICellOccupier occupier)
		{
			if (!IsFree)
			{
				return false;
			}

			Occupier = occupier;
			return true;
		}


		public void Release()
		{
			Occupier = Empty;
		}
	}
}
