using Godot;
using System;

public partial class DisclaimersMenu : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		InitSettings();
	}

	/**
	* Init Settings for Scene
	**/
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


	/** ----------------------------------------------------------------
	*	Start of Menu Buttons
	*	----------------------------------------------------------------
	**/   

	private void OnContinuePressed() { 
		GetTree().ChangeSceneToFile("res://scenes/intro_scene/IntroductionScene.tscn");
	}
}
