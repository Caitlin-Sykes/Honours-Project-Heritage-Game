using Godot;
using System;

public partial class Stonewall : Node
{
	private SceneState SCENESTATEACCESS; //accesses the singleton for the scene state

	[Export]
	private SpeechGUI DIALOGUE; //instance of speechGUI

	private Controls CONTROLS; //instance of controls
	private Cameras CAMERAS; //instance of cameraScript
	private InteractCircles CIRCLES; //instance of interactCircles
	private Camera3D INTROCAM; //the camera you start viewing from
	private JsonHandler DIALOGUEACCESS; //accesses the singleton for the dialogue json

	private PopupPanel MATTACHINE; //accesses the matachine popup panels


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SCENESTATEACCESS = GetNode<SceneState>("/root/SceneStateSingleton"); //accesses the singleton for the scene state
		DIALOGUEACCESS = GetNode<JsonHandler>("/root/DialogueImport"); //accesses the singleton for the dialogue json
		
		//Gets the pop up panels
		MATTACHINE = GetNode<PopupPanel>("CanvasLayer/IntroCam/MattachineBar");


		//Gets camera and animation nodes
		CAMERAS = GetNode<Cameras>("Cameras");
		CONTROLS = GetNode<Controls>("Cameras/Controls");
		INTROCAM = GetNode<Camera3D>("IntroCam");
		CIRCLES = GetNode<InteractCircles>("InteractableItems");

		//Actual Scene Init Stuff
		SCENESTATEACCESS.sceneState = SceneState.CurrentSceneState.Stage_1; //sets to stage 1.
		SCENESTATEACCESS.PlayerStatus = SceneState.StatusOfPlayer.InDialogue; //sets to in dialogue
		SCENESTATEACCESS.CurrentObjective = "Look around The Stonewall Inn";

		//Plays the stonewall entry dialogue scene.
		DIALOGUE.Dialogue(DIALOGUEACCESS.Speech.Stonewall_Dialogue, "Stonewall_Dialogue");
		DIALOGUE.ToggleGUIVisible();
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

}
