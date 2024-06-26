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
	private TextureRect[] avatar; //array of avatars

	private int currentAvatar = 0; //current index of avatar
	private Random rdm; //random number generator
	private PlayerData PLAYERDATA; //accesses the singleton for the PLAYERDATA
	private Transitions TRANSITION; //Handles screen transitions

	private bool currentlyEditing = false;


	public Player_Intro() {
		rdm = new Random();
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		TRANSITION = GetNode<Transitions>("Settings/Transition");
		PLAYERDATA = GetNode<PlayerData>("/root/PlayerData"); //accesses the singleton for the scene state
		InitSettings();


		//Sets random id number
		for (int num = 0; num < 7; num++) {
			id.Text += rdm.Next(0, 9).ToString();
		}
		
	}

	//Handles inputs
	public override void _Input(InputEvent @event) {

		if (Input.IsKeyPressed(Key.D) && !currentlyEditing)
		{
			MoveAvatarRight();
		}

		else if (Input.IsKeyPressed(Key.A) && !currentlyEditing)
		{
			MoveAvatarLeft();
		}
	}

	// Init Settings for Scene
	private void InitSettings()
	{
		//If GetFontDefault returns Dyslexic, sets theme to dyslexie
		//Else sets Theme to cascadia
		if (OptionsVisualsGUI.GetFontDefault() == "Dyslexic")
		{
			Theme = (Theme)GD.Load("res://resources/themes/main_theme_dyslexic.tres");
		}

		else
		{
			Theme = (Theme)GD.Load("res://resources/themes/main_theme_cascadia.tres");
		}

		//Converts GetFontSizeDefault to int (as it returns the default size)
		Theme.DefaultFontSize = OptionsVisualsGUI.GetFontSizeDefault().ToInt();


		if (OptionsVisualsGUI.GetFullScreenDefault())
		{
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
		}
	}


	/**
	* ----------------------------------------------------------------
	* Start of GUI Buttons
	* ----------------------------------------------------------------
	**/

	//On submit button pressed
	private void OnSubmitPressed() {
		if (name.Text.Length > 0) {
			PLAYERDATA.CreatePlayer(name.Text, DateTime.Now, pronouns.Text, id.Text, currentAvatar + 1);
			TRANSITION.NextScene("res://scenes/intro_scene/IntroductionScene.tscn");
		}

		
	}

	/**
	* ----------------------------------------------------------------
	* Currently Editing Handlers
	* ----------------------------------------------------------------
	**/

	private void OnLineEditInputStart() {
		currentlyEditing = true;
	}

	private void OnLineEditInputEnd() { 
		currentlyEditing = false;
	}

	/**
	* ----------------------------------------------------------------
	* Avatar Selection Stuff
	* ----------------------------------------------------------------
	**/

	//Moves avatar selection right
	private void MoveAvatarRight() {
		if (currentAvatar != 19) {
			avatar[currentAvatar].Visible = false;
			avatar[currentAvatar+1].Visible = true;
			currentAvatar++;
		}

		else {
			avatar[currentAvatar].Visible = false;
			avatar[0].Visible = true;
			currentAvatar = 0;
		}
	}

	//Moves avatar selection left
	private void MoveAvatarLeft()
	{
		//If not 0
		if (currentAvatar != 0)
		{
			avatar[currentAvatar].Visible = false;
			avatar[currentAvatar - 1].Visible = true;
			currentAvatar--;
		}

		else
		{
			avatar[currentAvatar].Visible = false;
			avatar[19].Visible = true;
			currentAvatar = 19;
		}
	}

}