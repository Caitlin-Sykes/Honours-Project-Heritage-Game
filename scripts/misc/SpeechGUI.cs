using System;
using System.Linq;
using Godot;

public partial class SpeechGUI : Control
{
	[Export]
	private InteractCircles CIRCLES; //instance of circles
	public TextureRect AvatarNode; //node to hold avatar image
	public Label NameNode; //node to hold the name of the speaker

	public Label SpeechNode; //node to hold the speech of the speaker

	private Cameras CAMERAS; //node to hold instance of cameras

	private CanvasLayer Select_Items; //node to hold instance of select items overlay
	private CanvasLayer Speech_Overlay; //node to hold instance of overlay

	public bool DialogueLocked {get; set;} = false; //to lock the dialogue

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
		if (Scene != null && Speech_Overlay.Visible == true && SceneState.PlayerStatus == SceneState.StatusOfPlayer.InDialogue)
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
		if (Scene != null && Speech_Overlay.Visible == true && SceneState.PlayerStatus == SceneState.StatusOfPlayer.InDialogue)
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
	/**
	* ----------------------------------------------------------------
	*	GUI Input Handlers
	* ----------------------------------------------------------------
	**/

	public void OnGUIClick(InputEvent @evnt) {
		if (@evnt is InputEventMouseButton mouse && @evnt.IsPressed() && SceneState.PlayerStatus == SceneState.StatusOfPlayer.InDialogue) {
			EmitSignal("DialogueProgress");
		}

		else if (@evnt is InputEventMouseButton ms && @evnt.IsPressed() && SceneState.PlayerStatus == SceneState.StatusOfPlayer.FreeRoam)
		{
			EmitSignal("SceneProgress");
		}

		else if (@evnt is InputEventMouseButton moose && @evnt.IsPressed() && SceneState.PlayerStatus == SceneState.StatusOfPlayer.LookingAtSomething)
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
			case "6":
				CIRCLES.ToggleSpecificDirection(GetNode<ButtonOverwrite>("../../../InteractableItems/Select_Items/Settings/Panel/East/1")); //disables previous specific direction
				CIRCLES.ToggleSpecificDirection(GetNode<ButtonOverwrite>("../../../InteractableItems/Select_Items/Settings/Panel/East/2")); //enables previous specific direction

				DialogueLocked = true;
				await ToSignal(this, "SceneProgress");
				SwapOverlay();
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