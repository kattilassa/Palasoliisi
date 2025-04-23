namespace PalaSoliisi
{
	/// <summary>
	/// Empty Cell in grid
	/// </summary>
	public class EmptyCell : ICellOccupier
	{
		public CellOccupierType Type => CellOccupierType.None;
	}
}