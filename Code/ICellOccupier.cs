namespace PalaSoliisi
{
	public interface ICellOccupier
	{
		CellOccupierType Type { get; }
	}

	public enum CellOccupierType
	{
		None,
		Card,
		CardBack
	}
}
