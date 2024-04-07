using System;
using System.Linq;
using Godot;
using Godot.Collections;

public partial class SpeechGUI : Control
{
	[Export] 
	private Node PARENT; //root node
	[Export]
	private InteractCircles CIRCLES; //instance of circles
	private TextureRect AvatarNode; //node to hold avatar image
	private Label NameNode; //node to hold the name of the speaker

	private Label SpeechNode; //node to hold the speech of the speaker

	private Cameras CAMERAS; //node to hold instance of cameras

	public CanvasLayer Select_Items; //node to hold instance of select items overlay
	
	[Export]
	public CanvasLayer Select_Items_Second; //node to hold instance of select items overlay for secondary nodes
	public CanvasLayer Speech_Overlay; //node to hold instance of overlay

	public CanvasLayer Active_Canvas_Layer;
	
	private JsonHandler DIALOGUEACCESS; //accesses the singleton for the dialogue json


	[Signal]
	public delegate void DialogueProgressEventHandler(); //handler for progressing scene text

	[Signal]
	public delegate void SceneProgressEventHandler(); //handler for progressing scene

	[Signal]
	public delegate void LookProgressEventHandler(); //handler for progressing looking at smth

	private SceneState SCENESTATEACCESS; //accesses the singleton for the scenestate

	private EventDict EVENTDICT; //accesses the singleton for the event dict

	private PlayerData PLAYERDATA; //accesses the singleton for the PLAYERDATA
	
	public bool CalledOutwidthDialogue = false; //determines whether extra steps are needed
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SCENESTATEACCESS = GetNode<SceneState>("/root/SceneStateSingleton"); //accesses the singleton for the scene state
		EVENTDICT = GetNode<EventDict>("/root/EventDict"); //accesses the singleton for the scene state
		PLAYERDATA = GetNode<PlayerData>("/root/PlayerData"); //accesses the singleton for the scene state
		DIALOGUEACCESS = GetNode<JsonHandler>("/root/DialogueImport"); //accesses the singleton for the dialogue json


		CAMERAS = GetNode<Cameras>("../../../Cameras"); //Gets camera and animation nodes
		AvatarNode = GetNode<TextureRect>("Main_Dialogue/Avatar"); //Gets instance of Avatar
		NameNode = GetNode<Label>("Main_Dialogue/Name Container/Name_Box/Name_Label");
		SpeechNode = GetNode<Label>("Main_Dialogue/Speech_Container/Speech"); //Gets instance of Speech
		
