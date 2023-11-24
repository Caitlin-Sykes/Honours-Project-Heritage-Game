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

	[Export]
	private TextureRect[] avatar;
	private Random rdm; //random number generator
	public PlayerDataStruct player;
	private Transitions TRANSITION; //Handles screen transitions



	public Player_Intro() {
		rdm = new Random();
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		TRANSITION = GetNode<Transitions>("Transition");
		InitSettings();


		//Sets random id number
		for (int num = 0; num < 7; num++) {
			id.Text += rdm.Next(0, 9).ToString();
		}
		
	}

	// Init Settings for Scene
	private void InitSettings()
	{
		//If GetFontDefault returns Dyslexie, sets theme to dyslexie
		//Else sets Theme to cascadia
		if (OptionsVisualsGUI.GetFontDefault() == "Dyslexie")
		{
			Theme = (Theme)GD.Load("res://resources/themes/main_theme_dyslexie.tres");
		}

		else
		{
			Theme = (Theme)GD.Load("res://resources/themes/main_theme_cascadia.tres");
		}

		//Converts GetFontSizeDefault to int (as it returns the default size)
		Theme.DefaultFontSize = OptionsVisualsGUI.GetFontSizeDefault().ToInt();


		if (OptionsVisualsGUI.GetFullScreenDefault() == true)
		{
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
		}
		// TODO: optimise this spaghetti junction of code for init
	}

	/**
	* ----------------------------------------------------------------
	* Start of GUI Buttons
	* ----------------------------------------------------------------
	**/

	//On submit button pressed
	//TODO: add proper error handling
	private void OnSubmitPressed() {
		if (name.Text.Length > 0) {
			player = new PlayerDataStruct(name.Text, DateTime.Now, pronouns.Text, id.Text, 1);
		}

		TRANSITION.NextScene("res://scenes/intro_scene/IntroductionScene.tscn");
	}
}