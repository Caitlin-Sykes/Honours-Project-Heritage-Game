using Godot;
using System;
using System.IO;
using System.Linq;

public partial class OptionsVisualsGUI : Control
{
	//An enum for replacing the default sizes
	enum ReplacingSize {
		Default,
		Bigger,
		Biggest
	} 

	/**
	*----------------------------------------------------------------
	* Start of Menu Buttons
	*----------------------------------------------------------------
	**/
	
	/**
	* A function to control the button "Back" on the Options
	* Sends you back to the Options menu
	**/
	private void OnBackPressed()
	{ 
		GetTree().ChangeSceneToFile("res://scenes/menus/OptionsMenu.tscn");
	}

	/**
	* A function to control the font toggle
	**/
	private void OnFontsItemSelected(int index) {
		if (index == 0) {
			OptionsFileHandler.ReplaceInFile("DYSLEXIE", "CASCADIA");
		}

		else {
			OptionsFileHandler.ReplaceInFile("CASCADIA", "DYSLEXIE");
		}	
	}

	/**
	* A function to control the font toggle
	**/
	private void OnFontSizeItemSelected(int index) {
		ReplacingSize rs = ReplacingSize.Default;
		
		if (OptionsFileHandler.FindInFile("BIGGER")) {
			rs = ReplacingSize.Bigger;
		}

		else if (OptionsFileHandler.FindInFile("BIGGEST")) {
			rs = ReplacingSize.Biggest;
		}

		switch (index) {
			case 0:
					GD.Print(rs.ToString());
					OptionsFileHandler.ReplaceInFile(rs.ToString().ToUpper(), "DEFAULT");
					break;	
			case 1:
					GD.Print(rs.ToString());
					OptionsFileHandler.ReplaceInFile(rs.ToString().ToUpper(), "BIGGER");
					break;
			case 2:
					GD.Print(rs.ToString());
					OptionsFileHandler.ReplaceInFile(rs.ToString().ToUpper(), "BIGGEST");
					break;
			default: 
					throw new ArgumentException("Something has gone wrong with the config. Please delete it and restart Pride.");
				
		}
	}
}
