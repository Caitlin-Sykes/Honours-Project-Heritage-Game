using Godot;
public partial class OptionsMenu : Control
{
	private Transitions TRANSITION; // Controls screen transitions


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	
		TRANSITION = GetNode<Transitions>("Transition");
		
		//If GetFontDefault returns Dyslexic, sets theme to dyslexie
		//Else sets Theme to cascadia
		if (OptionsVisualsGUI.GetFontDefault() == "Dyslexic") {
			Theme = (Theme)GD.Load("res://resources/themes/main_theme_dyslexic.tres");
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
	
	
	// A function to control the button "Back" on the Options
	// Sends you back to the main menu
	private void OnBackPressed()
	{ 
		TRANSITION.NextScene("res://scenes/menus/MainMenu.tscn");
	}

	// A function to control the button "Visuals" on the Options
	// Sends you to the visual settings screen
	private void OnVisualPressed()
	{ 
		TRANSITION.NextScene("res://scenes/menus/OptionsVisuals.tscn");
	}

	
	// A function to control the button "Options" on the Options
	// Sends you to the audio settings screen
	private void OnAudioPressed()
	{ 
		TRANSITION.NextScene("res://scenes/menus/OptionsAudio.tscn");
	}

	//A function to control the button "Credits" on the Options
	//Sends you to the credits screen
	private void OnCreditsPressed() {
		TRANSITION.NextScene("res://scenes/menus/OptionsCredits.tscn");
	}
}
