using Godot;
using System;

public partial class OptionsAudioGUI : Control
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
	* Sends you back to the options main menu
	**/
	private void OnBackPressed()
	{ 
		GetTree().ChangeSceneToFile("res://scenes/menus/OptionsMenu.tscn");
	}

	
}
