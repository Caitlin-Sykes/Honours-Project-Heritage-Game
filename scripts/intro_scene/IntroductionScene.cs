using Godot;
using System;

public partial class IntroductionScene : Node3D
{

	[Signal]
	public delegate void StartSceneEventHandler();

	[Export]
	private SpeechGUI DIALOGUE; //instance of speechGUI

	private Controls CONTROLS; //instance of controls

	private BaseButton getUpButton; // a button used solely for getting up from the bed

	private Cameras CAMERAS; //instance of cameraScript

	private InteractCircles CIRCLES;

	private Camera3D introCam; //the camera you start viewing from
	private AnimationPlayer ANIMATION_PLAYER_INTROCAM; // the animation handler for intro 

	[Signal]
	public delegate void Stage1EventHandler(); //handler for progressing looking at smth
	private SceneState SCENESTATEACCESS; //accesses the singleton for the scenestate
	private JsonHandler DIALOGUEACCESS; //accesses the singleton for the dialogue json



	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SCENESTATEACCESS = GetNode<SceneState>("/root/SceneStateSingleton"); //accesses the singleton for the scene state

		DIALOGUEACCESS = GetNode<JsonHandler>("/root/DialogueImport"); //accesses the singleton for the dialogue json

		SCENESTATEACCESS.CurrentObjective = "Follow the tutorial";
		SCENESTATEACCESS.sceneState = SceneState.CurrentSceneState.Tutorial;
		SCENESTATEACCESS.PlayerStatus = SceneState.StatusOfPlayer.InDialogue;

		//Gets button nodes
		getUpButton = GetNode<BaseButton>("CanvasLayer/Settings/OnItemSelect");

		//Gets camera and animation nodes
		CAMERAS = GetNode<Cameras>("Cameras");
		CONTROLS = GetNode<Controls>("Cameras/Controls");
		introCam = GetNode<Camera3D>("IntroCam");
		CIRCLES = GetNode<InteractCircles>("InteractableItems");
		
		ANIMATION_PLAYER_INTROCAM = (AnimationPlayer)introCam.GetNode("AnimationPlayer");
		
		
		EmitSignal(SignalName.StartScene); //Signal to start scene
	}

	/**
	* ----------------------------------------------------------------
	* Dialogue Handlers
	* ----------------------------------------------------------------
	**/

	//A function to handle the first dialogue
	private async void FirstDialogue() {

		await ToSignal(GetTree().CreateTimer(3), SceneTreeTimer.SignalName.Timeout); //timer so it waits out the animations
		DIALOGUE.ToggleGUIVisible(); //toggles the gui visible
		DIALOGUE.Dialogue(DIALOGUEACCESS.Speech.IntroductionScene, "Introduction_Scene", new string[] { "5" });
	}

	// A function to handle the second dialogue
	private void SecondDialogue() {
		DIALOGUE.ToggleGUIVisible(); //shows the gui
		DIALOGUE.Dialogue(DIALOGUEACCESS.Speech.Controls, "Controls", new string[] {"1", "3", "7", "8"}); //Starts playing through the controls 
	}

	// Start Stage 1 Handler
	public async void Stage1Start() {
		EmitSignal(SignalName.Stage1);
	}


	/**
	* ----------------------------------------------------------------
	* GUI Functions
	* ------------------------------------------------------------------
	**/

	// The 2D selecting icon, different icon for 3D scenes
	// in this scene, handles getting up from the bed
	private async void OnItemSelect()
	{
		DIALOGUE.ToggleGUIVisible(); //hides the gui
		ToggleWakeUpButton(); //hides the button
		ANIMATION_PLAYER_INTROCAM.Play("CameraWakeUp"); //plays the camera wake up animation
		await ToSignal(ANIMATION_PLAYER_INTROCAM, "animation_finished"); //waits for the animation to finish
		CAMERAS.SetCurrentCamera(GetNode<Camera3D>("Cameras/Controls/UI/East")); //sets current camera to east

		//Starts the controls dialogue
		SecondDialogue();

		
	}

	//A function to toggle the wake up button
	public void ToggleWakeUpButton() {
		getUpButton.Visible = !getUpButton.Visible;
	}

	/** 
	* ----------------------------------------------------------------
	* Stage 1 Handlers
	* ----------------------------------------------------------------
	**/

	//Starts the stage
	private void Stage1Stuff() {
		DIALOGUE.ToggleGUIVisible(); //hides the gui
		SCENESTATEACCESS.sceneState = SceneState.CurrentSceneState.Stage_1; //sets the current scene stage to stage_1
		SCENESTATEACCESS.PlayerStatus = SceneState.StatusOfPlayer.FreeRoam; //Sets the current status to free roam
		SCENESTATEACCESS.CurrentObjective = "Talk to Mum"; //sets the current objective

		CAMERAS.ToggleEvents(); //enables all events
		DIALOGUE.SwapOverlay(); //swaps the overlay to freer
		CIRCLES.ToggleSpecificDirection(GetNode<ButtonOverwrite>("InteractableItems/Select_Items/Settings/Panel/East/1")); //toggles the individual direction so the function below works 
		CIRCLES.ToggleParentNode(GetNode<ButtonOverwrite>("InteractableItems/Select_Items/Settings/Panel/East/1")); //toggles the parent node so the function below works
		CAMERAS.SetCurrentCamera(GetViewport().GetCamera3D()); //sets current camera


	}

	//Should only be accessed once during the initial fire, but after the misc dialogue
	//show puzzle if boolean is true? go by finding the path and using that to find the puzzle equiv?
	//TODO: was working on this
	private async void TriggerDialogueTwo() {

		await ToSignal(DIALOGUE, "LookProgress");
		SCENESTATEACCESS.PlayerStatus = SceneState.StatusOfPlayer.InDialogue; //Sets the current status to free roam

		DIALOGUE.Dialogue(DIALOGUEACCESS.Speech.Mum_Dialogue_1, "Mum_Dialogue_1", new string[] {"5"}); //Starts playing through the mum dialogue 
		
		
		DIALOGUE.SwapOverlay();


	}
}

