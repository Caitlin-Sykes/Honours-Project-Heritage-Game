using Godot;
using System;

public partial class OptionsMenu : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	
		//If GetFontDefault returns Dyslexie, sets theme to dyslexie
		//Else sets Theme to cascadia
		if (OptionsVisualsGUI.GetFontDefault() == "Dyslexie") {
			Theme = (Theme)GD.Load("res://resources/themes/main_theme_dyslexie.tres");
		}

		else {
			Theme = (Theme)GD.Load("res://resources/themes/main_theme_cascadia.tres");
		}

	}

	/**
	*----------------------------------------------------------------
	* Start of Menu Buttons
	*----------------------------------------------------------------
	**/
	
	/**
	* A function to control the button "Back" on the Options
	* Sends you back to the main menu
	**/
	private void OnBackPressed()
	{ 
		GetTree().ChangeSceneToFile("res://scenes/menus/MainMenu.tscn");
	}

	/**
	* A function to control the button "Visuals" on the Options
	* Sends you to the visual settings screen
	**/
	private void OnVisualPressed()
	{ 
		GetTree().ChangeSceneToFile("res://scenes/menus/OptionsVisuals.tscn");
	}

	/**
	* A function to control the button "Options" on the Options
	* Sends you to the audio settings screen
	**/
	private void OnAudioPressed()
	{ 
		GetTree().ChangeSceneToFile("res://scenes/menus/OptionsAudio.tscn");
	}
}
