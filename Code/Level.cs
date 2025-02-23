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

        private Grid _grid = null;
		private PackedScene _bearScene = null;
		private PackedScene _articleScene = null;
		private int _articlePieces = 0;
		private Bear _bear = null;
		private Article _article = null;

		public int ArticlePieces
		{
			get { return _articlePieces; }
			set { _articlePieces = value; }
		}

		public Grid Grid
		{
			get { return _grid; }
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
			_grid = GetNode<Grid>("Grid");
			if (_grid == null)
			{
				GD.PrintErr("Gridiä ei löytynyt Levelin lapsinodeista!");
			}
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
		private void CreateArticles()
		{
			if (_article != null)
			{
				_article.QueueFree();
				_article = null;
			}

			if (_articleScene == null)
			{
				_articleScene = ResourceLoader.Load<PackedScene>(_articleScenePath);
				if (_articleScene == null)
				{
					GD.PrintErr("Articles can't be found");
					return;
				}
			}

			_article = _articleScene.Instantiate<Article>();
			AddChild(_article);

			Cell freeCell = Grid.GetRandomFreeCell();
			if (Grid.OccupyCell(_article, freeCell.GridPosition))
			{
				_article.SetPosition(freeCell.GridPosition);
			}
		}
	}
}