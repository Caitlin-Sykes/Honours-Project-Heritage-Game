using Godot;
using System;
using System.IO;

public partial class MainMenu : Control
{

	[Export]
    public TextureRect intersexInclusive { get; set; } //IntersexInclusive logo
	
	[Export]
    public TextureRect philly { get; set; } //Philly2017 logo

	[Export]
    public TextureRect originalPride { get; set; } //originalPride logo

	[Export]
    public TextureRect prideProgress { get; set; } //prideProgress logo

	[Export]
    public TextureRect prideSeventyNine { get; set; } //pride1979 logo

	private Transitions TRANSITION; //Handles screen transitions

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
			TRANSITION = GetNode<Transitions>("Transition");

		DisplayServer.WindowSetMode(DisplayServer.WindowMode.Maximized);

		//Logo 
		DecideMainMenuLogo();

			//Inits config file
			OptionsFileHandler.InitConfigFile();

			InitSettings();		
	
			JsonHandler.PreLoadDialogue();
	}

	// Init scene settings
	private void InitSettings() {
		//If GetFontDefault returns Dyslexie, sets theme to dyslexie
			//Else sets Theme to cascadia
			if (OptionsVisualsGUI.GetFontDefault() == "Dyslexie") {
				Theme = (Theme)GD.Load("res://resources/themes/main_theme_dyslexie.tres");
			}

			else {
				Theme = (Theme)GD.Load("res://resources/themes/main_theme_cascadia.tres");
			}

			//Converts GetFontSizeDefault to int (as it returns the default size)
			Theme.DefaultFontSize = OptionsVisualsGUI.GetFontSizeDefault().ToInt();

	
			if (OptionsVisualsGUI.GetFullScreenDefault() == true) {
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
			}
			// TODO: optimise this spaghetti junction of code for init
	}

	/**
	* ----------------------------------------------------------------
	* Main Menu Logo
	* ----------------------------------------------------------------
	**/

	// Decides the logo for the main menu
	private void DecideMainMenuLogo() {
		Random rdm = new Random();
		int logo = rdm.Next(5); //0,1,2,3,4

		switch(logo) {
			case 0:
				intersexInclusive.Visible = true;
				break;

			case 1:
				philly.Visible = true;
				break;

			case 2:
				originalPride.Visible = true;
				break;

			case 3:
				prideProgress.Visible = true;
				break;

			case 4:
				prideSeventyNine.Visible = true;
				break;

			
			default:
				originalPride.Visible = true;
				break;
		
		}
	}

	/** 
	* ----------------------------------------------------------------
	*	Start of Menu Buttons
	* ----------------------------------------------------------------
	**/   


	
	// A function to control the button "Start" on the main menu
	private void OnStartPressed()
	{ 
		TRANSITION.NextScene("res://scenes/menus/Disclaimers.tscn");

	}

	// A function to control the button "Options" on the main menu
	private void OnOptionsPressed()
	{
		TRANSITION.NextScene("res://scenes/menus/OptionsMenu.tscn");
	}

	
	// A function to control the button "Sources" on the main menu
	private void OnSourcesPressed()
	{
		// TRANSITION.NextScene
	}

	// A function to control the button "Exit" on the main menu
	private void OnExitPressed()
	{
		GetTree().Quit();
	}
}
