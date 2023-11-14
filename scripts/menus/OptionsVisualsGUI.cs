using Godot;
using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

public partial class OptionsVisualsGUI : Control
{

	private static readonly string FONT_OPTION_BUTTON_PATH = "Menu_Panel/VisualButtons/PanelContainer/Panel/HBoxContainer/MarginContainer/VBoxContainer/Fonts";
	private static readonly string FONT_SIZE_OPTION_BUTTON_PATH = "Menu_Panel/VisualButtons/PanelContainer/Panel/HBoxContainer/MarginContainer/VBoxContainer/Font_Size";

	private static readonly string FULL_SCREEN_OPTION_BUTTON_PATH = "Menu_Panel/VisualButtons/PanelContainer/Panel/HBoxContainer/MarginContainer/VBoxContainer/Full_Screen";

	//An enum for replacing the default sizes
	enum ReplacingSize {
		Default,
		Bigger,
		Biggest
	} 

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
	*Sets default menu options
	*----------------------------------------------------------------
	**/

	/**
	* Selects the default options
	**/
	private void SetFontDefault() {
		//Sets default setting to Dyslexie if CURRENT_FONT is Dyslexie
		//Otherwise sets it to Cascadia
		if (GetFontDefault() == "Dyslexie") {
			GetNode<OptionButton>(FONT_OPTION_BUTTON_PATH).Select(1); //with 1 being the index of dyslexie
		}

		//If it finds biggest in the config file, sets setting to be replaced to biggest
		else {
			GetNode<OptionButton>(FONT_OPTION_BUTTON_PATH).Select(0); //with 0 being the index of cascadia
		}
	}

	//Gets the current font default
	public static string GetFontDefault() {
		//If DYSLEXIE is found in file, current font is set to dyslexie
		if (OptionsFileHandler.FindInFile("DYSLEXIE")) {
			return "Dyslexie";
		}

		return "Cascadia";
	}

	/**
	* Selects the default options
	**/
	public void SetFontSizeDefault() {
		//Sets default setting to Dyslexie if CURRENT_FONT is Dyslexie
		//Otherwise sets it to Cascadia
		if (GetFontSizeDefault() == "16") {
			GetNode<OptionButton>(FONT_SIZE_OPTION_BUTTON_PATH).Select(0); //with 0 being the index of default
		}

		if (GetFontSizeDefault() == "20") {
			GetNode<OptionButton>(FONT_SIZE_OPTION_BUTTON_PATH).Select(1); //with 1 being the index of bigger
		}

		//If it finds biggest in the config file, sets setting to be replaced to biggest
		else {
			GetNode<OptionButton>(FONT_SIZE_OPTION_BUTTON_PATH).Select(2); //with 2 being the index of biggest
		}
	}

	//Gets the current font size default
	public static string GetFontSizeDefault() {
		//If DYSLEXIE is found in file, current font is set to dyslexie
		if (OptionsFileHandler.FindInFile("[FONT_SIZE] : DEFAULT")) {
			return "16";
		}

		else if (OptionsFileHandler.FindInFile("[FONT_SIZE] : BIGGER")) {
			return "20";
		}

		return "24";
	}


	/**
	* Selects the default options
	**/
	public void SetFullScreenDefault() {
		//Sets default setting to False if FULL_SCREEN is false
		//Otherwise sets it to true
		if (GetFullScreenDefault() == false) {
			GetNode<OptionButton>(FULL_SCREEN_OPTION_BUTTON_PATH).Select(1); //with 1 being the index of false
		}

		else {
			GetNode<OptionButton>(FULL_SCREEN_OPTION_BUTTON_PATH).Select(0); //with 0 being the index of true
		}
	}

	//Gets the current full screen default
	public static bool GetFullScreenDefault() {
		//If full screen false is found in file, current state is set to false
		if (OptionsFileHandler.FindInFile("FALSE")) {
			return false;
		}
		return true;
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
			Theme = (Theme)GD.Load("res://resources/themes/main_theme_cascadia.tres");
		}

		else {
			OptionsFileHandler.ReplaceInFile("CASCADIA", "DYSLEXIE");
			Theme = (Theme)GD.Load("res://resources/themes/main_theme_dyslexie.tres");
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

					//Sets new size
					Theme.DefaultFontSize = 16;
	 
					break;	
			case 1:
					OptionsFileHandler.ReplaceInFile(rs.ToString().ToUpper(), "BIGGER");
					
					//Sets new size
					Theme.DefaultFontSize = 20;

					break;
			case 2:
					OptionsFileHandler.ReplaceInFile(rs.ToString().ToUpper(), "BIGGEST");

					//Sets new size
					Theme.DefaultFontSize = 24;

					break;
			default: 
					throw new ArgumentException("Something has gone wrong with the config. Please delete it and restart Pride.");
				
		}
	}

	/**
	* A function to control the full screen
	**/
	private void OnFullScreenSelected(int index) {

		//Depending on what screen is in settings, replaces old config with new config
		switch (index) {
			case 0:
					OptionsFileHandler.ReplaceInFile("FALSE", "TRUE");
					DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
					break;


			case 1:
					OptionsFileHandler.ReplaceInFile("TRUE", "FALSE");
					DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
					break;

			default:
					OptionsFileHandler.ReplaceInFile("TRUE", "FALSE");
					DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
					break;

		}
	}
}
