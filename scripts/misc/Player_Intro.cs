using Godot;
using System;
public partial class Player_Intro : Control
{

	[Export]
	private LineEdit id; //for the ID of the player
	[Export]
	private LineEdit name; //for the name of the player
	[Export]
	private OptionButton pronouns; //for the pronouns of the player
	private Random rdm; //random number generator
	public PlayerDataStruct player;


	public Player_Intro() {
		rdm = new Random();
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Sets random id number
		for (int num = 0; num < 7; num++) {
			id.Text += rdm.Next(0, 9).ToString();
		}
		
	}

	//On submit button pressed
	private void OnSubmitPressed() {
		if (name.Text.Length <= 0) {
			
		}
		player = new PlayerDataStruct(name.Text, DateTime.Now, pronouns.Text, id.Text);
	}
}