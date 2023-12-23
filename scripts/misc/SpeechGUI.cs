using System;
using System.Linq;
using Godot;
using Godot.Collections;

public partial class SpeechGUI : Control
{
	[Export]
	private InteractCircles CIRCLES; //instance of circles
	private TextureRect AvatarNode; //node to hold avatar image
	private Label NameNode; //node to hold the name of the speaker

	private Label SpeechNode; //node to hold the speech of the speaker

	private Cameras CAMERAS; //node to hold instance of cameras

	private CanvasLayer Select_Items; //node to hold instance of select items overlay
	private CanvasLayer Speech_Overlay; //node to hold instance of overlay
	
	[Signal]
	public delegate void DialogueProgressEventHandler(); //handler for progressing scene text

	[Signal]
	public delegate void SceneProgressEventHandler(); //handler for progressing scene

	[Signal]
	public delegate void LookProgressEventHandler(); //handler for progressing looking at smth

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		CAMERAS = GetNode<Cameras>("../../../Cameras"); //Gets camera and animation nodes
		AvatarNode = GetNode<TextureRect>("Main_Dialogue/Avatar"); //Gets instance of Avatar
		NameNode = GetNode<Label>("Main_Dialogue/Name Container/Name_Box/Name_Label");
		SpeechNode = GetNode<Label>("Main_Dialogue/Speech_Container/Speech"); //Gets instance of Speech
		Select_Items = GetNode<CanvasLayer>("../../../InteractableItems/Select_Items"); //Gets instance of select items
		Speech_Overlay = GetNode<CanvasLayer>("../.."); //Gets instance of overlay


	}

	/**
	* ----------------------------------------------------------------
	*	Misc handlers
	* ----------------------------------------------------------------
	**/

	//A function to toggle visibility
	public void ToggleGUIVisible() {
		this.Visible = !this.Visible;
	}

	// A function to swap the overlay
	public void SwapOverlay() {
		Speech_Overlay.Visible = !Speech_Overlay.Visible;
		Select_Items.Visible = !Select_Items.Visible;
	}

	public void SkipDialogue() {
		EmitSignal("DialogueProgress");
	}


	/**
	* ----------------------------------------------------------------
	*	Speech Handlers
	* ----------------------------------------------------------------
	**/

	//A function to set the name of the speaker
	public void SetNameNode(string Name) {
		NameNode.Text = Name;
	}

	//A function to set the speech
	public void SetSpeechNode(string speech)
	{
		SpeechNode.Text = speech;
	}

	//A function to set the avatar
	public void SetAvatarNode(string path)
	{
		AvatarNode.Texture = (Texture2D) GD.Load(path);
	}

	//Handles the dialogue if you pass in the Scene
	public async void Dialogue(JsonHandler.DialogueStructData[] Scene)
	{
		//Skips the whole thing if the speech overlay isn't visible
		if (Scene != null && Speech_Overlay.Visible && SceneState.PlayerStatus == SceneState.StatusOfPlayer.InDialogue)
		{

			//For every bit of speech in the scene
			foreach (var Speech in Scene)
			{				
				SetNameNode(string.Format(Speech.Speaker, PlayerData.Player.Name));
				SetSpeechNode(string.Format(Speech.Dialogue, PlayerData.Player.Name));
				SetAvatarNode(string.Format(Speech.Avatar, PlayerData.Player.Avatar));
				await ToSignal(this, "DialogueProgress");

				
			}
		}

		else
		{
			throw new System.InvalidOperationException("Error: something has gone wrong with parsing the dialogue.json");
		}
	}

	//Handles the dialogue if you pass in the Scene. This version handles triggering events at certain IDs
	public async void Dialogue(JsonHandler.DialogueStructData[] Scene, string name, string[] triggerID)
	{
		if (Scene != null && Speech_Overlay.Visible && SceneState.PlayerStatus == SceneState.StatusOfPlayer.InDialogue)
		{

			//For every bit of speech in the scene
			foreach (var Speech in Scene)
			{
				//If SpeechID == triggerID && Speech.Name matches the name given
				if (name == "Introduction_Scene" && triggerID.Contains(Speech.Id))
				{
					IntroductionSceneEvents(Speech.Id);
				}

				else if (name == "Controls" && triggerID.Contains(Speech.Id))
				{
					ControlsEvents(Speech.Id);
				}

				SetNameNode(string.Format(Speech.Speaker, PlayerData.Player.Name));
				SetSpeechNode(string.Format(Speech.Dialogue, PlayerData.Player.Name));
				SetAvatarNode(string.Format(Speech.Avatar, PlayerData.Player.Avatar));
				await ToSignal(this, "DialogueProgress");
			}
		}

		else
		{
			throw new InvalidOperationException("Error: something has gone wrong with parsing the dialogue.json");
		}
	}

	//Just displays
	public async void Dialogue(String name, String description, string avatar)
	{	
				SetNameNode(name);
				SetSpeechNode(description);
				SetAvatarNode(avatar);

				if (SceneState.PlayerStatus == SceneState.StatusOfPlayer.FreeRoam) {
					await ToSignal(this, "SceneProgress");
				}

				else if (SceneState.PlayerStatus == SceneState.StatusOfPlayer.InDialogue)
		{
					await ToSignal(this, "DialogueProgress");
				}
	}

	//For source display
	//BUG: doesnt work
	public async void Dialogue(Dictionary<string, string> extraInfo) {

		//Swaps overlay 
		SwapOverlay();

		// Sets the player name and avatar
		SetNameNode(PlayerData.Player.Name);
		SetAvatarNode(string.Format("res://resources/textures/sprites/main_char/{0}.svg", PlayerData.Player.Avatar));

		//if meta is not null
		if (extraInfo is not null) {

			//foreach key in the extraInfo dict, show the dialogue and await progression signal
			foreach (string key in extraInfo.Keys)
			{

				SetSpeechNode(string.Format(extraInfo[key], System.Environment.NewLine));
				await ToSignal(this, "LookProgress");
			}
		}

		//Hides dialogue again
		SwapOverlay();
	}

	/**
	* ----------------------------------------------------------------
	*	GUI Input Handlers
	* ----------------------------------------------------------------
	**/

	public void OnGUIClick(InputEvent @evnt) {
		if (@evnt is InputEventMouseButton && @evnt.IsPressed() && SceneState.PlayerStatus == SceneState.StatusOfPlayer.InDialogue) {
			EmitSignal("DialogueProgress");
		}

		else if (@evnt is InputEventMouseButton && @evnt.IsPressed() && SceneState.PlayerStatus == SceneState.StatusOfPlayer.FreeRoam)
		{
			EmitSignal("SceneProgress");
		}

		else if (@evnt is InputEventMouseButton && @evnt.IsPressed() && SceneState.PlayerStatus == SceneState.StatusOfPlayer.LookingAtSomething)
		{
			EmitSignal("LookProgress");

		}
	}

	/**
	* ----------------------------------------------------------------
	*	Specific Scene Events
	* ----------------------------------------------------------------
	**/

	//Handles all introduction scene events
	private void IntroductionSceneEvents(string id) {
		switch (id) {
			case "5":
					GetNode<IntroductionScene>("../../..").ToggleWakeUpButton();
				return;
		
			default:
				return;
		}
	}

	//Handles all control scene events
	private async void ControlsEvents(string id)
	{
		switch (id)
		{
			//If two, makes the top and bottom arrows visible
			case "1":
				CAMERAS.SetSingleArrowVisible(GetViewport().GetCamera3D(), "*Up_Arrow_Parent");
				CAMERAS.SetSingleArrowVisible(GetViewport().GetCamera3D(), "*Down_Arrow_Parent");
				return;

			//If three, hides the top and bottom arrows, enables the wardrobe icon
			case "3":
				CAMERAS.SetSingleArrowInvisible(GetViewport().GetCamera3D(), "*Up_Arrow_Parent");
				CAMERAS.SetSingleArrowInvisible(GetViewport().GetCamera3D(), "*Down_Arrow_Parent");
				CIRCLES.ToggleSpecificDirection(GetNode<ButtonOverwrite>("../../../InteractableItems/Select_Items/Settings/Panel/East/2"));
				SceneState.PlayerStatus = SceneState.StatusOfPlayer.FreeRoam;
				await ToSignal(this, "SceneProgress");

				SwapOverlay();

				return;

			//If six, enables the selection
			case "7":
				CIRCLES.ToggleSpecificDirection(GetNode<ButtonOverwrite>("../../../InteractableItems/Select_Items/Settings/Panel/East/1")); //enables poster circles
				CIRCLES.ToggleSpecificDirection(GetNode<ButtonOverwrite>("../../../InteractableItems/Select_Items/Settings/Panel/East/2")); //hides wardrobe circle

				SwapOverlay(); //hides overlay
				return;

			default:
				return;
		}
	}
}

//get current camera 
// move to position
// ie poster
// set staring at "object" to be true
// so if k is pressed, can open up specific information
// store camera pos in meta