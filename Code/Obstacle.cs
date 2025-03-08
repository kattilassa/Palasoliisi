using Godot;
using System;

namespace PalaSoliisi
{
	public partial class Obstacle : Collectable
	{
		  public CellOccupierType Type
		{
			get { return CellOccupierType.Obstacle; }
		}
		 public override void Collect()
		{
		}
	}
}