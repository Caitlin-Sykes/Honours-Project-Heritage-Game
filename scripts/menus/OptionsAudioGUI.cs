using Godot;
using System;

public partial class OptionsAudioGUI : Control
{
	private Transitions TRANSITION; //Controls screen transitions

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		TRANSITION = GetNode<Transitions>("Transition");

		InitSettings();
	}

	// Init Settings for Scene
	private void InitSettings() {
			//If GetFontDefault returns Dyslexic, sets theme to dyslexie
			//Else sets Theme to cascadia
			if (OptionsVisualsGUI.GetFontDefault() == "Dyslexic") {
				Theme = (Theme)GD.Load("res://resources/themes/main_theme_dyslexic.tres");
			}

			else {
				Theme = (Theme)GD.Load("res://resources/themes/main_theme_cascadia.tres");
			}

			//Converts GetFontSizeDefault to int (as it returns the default size)
			Theme.DefaultFontSize = OptionsVisualsGUI.GetFontSizeDefault().ToInt();

	
			if (OptionsVisualsGUI.GetFullScreenDefault()) {
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
			}
	}


	/**
	* ----------------------------------------------------------------
	* Start of Menu Buttons
	* ----------------------------------------------------------------
	**/
	
	// A function to control the button "Back" on the Options
	// Sends you back to the options main menu
	private void OnBackPressed()
	{ 
		TRANSITION.NextScene("res://scenes/menus/OptionsMenu.tscn");
	}

	
}
