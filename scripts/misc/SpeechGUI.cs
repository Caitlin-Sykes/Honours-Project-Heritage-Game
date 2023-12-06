using System;
using System.Linq;
using Godot;

public partial class SpeechGUI : Control
{
	public TextureRect AvatarNode; //node to hold avatar image
	public Label NameNode; //node to hold the name of the speaker

	public Label SpeechNode; //node to hold the speech of the speaker

	private Cameras CAMERAS; //node to hold instance of cameras

	[Signal]
	public delegate void DialogueProgressEventHandler(); //handler for progressing scene text

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		CAMERAS = GetNode<Cameras>("../../../Cameras"); //Gets camera and animation nodes
		AvatarNode = GetNode<TextureRect>("Main_Dialogue/Avatar"); //Gets instance of Avatar
		NameNode = GetNode<Label>("Main_Dialogue/Name Container/Name_Box/Name_Label");
		SpeechNode = GetNode<Label>("Main_Dialogue/Speech_Container/Speech"); //Gets instance of Speech
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
		if (Scene != null)
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
		if (Scene != null)
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
	private void ControlsEvents(string id)
	{
		switch (id)
		{
			//If one, makes the top and bottom arrows visible
			case "2":
				CAMERAS.SetSingleArrowVisible(GetViewport().GetCamera3D(), "*Up_Arrow_Parent");
				CAMERAS.SetSingleArrowVisible(GetViewport().GetCamera3D(), "*Down_Arrow_Parent");
				return;

			case "3":
				CAMERAS.SetSingleArrowInvisible(GetViewport().GetCamera3D(), "*Up_Arrow_Parent");
				CAMERAS.SetSingleArrowInvisible(GetViewport().GetCamera3D(), "*Down_Arrow_Parent");
				return;

			default:
				return;
		}
// TODO: add circles to interact with, appended with the current camera enabled. This can be perhaps stored in CAMERAS as opposed to SPeechGUI
//BUG:  id no 5 in controls too much text- downsize to smaller :D
	}
}