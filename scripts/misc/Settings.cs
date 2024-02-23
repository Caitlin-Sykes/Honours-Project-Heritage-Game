using Godot;
using System;

public partial class Settings : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		InitSettings();
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
}
