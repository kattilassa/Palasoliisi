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
		//public void CollectArticle()
		//{
		//	Level.Current._article.GlobalPosition = new Vector2(5,5);
		//}
	}
}