using Godot;
using System;
using System.IO;
using System.Linq;

public partial class OptionsVisualsGUI : Control
{

	private static readonly string OPTION_BUTTON_PATH = "Menu_Panel/Visual Buttons/PanelContainer/Panel/HBoxContainer/MarginContainer/VBoxContainer/Fonts";
	//An enum for replacing the default sizes
	enum ReplacingSize {
		Default,
		Bigger,
		Biggest
	} 

	/**
	*---------------------------------------------------------------- 
	*Sets default menu options
	*----------------------------------------------------------------
	**/

	/**
	* Selects the default options
	**/
	private void SetFontDefault() {
		
		//If font "Dyslexie" is found in config
		//Sets default setting to Dyslexie
		//Otherwise sets it to Cascadia
		if (OptionsFileHandler.FindInFile("DYSLEXIE")) {
			GetNode<OptionButton>(OPTION_BUTTON_PATH).Select(1); //with 1 being the index of dyslexie
		}

		//If it finds biggest in the config file, sets setting to be replaced to biggest
		else {
			GetNode<OptionButton>(OPTION_BUTTON_PATH).Select(0); //with 0 being the index of cascadia
		}

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
		
		//If it finds Bigger in the config file, sets setting to be replaced to bigger
		if (OptionsFileHandler.FindInFile("BIGGER")) {
			rs = ReplacingSize.Bigger;
		}

		//If it finds biggest in the config file, sets setting to be replaced to biggest
		else if (OptionsFileHandler.FindInFile("BIGGEST")) {
			rs = ReplacingSize.Biggest;
		}

		//Depending on what font size in settings, replaces old config with new config
		switch (index) {
			case 0:
					OptionsFileHandler.ReplaceInFile(rs.ToString().ToUpper(), "DEFAULT");
					break;	
			case 1:
					OptionsFileHandler.ReplaceInFile(rs.ToString().ToUpper(), "BIGGER");
					break;
			case 2:
					OptionsFileHandler.ReplaceInFile(rs.ToString().ToUpper(), "BIGGEST");
					break;
			default: 
					throw new ArgumentException("Something has gone wrong with the config. Please delete it and restart Pride.");
				
		}
	}
}
