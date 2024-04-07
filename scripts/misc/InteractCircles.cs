using Godot;
using Godot.Collections;
using System;
using System.Linq;


public partial class InteractCircles : Node3D
{
	[Export]

	private SpeechGUI DIALOGUE; //instance of speech gui

	private string PLAYER_AVATAR = "res://resources/textures/sprites/main_char/{0}.svg"; //player avatar string
	public MarginContainer BackButtonContainer; //instance of the back button container

	private SceneState SCENESTATEACCESS; //accesses the singleton for the scene state

	private JsonHandler DIALOGUEACCESS; //accesses the singleton for the dialogue json

	public Vector3 PREVIOUS_POS; //vector for previous Cam pos
	public Vector3 PREVIOUS_ANGLE; //vector for previous Cam angle

	private PlayerData PLAYERDATA; //accesses the singleton for the PLAYERDATA

	private PuzzleStart PUZZLES; //instance of puzzles

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BackButtonContainer = GetNode<MarginContainer>("Select_Items/Settings/Panel/Back_Button"); //Gets the back button
		PUZZLES = GetNode<PuzzleStart>("Select_Items/Settings/Puzzles"); //gets instance of puzzles
		SCENESTATEACCESS = GetNode<SceneState>("/root/SceneStateSingleton"); //accesses the singleton for the scene state
		DIALOGUEACCESS = GetNode<JsonHandler>("/root/DialogueImport"); //accesses the singleton for the dialogue json
		PLAYERDATA = GetNode<PlayerData>("/root/PlayerData"); //accesses the singleton for the scene state

	}

	/**
	* ----------------------------------------------------------------
	* Shows Extra Information Handlers
	* ----------------------------------------------------------------
	**/

	// Called from control, on J Key press
	public void ShowExtraInformation()
	{
		DIALOGUE.Dialogue((Godot.Collections.Dictionary<string, string>)GetNode<ButtonOverwrite>(SCENESTATEACCESS.CURRENT_PATH_CIRCLES).GetMeta("ExtraInformation"));
	}

	//Called from control, on K press show sources if looking at smth
	public void ShowSources()
	{
		DIALOGUE.Dialogue((Godot.Collections.Dictionary<string, string>)GetNode<ButtonOverwrite>(SCENESTATEACCESS.CURRENT_PATH_CIRCLES).GetMeta("Sources"));
	}
	
	//Called from control, on K press show sources if in dialogue
	public void ShowDialogueSources()
	{
		//If the current dialogue is not null and the source is not null
		if (DIALOGUEACCESS.CURRENT_DIALOGUE != null && DIALOGUEACCESS.CURRENT_DIALOGUE.Source != "")
		{
			DIALOGUE.Dialogue(DIALOGUEACCESS.CURRENT_DIALOGUE.Speaker, DIALOGUEACCESS.CURRENT_DIALOGUE.Dialogue, "res://resources/textures/sprites/guide/1.svg");	
		}

		else
		{
			DIALOGUE.Dialogue("Narrator", "There is no source for this piece of dialogue",
				"res://resources/textures/sprites/guide/1.svg");
		}
	}



	/**
	* ----------------------------------------------------------------
	* Handlers for enabling and disabling event circles
	* ----------------------------------------------------------------
	**/

	//Enables the events circle
	public void ToggleEventsDirection(ButtonOverwrite cir)
	{
		// If the circle has puzzle enabled value and its true call check puzzle 
		if (cir.HasMeta("PuzzleEnabled") && cir.GetMeta("PuzzleEnabled").AsBool())
		{
			PUZZLES.CheckPuzzle(cir);
			GD.Print("in puzzle enabled in toggle events");
		}

		if (cir.GetParent() is Control parNode) {
			//Toggles parent node
			ToggleParentNode(cir);
			
			//Enables any puzzles
			//For every circle in the direction container, toggles them
			foreach (var circle in parNode.GetChildren())
				{
					//parNode is the parent of cir node
					//Gets the path by getting the circles parent's name and the circles own name (ie, /North/1)
					if (circle is ButtonOverwrite btnO) {
						btnO.Visible = !btnO.Visible;
					}		
				}
			}
	}

	//Toggles the visibility of the parent node
	public void ToggleParentNode(ButtonOverwrite cir)
	{
		if (cir.GetParent() is Control parNode) {
			parNode.Visible = !parNode.Visible;
		}
	}

	//Enables a specific event circle
	public void ToggleSpecificDirection(ButtonOverwrite cir)
	{
		//Gets the circle to be enabled, passed in by path
		try
		{
			cir.GetNode<Control>("..").Visible = true;
			cir.Visible = !cir.Visible;
		}

		catch (InvalidCastException e)
		{
			GD.PrintErr("Cannot find the circle to be disabled. Is the object correct? Specific Error is: " + e.Message);
		}
	}
	
	//Hides a specific puzzle
	public void HideMasterPuzzle()
	{
		GetNode<Control>("Select_Items/Settings/Puzzles").Visible = false;
	}

	//Enables/Disables a specific event circle by path
	public void ToggleSpecificDirectionPath(string cirPath)
	{

		//Gets the circle to be enabled, passed in by path
		try
		{
			GetNode<ButtonOverwrite>(cirPath).Visible = !GetNode<ButtonOverwrite>(cirPath).Visible;
		}

		catch (InvalidCastException e)
		{
			GD.PrintErr("Cannot find the circle to be disabled. Is the path correct? Specific Error is: " + e.Message);
		}


	}

	//A handler for passing in the jsonhandler by string
	public void DialogueByString(string DialogueStructName)
	{
		SCENESTATEACCESS.PlayerStatus = SceneState.StatusOfPlayer.InDialogue;

		switch (DialogueStructName) {
			case "Yvonne":
				DIALOGUE.Speech_Overlay.Visible = false;
				DIALOGUE.Dialogue(DIALOGUEACCESS.Speech.Stonewall_Yvonne);
				return;
			case "Martha":
				DIALOGUE.Speech_Overlay.Visible = false;
				DIALOGUE.Dialogue(DIALOGUEACCESS.Speech.Stonewall_Martha);
				return;
			case "Leitsch":
				DIALOGUE.Speech_Overlay.Visible = false;
				DIALOGUE.Dialogue(DIALOGUEACCESS.Speech.Stonewall_Leitsch);
				return;
			case "Safe_Incorrect_WrongCode":
				DIALOGUE.Dialogue(DIALOGUEACCESS.Speech.Safe_Incorrect_WC);
				return;
			case "Safe_Incorrect_TMD":
				DIALOGUE.Dialogue(DIALOGUEACCESS.Speech.Safe_Incorrect_TMD);
				return;
			default:
				GD.PrintErr("Tried accessing a dialogue that's not in the switch case.");
				return;
		}
	}


	/**
	* ----------------------------------------------------------------
	* Circles Pressed function and associated functions
	* ----------------------------------------------------------------
	**/

	// Handles on circle click
	public async void CirclesPressed(String path)
	{
		GD.Print("--------" + path);
		// Circles init stuff
		CirclesInit();

		// Sets the circles current path to path
		SCENESTATEACCESS.CURRENT_PATH_CIRCLES = path;

		CheckForCamPosition();
		
		//Gets meta description of button clicked 
		var description = (Godot.Collections.Dictionary<string, string>)GetNode<ButtonOverwrite>(path).GetMeta("Description");
		
		// Validates the description
		ValidateDescription(description);

		//Swap back to gui speech
		DIALOGUE.SwapOverlay();

		// In case the order gets messed up
		if (!DIALOGUE.Speech_Overlay.Visible) {
			DIALOGUE.SwapOverlay();
		}
		//toggles the speech gui
		DIALOGUE.Visible = true; 
		
		//Awaits the dialogue progression
		if (SCENESTATEACCESS.PlayerStatus == SceneState.StatusOfPlayer.LookingAtSomething)
		{
			await ToSignal(DIALOGUE, "LookProgress");
		}
		
		else if (SCENESTATEACCESS.PlayerStatus == SceneState.StatusOfPlayer.InDialogue)
		{
			await ToSignal(DIALOGUE, "DialogueProgress");
		}

		else if (SCENESTATEACCESS.PlayerStatus == SceneState.StatusOfPlayer.FreeRoam)
		{
			await ToSignal(DIALOGUE, "SceneProgress");
		}

		else
		{
			GD.PrintErr("shouldnt rly be here");
		}
		
		GD.Print("aftr waiting");
		// Swap back to free-roam view
		DIALOGUE.SwapOverlay();
		
		//If the camera isn't moved
		if (GetNode<ButtonOverwrite>(path).GetMeta("NewCamPos").AsVector3() == Vector3.Zero)
		{
			GD.Print("in the no move func");
			if (GetNode<ButtonOverwrite>(path).GetMeta("Object").AsString() != "Door")
			{
				//Swaps back to dialogue mode
				SCENESTATEACCESS.PlayerStatus = SCENESTATEACCESS.PreviousState;
				GD.Print("Player status is now: " + SCENESTATEACCESS.PlayerStatus);
				if (DIALOGUE.Speech_Overlay.Visible != false)
				{
					DIALOGUE.Speech_Overlay.Visible = false;
					DIALOGUE.Active_Canvas_Layer.Visible = true;
				}
			}
		
			//If tutorial
			if (SCENESTATEACCESS.sceneState == SceneState.CurrentSceneState.Tutorial)
			{
				// Swap back to gui view
				DIALOGUE.SwapOverlay();
		
				SCENESTATEACCESS.PlayerStatus = SceneState.StatusOfPlayer.InDialogue; //swaps the status to in dialogue
				DIALOGUE.SkipDialogue(); //skips dialogue
			}
		}
		
		GD.Print("end func");

	}

	//A function to check whether the cam needs to be moved
	private void CheckForCamPosition() {
		//If it has a camera position then
		if (GetNode<ButtonOverwrite>(SCENESTATEACCESS.CURRENT_PATH_CIRCLES).GetMeta("NewCamPos").AsVector3() != Vector3.Zero)
		{
			// Toggles direction only if parent isnt called settings
			if (GetNode<ButtonOverwrite>(SCENESTATEACCESS.CURRENT_PATH_CIRCLES).GetParent().Name != "Settings")
			{
				GD.Print("in check cam for pos");
				ToggleEventsDirection(GetNode<ButtonOverwrite>(SCENESTATEACCESS.CURRENT_PATH_CIRCLES)); //turns off all the circles 
			}

			SetCam(GetNode<ButtonOverwrite>(SCENESTATEACCESS.CURRENT_PATH_CIRCLES).GetMeta("NewCamPos").AsVector3(), (Vector3)GetNode<ButtonOverwrite>(SCENESTATEACCESS.CURRENT_PATH_CIRCLES).GetMeta("CamRotation")); //sets the camera to the position and rotation in the meta data
			ToggleBackButton(); //shows the back button
		}
	}

	//A function to initialise the things needed for CirclesPressed
	private void CirclesInit() {
		//if previous stage != PlayerStatus.Dialogue (aka, coming from freeroam), make gui visible!
		if (SCENESTATEACCESS.PlayerStatus != SceneState.StatusOfPlayer.InDialogue)
		{
			DIALOGUE.ToggleGUIVisible();
		}

		SCENESTATEACCESS.PreviousState = SCENESTATEACCESS.PlayerStatus; //sets the previous state
		SCENESTATEACCESS.PlayerStatus = SceneState.StatusOfPlayer.LookingAtSomething;
	}

	//A function to validate the description metadata
	private void ValidateDescription(Dictionary<string, string> description) {
		try {
			// If has key then shows dialogue, if not, then break
			 if (description.ContainsKey(SCENESTATEACCESS.CurrentStateAsString()))
			 {
			 	if (this.GetParent().Name != "IntroductionScene")
			 	{
			 		DIALOGUE.SwapOverlay();
			 	}
			
			 	DIALOGUE.Dialogue(PLAYERDATA.Player.Name, description[SCENESTATEACCESS.CurrentStateAsString()], string.Format(PLAYER_AVATAR, PLAYERDATA.Player.Avatar));
			 }
			
			//Breaks out the function if it doesn't have the key
			else
			{
				if (GetNode<ButtonOverwrite>(SCENESTATEACCESS.CURRENT_PATH_CIRCLES).GetMeta("Object").AsString() != "Door")
				{
					//Swaps back to dialogue mode
					SCENESTATEACCESS.PlayerStatus = SCENESTATEACCESS.PreviousState;
				}
			
				else
				{
					SCENESTATEACCESS.PlayerStatus = SceneState.StatusOfPlayer.FreeRoam; //swaps the status to in dialogue
				}
			}
		}
		catch (Exception e) { 
			GD.PrintErr("Failed to get the meta data of the passed in dictionary: " + e);
			GD.PrintErr(string.Format("Dictionary: {0}, CurrentSceneState: {1}, PlayerStatus: {2}", description, SCENESTATEACCESS.CurrentStateAsString(), SCENESTATEACCESS.PlayerStatus));
		}
	

	}

	/**
	* ----------------------------------------------------------------
	* Misc Handlers
	* ----------------------------------------------------------------
	**/

	//Returns current button gameobject from path
	public ButtonOverwrite ReturnCurrentButton()
	{
		return GetNode<ButtonOverwrite>(SCENESTATEACCESS.CURRENT_PATH_CIRCLES);
	}
	
	//On back button click
	// Returns to camera origin
	private void OnBackPressed()
	{
		GD.Print(GetNode<ButtonOverwrite>(SCENESTATEACCESS.CURRENT_PATH_CIRCLES).Name);
		
		ToggleBackButton(); //hides the back button
		ResetCam(); //resets camera position

		// If not already visible, show.
		if (!GetNode<ButtonOverwrite>(SCENESTATEACCESS.CURRENT_PATH_CIRCLES).Visible) {
			ToggleEventsDirection(GetNode<ButtonOverwrite>(SCENESTATEACCESS.CURRENT_PATH_CIRCLES)); //Renables the circles
		}

		//Swaps back to dialogue mode
		SCENESTATEACCESS.PlayerStatus = SCENESTATEACCESS.PreviousState;

		//If the previous state is freeroam
		if (SCENESTATEACCESS.PreviousState == SceneState.StatusOfPlayer.FreeRoam)
		{
			var parNode = (Control)GetNode<ButtonOverwrite>(SCENESTATEACCESS.CURRENT_PATH_CIRCLES).GetParent();

			//Sets visible to true
			if (!parNode.Visible)
			{
				parNode.Visible = true;

			}
		}
		//Skips dialogue so it doesnt repeat itself
		DIALOGUE.SkipDialogue();

		//Swap back to gui speech and unlock the dialogue
		DIALOGUE.SwapOverlay();

		// this line is for when returning to freeroam after clicking the back button
		if (SCENESTATEACCESS.PlayerStatus == SceneState.StatusOfPlayer.FreeRoam && GetViewport().GetCamera3D().Name != "IntroCam")
		{
			DIALOGUE.SwapOverlay();
		}
		// bug fixing function for one complicated scene 
		else if (GetTree().CurrentScene.Name == "Stonewall") {
			DIALOGUE.ToggleGUIVisible();
			
			//Rly bad practice, this entire script ideally needs a rewrite
			if (ReturnCurrentButton().Name == "1" && ReturnCurrentButton().GetMeta("Object").ToString() == "Yvonne" || ReturnCurrentButton().Name == "2" && ReturnCurrentButton().GetMeta("Object").ToString() == "Martha" || ReturnCurrentButton().Name == "3" && ReturnCurrentButton().GetMeta("Object").ToString() == "Leitsch")
			{
				DIALOGUE.Speech_Overlay.Visible = false;
				DIALOGUE.Active_Canvas_Layer.Visible = true;
				SCENESTATEACCESS.PlayerStatus = SceneState.StatusOfPlayer.FreeRoam;
			}
			//For one specific broken button where on back it does not go back so needs its own code to fix it
			else if (ReturnCurrentButton().GetMeta("Object").ToString() == "TriggerEventBack" || ReturnCurrentButton().GetMeta("Object").ToString() == "TriggerEvent" && ReturnCurrentButton().Name == "2" ||  ReturnCurrentButton().Name == "c1" ||  ReturnCurrentButton().Name == "c2")
			{
				GD.Print("pingi");
				SCENESTATEACCESS.PlayerStatus = SceneState.StatusOfPlayer.FreeRoam;
				DIALOGUE.Select_Items.Visible = true;
				DIALOGUE.Speech_Overlay.Visible = false;
				// HideMasterPuzzle();
				PUZZLES.TogglePuzzleVisibility(GetNode<ButtonOverwrite>("Select_Items/Settings/Puzzles/PuzzlesPanel/West/PuzzleCont/2"));
				ToggleEventsDirection(GetNode<ButtonOverwrite>("Select_Items/Settings/Panel/West/1"));
			}
		}
	}
	
	//Toggles the back button visibility
	public void ToggleBackButton()
	{
		BackButtonContainer.Visible = !BackButtonContainer.Visible;
	}

	//Sets current Camera position
	public void SetCam(Vector3 pos, Vector3 angle)
	{
		
			Camera3D curCam = GetViewport().GetCamera3D(); //Gets the current active camera
		
			PREVIOUS_POS = curCam.Position;
			PREVIOUS_ANGLE = curCam.RotationDegrees;
		
			curCam.Position = pos; //Sets current camera to the position
			curCam.RotationDegrees = angle; //rotates to the rotation

	}

	//Sets camera using cam variable
	public void SetCam(Camera3D cam)
	{
		Camera3D curCam = GetViewport().GetCamera3D(); //Gets the current active camera
		curCam.Position = cam.Position; //Sets current camera to the position
		curCam.Rotate(curCam.Transform.Origin, 0); //rotates to the rotation

	}

	public void ResetCam() {
		Camera3D curCam = GetViewport().GetCamera3D(); //Gets the current active camera
	
		if (this.Name == "InteractableItems2" && GetNode<ButtonOverwrite>(SCENESTATEACCESS.CURRENT_PATH_CIRCLES).Name == "2" && GetNode<ButtonOverwrite>(SCENESTATEACCESS.CURRENT_PATH_CIRCLES).GetParent().GetParent().Name == "West")
		{
			GD.Print("two pinged, in int item 2");
			Safecracking sc = GetNode<Safecracking>("Select_Items/Settings/Puzzles/PuzzlesPanel/West");
			
			curCam.Position = sc.BACKUP_POS; //Sets current camera to the position
			curCam.RotationDegrees = sc.BACKUP_ANGLE; //rotates to the rotation
			ToggleEventsDirection(GetNode<ButtonOverwrite>("Select_Items/Settings/Panel/West/2"));
		}
		
		else 
		{
			GD.Print("yh");
			try
			{
				if (this.Name == "InteractableItems" &&
				    GetNode<ButtonOverwrite>(SCENESTATEACCESS.CURRENT_PATH_CIRCLES).Name == "2" &&
				    GetTree().CurrentScene.Name == "Stonewall")
				{
					if (GetNode<ButtonOverwrite>(SCENESTATEACCESS.CURRENT_PATH_CIRCLES).GetParent().GetParent().Name ==
					    "West")
					{
						GD.Print("ping west");
						ToggleEventsDirection(GetNode<ButtonOverwrite>("Select_Items/Settings/Panel/West/2"));
					}
					
					else if (GetNode<ButtonOverwrite>(SCENESTATEACCESS.CURRENT_PATH_CIRCLES).GetParent().GetParent()
						         .Name == "North")
					{
						GD.Print("ping north");
						ToggleEventsDirection(GetNode<ButtonOverwrite>("Select_Items/Settings/Panel/North/2"));
					}
					
					// SCENESTATEACCESS.PlayerStatus = SceneState.StatusOfPlayer.FreeRoam;
					// DIALOGUE.Select_Items.Visible = true;
					// DIALOGUE.Speech_Overlay.Visible = false;

				}
				
			
			}

			catch(Exception e)
			{
				GD.Print("Weird error: " + e);
			}
			
			curCam.Position = PREVIOUS_POS; //Sets current camera to the position
			curCam.RotationDegrees = PREVIOUS_ANGLE; //rotates to the rotation
		}
		
	}

	/**
	* ----------------------------------------------------------------
	* Emit Signals Handlers
	* ----------------------------------------------------------------
	**/

	//Toggles event direction depending on string
	public void EmitEvent(string evnt)
	{

		switch (evnt)
		{
			case "ToggleNorthEvents":
				ToggleEventsDirection(GetNode<ButtonOverwrite>("Select_Items/Settings/Panel/North/1"));
				return;
			case "ToggleEastEvents":
				ToggleEventsDirection(GetNode<ButtonOverwrite>("Select_Items/Settings/Panel/East/5"));
				return;
			case "ToggleSouthEvents":
				ToggleEventsDirection(GetNode<ButtonOverwrite>("Select_Items/Settings/Panel/South/1"));
				return;
			case "ToggleWestEvents":
				ToggleEventsDirection(GetNode<ButtonOverwrite>("Select_Items/Settings/Panel/West/1"));
				return;
			case "ToggleUpEvents":
				ToggleEventsDirection(GetNode<ButtonOverwrite>("Select_Items/Settings/Panel/Up/1"));
				return;
			case "ToggleDownEvents":
				ToggleEventsDirection(GetNode<ButtonOverwrite>("Select_Items/Settings/Panel/Down/1"));
				return;
			default:
				return;
		}
	}

}