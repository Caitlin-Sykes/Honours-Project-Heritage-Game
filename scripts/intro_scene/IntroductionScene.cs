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

	private Camera3D introCam; //the camera you start viewing from
	private AnimationPlayer ANIMATION_PLAYER_INTROCAM; // the animation handler for intro 

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SceneState.sceneState = SceneState.CurrentSceneState.Tutorial;
		SceneState.PlayerStatus = SceneState.StatusOfPlayer.InDialogue;
		
		//Gets button nodes
		getUpButton = GetNode<BaseButton>("CanvasLayer/Settings/OnItemSelect");

		//Gets camera and animation nodes
		CAMERAS = GetNode<Cameras>("Cameras");
		CONTROLS = GetNode<Controls>("Cameras/Controls");
		introCam = GetNode<Camera3D>("IntroCam");
		
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
		DIALOGUE.Dialogue(JsonHandler.Speech.IntroductionScene, "Introduction_Scene", new string[] { "5" });
	}

	// A function to handle the second dialogue
	private void SecondDialogue() {
		
		DIALOGUE.ToggleGUIVisible(); //toggles the gui visible
		DIALOGUE.Dialogue(JsonHandler.Speech.Controls, "Controls", new string[] {"1", "3", "7"}); //Starts playing through the controls dialogue
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

		SecondDialogue();

	}

	//A function to toggle the wake up button
	public void ToggleWakeUpButton() {
		getUpButton.Visible = !getUpButton.Visible;
	}
}
