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
	public PlayerDataStruct player;
	private Transitions TRANSITION; //Handles screen transitions

	private bool currentlyEditing = false;


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

	//Handles inputs
	public override void _Input(InputEvent @event) {

		if (Input.IsKeyPressed(Key.D) && currentlyEditing == false)
		{
			MoveAvatarRight();
		}

		else if (Input.IsKeyPressed(Key.A) && currentlyEditing == false)
		{
			MoveAvatarLeft();
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
			PlayerData.CreatePlayer(name.Text, DateTime.Now, pronouns.Text, id.Text, currentAvatar);
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