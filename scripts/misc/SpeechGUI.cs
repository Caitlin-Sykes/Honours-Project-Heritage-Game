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

	public CanvasLayer Select_Items; //node to hold instance of select items overlay
	public CanvasLayer Speech_Overlay; //node to hold instance of overlay

	[Signal]
	public delegate void DialogueProgressEventHandler(); //handler for progressing scene text

	[Signal]
	public delegate void SceneProgressEventHandler(); //handler for progressing scene

	[Signal]
	public delegate void LookProgressEventHandler(); //handler for progressing looking at smth

	private SceneState SCENESTATEACCESS; //accesses the singleton for the scenestate

	private EventDict EVENTDICT; //accesses the singleton for the event dict


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SCENESTATEACCESS = GetNode<SceneState>("/root/SceneStateSingleton"); //accesses the singleton for the scene state
		EVENTDICT = GetNode<EventDict>("/root/EventDict"); //accesses the singleton for the scene state

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
	public void ToggleGUIVisible()
	{
		this.Visible = !this.Visible;
		GD.Print("GUI is: " + this.Visible);
	}

	// A function to swap the overlay
	public void SwapOverlay()
	{
		Speech_Overlay.Visible = !Speech_Overlay.Visible;
		Select_Items.Visible = !Select_Items.Visible;

		GD.Print("speech overlay: " + Speech_Overlay.Visible);
		GD.Print("select_items: " + Select_Items.Visible);

	}

	public void SkipDialogue()
	{
		EmitSignal("DialogueProgress");
	}


	/**
	* ----------------------------------------------------------------
	*	Speech Handlers
	* ----------------------------------------------------------------
	**/

	//A function to set the name of the speaker
	public void SetNameNode(string Name)
	{
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
		AvatarNode.Texture = (Texture2D)GD.Load(path);
	}

	// Sets the nodes
	private void SetConversation(string name, string speech, string path)
	{
		SetNameNode(name);
		SetSpeechNode(speech);
		SetAvatarNode(path);
	}

	//Handles the dialogue if you pass in the Scene
	public async void Dialogue(JsonHandler.DialogueStructData[] Scene)
	{
		//Skips the whole thing if the speech overlay isn't visible
		if (Scene != null && Speech_Overlay.Visible && SCENESTATEACCESS.PlayerStatus == SceneState.StatusOfPlayer.InDialogue)
		{

			//For every bit of speech in the scene
			foreach (var Speech in Scene)
			{
				SetConversation(string.Format(Speech.Speaker, PlayerData.Player.Name), string.Format(Speech.Dialogue, PlayerData.Player.Name), string.Format(Speech.Avatar, PlayerData.Player.Avatar));
				await ToSignal(this, "DialogueProgress");


			}
		}

		else
		{
			throw new System.InvalidOperationException("Error: something has gone wrong with parsing the dialogue.json");
		}
	}

	//Handles the dialogue if you pass in the Scene. This version handles triggering events at certain IDs
	public async void Dialogue(JsonHandler.DialogueStructData[] Scene, string name)
	{
		if (Scene != null && SCENESTATEACCESS.PlayerStatus == SceneState.StatusOfPlayer.InDialogue)
		{
			ValidOverlay();

			//For every bit of speech in the scene
			foreach (var Speech in Scene)
			{
				if (EVENTDICT.CheckIfSceneTrigger(name, Speech.Id))
				{
					switch (name)
					{
						case "Introduction_Scene":
							IntroductionSceneEvents(Speech.Id);
							break;
						case "Controls":
							ControlsEvents(Speech.Id);
							break;
						case "Mum_Dialogue_1":
							MumDialogueEvent(Speech.Id);
							break;
						case "Mum_Dialogue_2":
							MTwoDialogueEvent(Speech.Id);
							break;
						case "Stonewall_Dialogue":
							StonewallEvent(Speech.Id);
							break;
						default:
							break;
					}
				}

				//Sets the name and speech
				SetNameNode(string.Format(Speech.Speaker, PlayerData.Player.Name));
				SetSpeechNode(string.Format(Speech.Dialogue, PlayerData.Player.Name));
				if (Speech.Avatar != "NULL")
				{
					SetAvatarNode(string.Format(Speech.Avatar, PlayerData.Player.Avatar));
				}
				await ToSignal(this, "DialogueProgress");

			}
		}
	}

	//Just displays
	public async void Dialogue(String name, String description, string avatar)
	{
		SetConversation(name, description, avatar);

		if (SCENESTATEACCESS.PlayerStatus == SceneState.StatusOfPlayer.FreeRoam)
		{
			await ToSignal(this, "SceneProgress");
		}

		else if (SCENESTATEACCESS.PlayerStatus == SceneState.StatusOfPlayer.InDialogue)
		{
			await ToSignal(this, "DialogueProgress");
		}
	}

	//For source display
	public async void Dialogue(Dictionary<string, string> extraInfo)
	{

		//Swaps overlay 
		SwapOverlay();

		// Sets the player name and avatar
		SetNameNode(PlayerData.Player.Name);
		SetAvatarNode(string.Format("res://resources/textures/sprites/main_char/{0}.svg", PlayerData.Player.Avatar));

		//if meta is not null
		if (extraInfo is not null)
		{

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

	private void ValidOverlay()
	{
		if (!Speech_Overlay.Visible)
		{
			Speech_Overlay.Visible = true;
			Select_Items.Visible = false;
		}
	}
	/**
	* ----------------------------------------------------------------
	*	GUI Input Handlers
	* ----------------------------------------------------------------
	**/

	//Does different things depending on the gui and clicking it
	public void OnGUIClick(InputEvent @evnt)
	{

		if (@evnt is InputEventMouseButton && @evnt.IsPressed() && SCENESTATEACCESS.PlayerStatus == SceneState.StatusOfPlayer.InDialogue)
		{
			EmitSignal("DialogueProgress");
			GD.Print("DialogueProgress");
		}

		else if (@evnt is InputEventMouseButton && @evnt.IsPressed() && SCENESTATEACCESS.PlayerStatus == SceneState.StatusOfPlayer.FreeRoam)
		{
			EmitSignal("SceneProgress");
			GD.Print("SceneProgress");

		}

		else if (@evnt is InputEventMouseButton && @evnt.IsPressed() && SCENESTATEACCESS.PlayerStatus == SceneState.StatusOfPlayer.LookingAtSomething)
		{
			EmitSignal("LookProgress");
			GD.Print("LookProgress");

		}
	}

	//Shows the current objective of the player
	public void ShowObjective()
	{
		SetNameNode("Guide");
		SetAvatarNode("res://resources/textures/sprites/guide/1.svg");
		SetSpeechNode(SCENESTATEACCESS.CurrentObjective);
	}

	/**
	* ----------------------------------------------------------------
	*	Specific Scene Events
	* ----------------------------------------------------------------
	**/

	//Handles all introduction scene events
	private void IntroductionSceneEvents(string id)
	{
		switch (id)
		{
			case "7":
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
				SCENESTATEACCESS.PlayerStatus = SceneState.StatusOfPlayer.FreeRoam;

				await ToSignal(this, "SceneProgress");

				SwapOverlay();

				return;

			//If six, enables the selection
			case "7":
				CIRCLES.ToggleSpecificDirection(GetNode<ButtonOverwrite>("../../../InteractableItems/Select_Items/Settings/Panel/East/1")); //enables poster circles
				CIRCLES.ToggleSpecificDirection(GetNode<ButtonOverwrite>("../../../InteractableItems/Select_Items/Settings/Panel/East/2")); //hides wardrobe circle

				SwapOverlay(); //hides overlay
				return;

			//If eight (the last one), emits stage signal
			case "8":
				await ToSignal(this, "DialogueProgress");
				GetNode<IntroductionScene>("../../..").Stage1Start();
				return;

			default:
				return;
		}
	}

	//Controls the events for the mum dialogue
	private async void MumDialogueEvent(string id)
	{
		switch (id)
		{
			case "5":
				await ToSignal(this, "DialogueProgress");
				ToggleGUIVisible(); //hides the gui
				SwapOverlay();
				SCENESTATEACCESS.PlayerStatus = SceneState.StatusOfPlayer.FreeRoam; //swaps back to freeroam view
				GetNode<ButtonOverwrite>("../../../InteractableItems/Select_Items/Settings/Panel/South/2").SetMeta("PuzzleEnabled", true); //enables the puzzle for the middle bookshelf
				return;

			default:
				return;
		}
	}


	//Controls the events for the second mum dialogue
	private async void MTwoDialogueEvent(string id)
	{
		switch (id)
		{
			case "8":
				ToggleGUIVisible(); //hides the gui
				SCENESTATEACCESS.PlayerStatus = SceneState.StatusOfPlayer.FreeRoam; //swaps back to freeroam view
				GetNode<ButtonOverwrite>("../BookInteract").Visible = true;
				return;

			default:
				return;
		}
	}

	//Controls the events for the stonewall init scene
	private void StonewallEvent(string id)
	{

		switch (id)
		{
			case "11":
				SCENESTATEACCESS.PlayerStatus = SceneState.StatusOfPlayer.FreeRoam; //swaps back to freeroam view
				ToggleGUIVisible();

				// Enables the three circles
				for (int i = 0; i < 3; i++)
				{
					string path = string.Format("../Pre{0}", i);
					CIRCLES.ToggleSpecificDirection(GetNode<ButtonOverwrite>(path));
				}


			
				return;

			default:
				return;
		}

	}
}

