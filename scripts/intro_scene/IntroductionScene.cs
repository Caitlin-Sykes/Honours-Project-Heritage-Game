using Godot;
using System;
using System.Runtime.InteropServices;

public partial class IntroductionScene : Node3D
{

	[Signal]
	public delegate void StartSceneEventHandler();

	[Export]
	private SpeechGUI DIALOGUE; //instance of speechGUI

	private Controls CONTROLS; //instance of controls

	private BaseButton GETUPBUTTON; // a button used solely for getting up from the bed

	private ButtonOverwrite INTERACTBOOKBUTTON; //a button used solely for interacting with the book.

	private Cameras CAMERAS; //instance of cameraScript

	private InteractCircles CIRCLES; //instance of interactCircles

	private Camera3D INTROCAM; //the camera you start viewing from
	private AnimationPlayer ANIMATION_PLAYER_INTROCAM; // the animation handler for intro 
	private AnimationPlayer ANIM_PLAYER; // the animation handler for intro 


	[Signal]
	public delegate void Stage1EventHandler(); //handler for progressing looking at smth
	private SceneState SCENESTATEACCESS; //accesses the singleton for the scenestate
	private JsonHandler DIALOGUEACCESS; //accesses the singleton for the dialogue json

	[Export]
	private Transitions TRANSITION;



	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SCENESTATEACCESS = GetNode<SceneState>("/root/SceneStateSingleton"); //accesses the singleton for the scene state

		DIALOGUEACCESS = GetNode<JsonHandler>("/root/DialogueImport"); //accesses the singleton for the dialogue json

		SCENESTATEACCESS.CurrentObjective = "Follow the tutorial";
		SCENESTATEACCESS.sceneState = SceneState.CurrentSceneState.Tutorial;
		SCENESTATEACCESS.PlayerStatus = SceneState.StatusOfPlayer.InDialogue;

		//Gets button nodes
		GETUPBUTTON = GetNode<BaseButton>("CanvasLayer/Settings/OnItemSelect");
		INTERACTBOOKBUTTON = GetNode<ButtonOverwrite>("CanvasLayer/Settings/BookInteract");


		//Gets camera and animation nodes
		CAMERAS = GetNode<Cameras>("Cameras");
		CONTROLS = GetNode<Controls>("Cameras/Controls");
		INTROCAM = GetNode<Camera3D>("IntroCam");
		CIRCLES = GetNode<InteractCircles>("InteractableItems");
		
		ANIMATION_PLAYER_INTROCAM = (AnimationPlayer)INTROCAM.GetNode("AnimationPlayer");
		ANIM_PLAYER = (AnimationPlayer)GetNode("Transition/AnimationPlayer");



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
	public void Stage1Start() {
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
		GETUPBUTTON.Visible = !GETUPBUTTON.Visible;
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
	private async void TriggerDialogueTwo() {

		await ToSignal(DIALOGUE, "LookProgress");
		SCENESTATEACCESS.PlayerStatus = SceneState.StatusOfPlayer.InDialogue; //Sets the current status to free roam

		DIALOGUE.Dialogue(DIALOGUEACCESS.Speech.Mum_Dialogue_1, "Mum_Dialogue_1", new string[] {"5"}); //Starts playing through the mum dialogue 
		
		
		DIALOGUE.SwapOverlay();

		SCENESTATEACCESS.sceneState = SceneState.CurrentSceneState.Stage_2; //sets the current scene stage to stage_1
	}

	//Triggers the dialogue for the final text of intro scene.
	private async void TriggerDialogueThree()
	{
		//Sets the current status to in dialogue
		SCENESTATEACCESS.PlayerStatus = SceneState.StatusOfPlayer.InDialogue; 

		//Starts playing through the mum dialogue 
		DIALOGUE.Dialogue(DIALOGUEACCESS.Speech.Mum_Dialogue_2, "Mum_Dialogue_2", new string[] { "8" }); 

		//Hides the overlay
		DIALOGUE.Select_Items.Visible = false;
		//Hides the circles
		CIRCLES.EmitEvent("ToggleEastEvents");

		SCENESTATEACCESS.CurrentObjective = "Open the book.";
	}


	private async void OnBookInteract() {
	SCENESTATEACCESS.PlayerStatus = SceneState.StatusOfPlayer.InDialogue;
	GetNode<ButtonOverwrite>("CanvasLayer/Settings/BookInteract").Visible = false;

	//Starts playing through the mum dialogue 
	DIALOGUE.Dialogue(DIALOGUEACCESS.Speech.Mum_Dialogue_3, "Mum_Dialogue_3", new string[] { "-1" });
	DIALOGUE.ToggleGUIVisible();
	//Waits 
	await ToSignal(DIALOGUE, "DialogueProgress");
	DIALOGUE.Dialogue(PlayerData.Player.Name, "Ugh... I feel... weird. Mu-", string.Format("res://resources/textures/sprites/main_char/{0}.svg", PlayerData.Player.Avatar));

	ANIM_PLAYER.PlayBackwards("Wake"); //plays the camera wake up animation reversed (sleep)
	DIALOGUE.ToggleGUIVisible();

	await ToSignal(ANIM_PLAYER, "animation_finished"); //waits for the animation to finish
	TRANSITION.NextScene("res://scenes/stonewall/Stonewall.tscn");
	}
}
