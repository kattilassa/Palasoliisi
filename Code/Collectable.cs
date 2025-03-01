using Godot;

namespace PalaSoliisi
{
	public abstract partial class Collectable : Sprite2D, ICellOccupier
	{
		public CellOccupierType Type => CellOccupierType.Collectable;
		public Grid Grid
		{
			get { return Level.Current.Grid; }
		}

		public Vector2I GridPosition { get; set; }

		public bool SetPosition(Vector2I gridPosition)
		{
			if (Grid.GetWorldPosition(gridPosition, out Vector2 worldPosition))
			{
				Position = worldPosition;
				GridPosition = gridPosition;
				return true;
			}

			return false;
		}
		public abstract void Collect();
	}

}