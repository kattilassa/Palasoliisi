using Godot;
using System;

namespace PalaSoliisi
{
	public partial class CardBack1 : CardBack
	{
        // Hides sprite when clicked
        public override void Turn()
        {
            GD.Print("Turn card");
            this.Hide();
        }

        // If not maching pair sets to visible
        public override void Cover()
        {
            this.Show();
        }
	}
}