using Godot;
using System;

namespace PalaSoliisi
{
	public partial class Level : Node2D
	{
		// Static on luokan ominaisuus, ei siitä luotujen ominaisuuksien.
		// Kaikki luokasta luodut olio jakavat saman staattisen muuttujan.
		private static Level _current = null;
		public static Level Current
		{
			get { return _current; }
		}
		[Export] private string _bearScenePath = "res://Character/Bear.tscn";
		[Export] private string _articleScenePath = "res://Levels/Collectables/Article.tscn";
		private PackedScene _bearScene = null;
		private PackedScene _articleScene = null;
		private int _articlePieces = 0;
		private Bear _bear = null;

		public int ArticlePieces
		{
			get { return _articlePieces; }
			set { _articlePieces = value; }
		}
		public Bear Bear
		{
			get { return _bear; }
		}

		// Rakentaja. Käytetään alustamaan olio.
		public Level()
		{
			// _current muuttujaan asetetaan viittaus juuri juotuun Level-olioon.
			// Tälläön Current-propertyn kautta muut oliot pääsevät käsiksi Level-olioon.
			_current = this;
		}
		public override void _Ready()
		{
			ResetGame();
		}
		public void ResetGame()
		{
			// Destroy previous bear if it exists
			if (_bear != null)
			{
				_bear.QueueFree();
				_bear = null;
			}

			// reate bear
			_bear = CreateBear();
			AddChild(_bear);

			//Articles to zero
			ArticlePieces= 0;

			//Create articles
			CreateArticles();
		}
		public override void _Process(double delta)
		{
		}
		private Bear CreateBear()
		{
			if (_bearScene == null)
			{
				_bearScene = ResourceLoader.Load<PackedScene>(_bearScenePath);
				if (_bearScene == null)
				{
					GD.PrintErr("Bear can't be found");
					return null;
				}
			}
			return _bearScene.Instantiate<Bear>();
		}
		private Article CreateArticles()
		{
			if (_articleScene == null)
			{
				_articleScene = ResourceLoader.Load<PackedScene>(_articleScenePath);
				if (_articleScene == null)
				{
					GD.PrintErr("Articles can't be found");
					return null;
				}
			}
			return _articleScene.Instantiate<Article>(); 
		}

		public void CollectArticle()
		{
		}
	}
}