using Godot;
using System;

namespace PalaSoliisi
{
	public partial class CardBack1 : CardBack
	{
        public override void Turn()
        {
            GD.Print("Turn card");
            this.Hide();
        }

        public override void Cover()
        {
            this.Show();
        }
	}
}