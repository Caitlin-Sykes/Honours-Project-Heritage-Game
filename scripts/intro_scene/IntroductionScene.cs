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

	private PlayerData PLAYERDATA; //accesses the singleton for the PLAYERDATA


	[Export]
	private Transitions TRANSITION;



	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		//Gets camera and animation nodes
		CAMERAS = GetNode<Cameras>("Cameras");
		CONTROLS = GetNode<Controls>("Cameras/Controls");
		INTROCAM = GetNode<Camera3D>("IntroCam");
		CIRCLES = GetNode<InteractCircles>("InteractableItems");


		//Gets the singletons
		SCENESTATEACCESS = GetNode<SceneState>("/root/SceneStateSingleton"); //scene state
		DIALOGUEACCESS = GetNode<JsonHandler>("/root/DialogueImport"); //dialogue json
		PLAYERDATA = GetNode<PlayerData>("/root/PlayerData"); //dialogue json
		CAMERAS.state = Cameras.State.Enabled; //enables that cameras



		//Sets the initial scene state, the current objective, and the status of the player in a singleton
		SCENESTATEACCESS.CurrentObjective = "Follow the tutorial";
		SCENESTATEACCESS.sceneState = SceneState.CurrentSceneState.Tutorial;
		SCENESTATEACCESS.PlayerStatus = SceneState.StatusOfPlayer.InDialogue;

		//Gets button nodes
		GETUPBUTTON = GetNode<BaseButton>("CanvasLayer/Settings/OnItemSelect");
		INTERACTBOOKBUTTON = GetNode<ButtonOverwrite>("CanvasLayer/Settings/BookInteract");

		//Gets the animation player and intro cam
		ANIMATION_PLAYER_INTROCAM = (AnimationPlayer)INTROCAM.GetNode("AnimationPlayer");
		ANIM_PLAYER = (AnimationPlayer)GetNode("Transition/AnimationPlayer");

		//Signal to start scene
		EmitSignal(SignalName.StartScene); 
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
		DIALOGUE.Dialogue(DIALOGUEACCESS.Speech.IntroductionScene, "Introduction_Scene"); //runs the dialogue
	}

	// A function to handle the second dialogue
	private void SecondDialogue() {
		DIALOGUE.ToggleGUIVisible(); //shows the gui
		DIALOGUE.Dialogue(DIALOGUEACCESS.Speech.Controls, "Controls"); //Starts playing through the controls 
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
		DIALOGUE.SwapOverlay(); //swaps the overlay to freeroam
		CIRCLES.ToggleSpecificDirection(GetNode<ButtonOverwrite>("InteractableItems/Select_Items/Settings/Panel/East/1")); //toggles the individual direction so the function below works 
		CIRCLES.ToggleParentNode(GetNode<ButtonOverwrite>("InteractableItems/Select_Items/Settings/Panel/East/1")); //toggles the parent node so the function below works
		CAMERAS.SetCurrentCamera(GetViewport().GetCamera3D()); //sets current camera


	}

	//Should only be accessed once during the initial fire, but after the misc dialogue
	private async void TriggerDialogueTwo() {

		await ToSignal(DIALOGUE, "LookProgress"); //waits for player input to progress
		SCENESTATEACCESS.PlayerStatus = SceneState.StatusOfPlayer.InDialogue; //Sets the current status to free roam

		DIALOGUE.Dialogue(DIALOGUEACCESS.Speech.Mum_Dialogue_1, "Mum_Dialogue_1"); //Starts playing through the mum dialogue 
		
		
		DIALOGUE.SwapOverlay(); //swaps the overlay  to dialogue

		SCENESTATEACCESS.sceneState = SceneState.CurrentSceneState.Stage_2; //sets the current scene stage to stage_1
	}

	//Triggers the dialogue for the final text of intro scene.
	private void TriggerDialogueThree()
	{
		//Sets the current status to in dialogue
		SCENESTATEACCESS.PlayerStatus = SceneState.StatusOfPlayer.InDialogue; 

		//Starts playing through the mum dialogue 
		DIALOGUE.Dialogue(DIALOGUEACCESS.Speech.Mum_Dialogue_2, "Mum_Dialogue_2"); 

		//Hides the overlay
		DIALOGUE.Select_Items.Visible = false;
		//Hides the circles
		CIRCLES.EmitEvent("ToggleEastEvents");

		//Sets current objective
		SCENESTATEACCESS.CurrentObjective = "Open the book.";
	}


	//Function that handles interacting with the book
	private async void OnBookInteract() {
	SCENESTATEACCESS.PlayerStatus = SceneState.StatusOfPlayer.InDialogue; //sets state to in dialogue
	GetNode<ButtonOverwrite>("CanvasLayer/Settings/BookInteract").Visible = false; //makes it invisible

	//Starts playing through the mum dialogue 
	DIALOGUE.Dialogue(DIALOGUEACCESS.Speech.Mum_Dialogue_3, "Mum_Dialogue_3");
	DIALOGUE.ToggleGUIVisible();
	//Waits 
	await ToSignal(DIALOGUE, "DialogueProgress");
	// Plays dialogue
	DIALOGUE.Dialogue(PLAYERDATA.Player.Name, "Ugh... I feel... weird. Mu-", string.Format("res://resources/textures/sprites/main_char/{0}.svg", PLAYERDATA.Player.Avatar));

	ANIM_PLAYER.PlayBackwards("Wake"); //plays the camera wake up animation reversed (sleep)
	DIALOGUE.ToggleGUIVisible(); //hides the dialogue gui

	await ToSignal(ANIM_PLAYER, "animation_finished"); //waits for the animation to finish
	TRANSITION.NextScene("res://scenes/stonewall/Stonewall.tscn"); //transitions to scene two
	}
}
