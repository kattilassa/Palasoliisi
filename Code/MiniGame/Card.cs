using Godot;

namespace PalaSoliisi
{
	public abstract partial class Card : Sprite2D, ICellOccupier
	{
		// Sets CellOccupierType
		public CellOccupierType Type => CellOccupierType.Card;
		public Grid Grid
		{
			get { return MiniGame.Current.Grid; }
		}

		public Vector2I GridPosition { get; set; }

		/// <summary>
		/// Sets the objects position in grid
		/// </summary>
		/// <param name="gridPosition">Position where object is placed in grid</param>
		/// <returns></returns>
		public bool SetPosition(Vector2I gridPosition)
		{
			if (Grid.GetWorldPosition(gridPosition, out Vector2 worldPosition))
			{
				Position = worldPosition;
				GridPosition = gridPosition;
				// Position is within the grid boundaries
				return true;
			}

			// Position out of bounds for grid
			return false;
		}

		public abstract void Turn();
	}
}