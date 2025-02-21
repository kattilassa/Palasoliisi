using Godot;
using System;

namespace PalaSoliisi
{
	public partial class Article : Collectable
	{
		[Export] private int _articlePieces = 1;
        public override void Collect(Bear bear)
		{
			Level.Current.ArticlePieces += _articlePieces;
		}
	}
}