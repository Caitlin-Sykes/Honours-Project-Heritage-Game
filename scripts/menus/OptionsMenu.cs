using Godot;
using System;

public partial class OptionsMenu : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
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
	* Sends you back to the visual settings screen
	**/
	private void OnVisualPressed()
	{ 
		GetTree().ChangeSceneToFile("res://scenes/menus/OptionsVisuals.tscn");
	}
}
