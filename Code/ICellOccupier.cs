namespace PalaSoliisi
{
	public interface ICellOccupier
	{
		CellOccupierType Type { get; }
	}

	public enum CellOccupierType
	{
		None,
		Bear,
		Collectable,
		Obstacle
	}
}
