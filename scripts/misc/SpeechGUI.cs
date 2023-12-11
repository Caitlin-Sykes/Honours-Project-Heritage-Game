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

	[Signal]
	public delegate void DialogueProgressEventHandler(); //handler for progressing scene text

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
		if (Scene != null && Speech_Overlay.Visible == true)
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
		if (Scene != null && Speech_Overlay.Visible == true)
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
			throw new System.InvalidOperationException("Error: something has gone wrong with parsing the dialogue.json");
		}
	}

	//Just displays
	public async void Dialogue(String name, String description, string avatar)
	{
		
				SetNameNode(name);
				SetSpeechNode(description);
				SetAvatarNode(avatar);
				await ToSignal(this, "DialogueProgress");
	}
	/**
	* ----------------------------------------------------------------
	*	GUI Input Handlers
	* ----------------------------------------------------------------
	**/

	public void OnGUIClick(InputEvent @evnt) {
		if (@evnt is InputEventMouseButton mouse && @evnt.IsPressed()) {
			EmitSignal("DialogueProgress");
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
				CIRCLES.ToggleSpecificDirection(GetNode<ButtonOverwrite>("../../../InteractableItems/Select_Items/Panel/East/1"));
				await ToSignal(this, "DialogueProgress");
				SwapOverlay();

				return;

			//If five, downsizes the text
			case "5":
				// previousFontSize = GetThemeDefaultFontSize();
				return;

			default:
				return;
		}
// TODO: add circles to interact with, appended with the current camera enabled. This can be perhaps stored in CAMERAS as opposed to SPeechGUI
//BUG:  id no 5 in controls too much text- downsize to smaller :D
	}
}