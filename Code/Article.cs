using Godot;
using System;

namespace PalaSoliisi
{
	public partial class Article : Collectable
	{
		[Export] private int _score = 1;
        public override void Collect()
		{
			Level.Current.ArticlePieces += _score;
			Level.Current.CreateArticles();
		}
	}
}