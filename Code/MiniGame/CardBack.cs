using Godot;

namespace PalaSoliisi
{
	public abstract partial class CardBack : Sprite2D, ICellOccupier
	{
		public CellOccupierType Type => CellOccupierType.CardBack;
		public Grid Grid
		{
			get { return MiniGame.Current.Grid; }
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
	}
}