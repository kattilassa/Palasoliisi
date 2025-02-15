using Godot;

namespace PalaSoliisi
{
	public abstract partial class Collectable : Sprite2D
	{
		public abstract void Collect(Bear bear);
	}
}