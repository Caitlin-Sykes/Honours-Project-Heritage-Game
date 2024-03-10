using Godot;
using System;

public partial class Stonewall : Node
{
	private SceneState SCENESTATEACCESS; //accesses the singleton for the scene state

	[Export]
	private SpeechGUI DIALOGUE; //instance of speechGUI

	private Controls CONTROLS; //instance of controls
	private Cameras CAMERAS; //instance of cameraScript
	private Cameras CAMERASBAR; //instance of bar side camera Script

	private InteractCircles CIRCLES; //instance of interactCircles
	private Camera3D INTROCAM; //the camera you start viewing from
	private JsonHandler DIALOGUEACCESS; //accesses the singleton for the dialogue json

	private PopupPanel MATTACHINE; //accesses the matachine popup panels
	private PopupPanel GLFPOSTER; //accesses the GLF popup panels



	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SCENESTATEACCESS = GetNode<SceneState>("/root/SceneStateSingleton"); //accesses the singleton for the scene state
		DIALOGUEACCESS = GetNode<JsonHandler>("/root/DialogueImport"); //accesses the singleton for the dialogue json
		
		//Gets the pop up panels
		MATTACHINE = GetNode<PopupPanel>("CanvasLayer/IntroCam/MattachineBar");
		GLFPOSTER = GetNode<PopupPanel>("CanvasLayer/IntroCam/GLFPoster");


		//Gets camera and animation nodes
		CAMERAS = GetNode<Cameras>("Cameras");

		//Gets camera and animation nodes
		CAMERASBAR = GetNode<Cameras>("CamerasTwo");

		CONTROLS = GetNode<Controls>("Cameras/Controls");
		INTROCAM = GetNode<Camera3D>("CanvasLayer/IntroCam/");
		CIRCLES = GetNode<InteractCircles>("InteractableItems");

		//Actual Scene Init Stuff
		SCENESTATEACCESS.sceneState = SceneState.CurrentSceneState.Stage_1; //sets to stage 1.
		SCENESTATEACCESS.PlayerStatus = SceneState.StatusOfPlayer.InDialogue; //sets to in dialogue
		SCENESTATEACCESS.CurrentObjective = "Look around The Stonewall Inn";

		//Plays the stonewall entry dialogue scene.
		DIALOGUE.Dialogue(DIALOGUEACCESS.Speech.Stonewall_Dialogue, "Stonewall_Dialogue");
		DIALOGUE.ToggleGUIVisible();

		//Sets Camera Normal to enabled
		CAMERAS.state = Cameras.State.Enabled;

	}

	/** 
	* ------------------------------------------------------------------
	* Handlers for showing extra information
	* ------------------------------------------------------------------
	**/

	//Toggle Mattachine Panel
	private async void ToggleMattachine() {
		await ToSignal(DIALOGUE, "LookProgress");
		MATTACHINE.Visible = !MATTACHINE.Visible;
	}

	//Toggle Poster Panel
	private async void ToggleGLFPoster()
	{
		await ToSignal(DIALOGUE, "LookProgress");
		GLFPOSTER.Visible = !GLFPOSTER.Visible;
	}

	/** 
	* ------------------------------------------------------------------
	* Handlers for moving around Stonewall
	* ------------------------------------------------------------------
	**/

	// Handles swapping camera from IntroCam to West
	private void MoveIntoStonewall() {
		// Gets the camera
		Camera3D cam = GetNode<Camera3D>("Cameras/Controls/UI/West");

		// Disables the four circles
		for (int i = 0; i < 4; i++)
		{
			string path = string.Format("CanvasLayer/Settings/Pre{0}", i);
			CIRCLES.ToggleSpecificDirection(GetNode<ButtonOverwrite>(path));
		}

		cam.Current = true;

		DIALOGUE.ToggleGUIVisible();
		CAMERAS.ToggleEvents();

		SCENESTATEACCESS.CurrentObjective = "Explore the inside of Stonewall";
		SCENESTATEACCESS.PlayerStatus = SceneState.StatusOfPlayer.InDialogue;
		 
		DIALOGUE.Dialogue(DIALOGUEACCESS.Speech.Stonewall_Dialogue_Inside, "Inside_Stonewall");
	}

	//Handles moving from main room to the side with the safe
	private void MoveToSafeSide() {

		//go to south camera on the other side, and sets it to true
		Camera3D cam = CAMERASBAR.CAMERAS[2];

		// Swaps the active cameras between cluster 1 and 2
		// Sets current cam to cameras bar
		SwapActiveCameraPod();

		// Toggles the secondary item overlay
		DIALOGUE.EnableSecondaryItemOverlay();
		DIALOGUE.Active_Canvas_Layer.Visible = true;

		// Enables the events
		CAMERASBAR.EnableEvents = true;
		CAMERASBAR.SetCurrentCamera(cam);

		// Sets status back to free roam
		SCENESTATEACCESS.PlayerStatus = SceneState.StatusOfPlayer.FreeRoam;

		//Locks input for cluster 1. enables input for cluster two
		CAMERAS.state = Cameras.State.Disabled;
		CAMERASBAR.state = Cameras.State.Enabled;

	}

	//Handles moving from main room to the side with the safe
	private void MoveToNormalSide()
	{
		GD.Print("Going to the normal side");
		
		//go to south camera on the other side, and sets it to true
		Camera3D cam = CAMERAS.CAMERAS[2];
		cam.Current = true;

		// Swaps the active cameras between cluster 1 and 2
		// Sets current cam to cameras bar
		SwapActiveCameraPod();
		CAMERASBAR.SetCurrentCamera(cam);

		// Sets status back to free roam
		SCENESTATEACCESS.PlayerStatus = SceneState.StatusOfPlayer.FreeRoam;

		// Toggles the secondary item overlay
		DIALOGUE.DisableSecondaryItemOverlay();
		DIALOGUE.Active_Canvas_Layer.Visible = true;

		//Locks input for cluster 2. enables input for cluster 1
		CAMERAS.state = Cameras.State.Enabled;
		CAMERASBAR.state = Cameras.State.Disabled;

	}

	// A function to swap which camera pod is active, by toggling cameras state.
	private void SwapActiveCameraPod() {
		switch(CAMERAS.state) {
			case Cameras.State.Enabled:
				CAMERAS.state = Cameras.State.Disabled; //disables one side
				CAMERASBAR.state = Cameras.State.Enabled; //enables the other
				break;
			case Cameras.State.Disabled:
				CAMERAS.state = Cameras.State.Enabled; //enables one side
				CAMERASBAR.state = Cameras.State.Disabled; //disables the other
				break;

		}

	}

}