		// Active layers
		Select_Items = GetNode<CanvasLayer>("../../../InteractableItems/Select_Items"); //Gets instance of select items
		Active_Canvas_Layer = Select_Items;

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
	}

	// A function to swap the overlay
	public void SwapOverlay()
	{
		Speech_Overlay.Visible = !Speech_Overlay.Visible;
		Active_Canvas_Layer.Visible = !Active_Canvas_Layer.Visible;
	}

	//Emits a signal to skip dialogue
	public void SkipDialogue()
	{
		EmitSignal("DialogueProgress");
	}

	// A function to disable the primary item overlay, and enable the secondary item overlay
	public void EnableSecondaryItemOverlay() {
		Active_Canvas_Layer = Select_Items_Second;
	}

	// A function to disable the secondary item overlay, and enable the primary item overlay
	public void DisableSecondaryItemOverlay() {
		Active_Canvas_Layer = Select_Items;
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
		bool hiddenBefore = false;
		//Skips the whole thing if the speech overlay isn't visible
		if (Scene != null && SCENESTATEACCESS.PlayerStatus == SceneState.StatusOfPlayer.InDialogue)
		{
			if (!Speech_Overlay.Visible)
			{
				GD.Print("hidden b4");
				//Checks if speech dialogue overlay is vis, swaps it if not
				ValidOverlay();
				ValidGUI();
				hiddenBefore = true;
			}

			//For every bit of speech in the scene
			foreach (var Speech in Scene)
			{
				//Gets current bit of dialogue and saves to a variable
				DIALOGUEACCESS.CURRENT_DIALOGUE = new JsonHandler.DialogueStructData(Speech.Id, Speech.Speaker,
					Speech.Dialogue, Speech.Avatar, Speech.Source);
					SetConversation(string.Format(Speech.Speaker, PLAYERDATA.Player.Name),
					string.Format(Speech.Dialogue, PLAYERDATA.Player.Name),
					string.Format(Speech.Avatar, PLAYERDATA.Player.Avatar));
				await ToSignal(this, "DialogueProgress");
			}

			if (hiddenBefore)
			{
				SwapOverlay();
				ToggleGUIVisible();
				SCENESTATEACCESS.PlayerStatus = SceneState.StatusOfPlayer.LookingAtSomething;
			}
		}

		else
		{
			throw new System.InvalidOperationException(
				"Error: something has gone wrong with parsing the dialogue.json");
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
				//Gets current bit of dialogue and saves to a variable
				DIALOGUEACCESS.CURRENT_DIALOGUE = new JsonHandler.DialogueStructData(Speech.Id, Speech.Speaker,
					Speech.Dialogue, Speech.Avatar, Speech.Source);
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
						case "Inside_Stonewall":
							InsideStoneWallEvent(Speech.Id);
							break;
						default:
							break;
					}
				}

				//Sets the name and speech
				SetNameNode(string.Format(Speech.Speaker, PLAYERDATA.Player.Name));
				SetSpeechNode(string.Format(Speech.Dialogue, PLAYERDATA.Player.Name));
				if (Speech.Avatar != "NULL")
				{
					SetAvatarNode(string.Format(Speech.Avatar, PLAYERDATA.Player.Avatar));
				}
				await ToSignal(this, "DialogueProgress");

			}
		}
	}

    //Just displays
    public async void Dialogue(String name, String description, string avatar)
	{
		ValidOverlay();
		ValidGUI();
		
		SetConversation(name, description, avatar);
		
		//If its called outwidth dialogue, do these extra steps
		if (CalledOutwidthDialogue)
		{
			CalledOutwidthDialogue = false;
			
			if (SCENESTATEACCESS.PlayerStatus == SceneState.StatusOfPlayer.InDialogue)
			{
				await ToSignal(this,"DialogueProgress");
			}

			else if (SCENESTATEACCESS.PlayerStatus == SceneState.StatusOfPlayer.FreeRoam)
			{
				await ToSignal(this,"SceneProgress");
			}

			else if (SCENESTATEACCESS.PlayerStatus == SceneState.StatusOfPlayer.LookingAtSomething)
			{
				await ToSignal(this,"LookProgress");
			}

			//If not the intro cam
			if (GetViewport().GetCamera3D().Name != "IntroCam")
			{
				Speech_Overlay.Visible = false;
				Active_Canvas_Layer.Visible = true;
			}
			
			else
			{
				if (SCENESTATEACCESS.PlayerStatus == SceneState.StatusOfPlayer.LookingAtSomething)
				{
					GD.Print("lookin at smth");
					Active_Canvas_Layer.Visible = true;
					Speech_Overlay.Visible = false;
				}

				else
				{
					this.Visible = false;
				}
			}
			
		}
	}

	//For source display
	public async void Dialogue(Dictionary<string, string> extraInfo)
	{
		//Checks if speech dialogue overlay is vis, swaps it if not
		ValidOverlay();
		ValidGUI();

		// Sets the player name and avatar
		SetNameNode(PLAYERDATA.Player.Name);
		SetAvatarNode(string.Format("res://resources/textures/sprites/main_char/{0}.svg", PLAYERDATA.Player.Avatar));

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

		if (CIRCLES.ReturnCurrentButton().GetParent().Name == "Settings")
		{
			//SwapsOverlay
			Active_Canvas_Layer.Visible = true;
			Speech_Overlay.Visible = false;
		}
	}

	//Forces SpeechOverlay to be active
	private void ValidOverlay()
	{
		if (!Speech_Overlay.Visible)
		{
			Speech_Overlay.Visible = true;
			Active_Canvas_Layer.Visible = false;
		}
	}

	//Forces GUI display to be visible
	private void ValidGUI()
	{
		if (!this.Visible)
		{
			ToggleGUIVisible();
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
		Dialogue("Narrator", SCENESTATEACCESS.CurrentObjective, "res://resources/textures/sprites/guide/1.svg");
	}
	
	//Shows the puzzle hints
	public void ShowHint()
	{
		var hints = (Godot.Collections.Dictionary<string, string>)PARENT.GetMeta("Hints");
		CalledOutwidthDialogue = true;
		if (hints != null)
		{
			if (SCENESTATEACCESS.TimesStuck == -1)
			{
				GD.PrintErr(SCENESTATEACCESS.TimesStuck);

				Dialogue("Narrator", "You've not started a puzzle yet.", "res://resources/textures/sprites/guide/1.svg");
			}
		
			else if (SCENESTATEACCESS.TimesStuck >= 0 && SCENESTATEACCESS.TimesStuck <= 2)
			{
				GD.PrintErr(SCENESTATEACCESS.TimesStuck);

				Dialogue("Narrator", hints["0"], "res://resources/textures/sprites/guide/1.svg");
				SCENESTATEACCESS.TimesStuck++;
			}
		
			else if (SCENESTATEACCESS.TimesStuck > 2 && SCENESTATEACCESS.TimesStuck <= 4)
			{
				GD.PrintErr(SCENESTATEACCESS.TimesStuck);

				Dialogue("Narrator", hints["5"], "res://resources/textures/sprites/guide/1.svg");
				SCENESTATEACCESS.TimesStuck++;

			}
		
			else if (SCENESTATEACCESS.TimesStuck > 4 && SCENESTATEACCESS.TimesStuck <= 6)
			{
				GD.PrintErr(SCENESTATEACCESS.TimesStuck);

				Dialogue("Narrator", hints["10"], "res://resources/textures/sprites/guide/1.svg");
				SCENESTATEACCESS.TimesStuck++;
			}
		
			else if (SCENESTATEACCESS.TimesStuck > 6)
			{
				GD.PrintErr(SCENESTATEACCESS.TimesStuck);

				Dialogue("Narrator", hints["15"], "res://resources/textures/sprites/guide/1.svg");
			}

			else
			{
				GD.PrintErr(SCENESTATEACCESS.TimesStuck);
			}
		}
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
				for (int i = 0; i < 4; i++)
				{
					string path = string.Format("../Pre{0}", i);
					CIRCLES.ToggleSpecificDirection(GetNode<ButtonOverwrite>(path));
				}
				return;

			default:
				return;
		}

	}


	// Controls the events for inside of stonewall
	private async void InsideStoneWallEvent(string id)
	{
		switch (id)
		{
			case "9":
				await ToSignal(this, "DialogueProgress");

				// Sets scene to free roam
				SCENESTATEACCESS.PlayerStatus = SceneState.StatusOfPlayer.FreeRoam; //swaps back to freeroam view
				ToggleGUIVisible();
			
				// Enables the arrows and current cameras
				CAMERAS.SetCurrentCamera(GetViewport().GetCamera3D());
				SwapOverlay();
				break;
			default:
				return;
		}

	}
}

