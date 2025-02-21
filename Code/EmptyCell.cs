namespace PalaSoliisi
{
	/// <summary>
	/// Kuvaa tyhjää solua Gridillä.
	/// </summary>
	public class EmptyCell : ICellOccupier
	{
		public CellOccupierType Type => CellOccupierType.None;
	}
}