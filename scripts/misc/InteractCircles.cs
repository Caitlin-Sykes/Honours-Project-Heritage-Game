using Godot;
using System;
using System.Runtime.CompilerServices;



public partial class InteractCircles : Node3D
{
	[Export]

	private SpeechGUI DIALOGUE; //instance of speech gui

	private string PLAYER_AVATAR = "res://resources/textures/sprites/main_char/{0}.svg"; //player avatar string

	private string CURRENT_PATH_CIRCLES { get; set; } //current circle

	private MarginContainer BackButtonContainer; //instance of the back button container

	private SceneState SCENESTATEACCESS; //accesses the singleton for the 

	private JsonHandler DIALOGUEACCESS; //accesses the singleton for the dialogue json

	public Vector3 PREVIOUS_POS; //vector for previous Cam pos
	public Vector3 PREVIOUS_ANGLE; //vector for previous Cam angle

	private PuzzleStart PUZZLES; //instance of puzzles

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BackButtonContainer = GetNode<MarginContainer>("Select_Items/Settings/Panel/Back_Button"); //Gets the back button
		PUZZLES = GetNode<PuzzleStart>("Select_Items/Settings/Puzzles"); //gets instance of puzzles
		SCENESTATEACCESS = GetNode<SceneState>("/root/SceneStateSingleton"); //accesses the singleton for the scene state
		DIALOGUEACCESS = GetNode<JsonHandler>("/root/DialogueImport"); //accesses the singleton for the dialogue json
	}

	/**
	* ----------------------------------------------------------------
	* Shows Extra Information Handlers
	* ----------------------------------------------------------------
	**/

	// Called from control, on J Key press
	public void ShowExtraInformation()
	{
		DIALOGUE.Dialogue((Godot.Collections.Dictionary<string, string>)GetNode<ButtonOverwrite>(CURRENT_PATH_CIRCLES).GetMeta("ExtraInformation"));
	}

	//Called from control, on K press
	public void ShowSources()
	{
		DIALOGUE.Dialogue((Godot.Collections.Dictionary<string, string>)GetNode<ButtonOverwrite>(CURRENT_PATH_CIRCLES).GetMeta("Sources"));
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
		switch(DialogueStructName) {
			case "Yvonne":
				DIALOGUE.Dialogue(DIALOGUEACCESS.Speech.Stonewall_Yvonne);
				return;
			case "Martha":
				DIALOGUE.Dialogue(DIALOGUEACCESS.Speech.Stonewall_Martha);
				return;
			case "Leitsch":
				DIALOGUE.Dialogue(DIALOGUEACCESS.Speech.Stonewall_Leitsch);
				return;
			default:
				GD.PrintErr("Tried accessing a dialogue that's not in the switch case.");
				return;
		}
	}

	// Handles on circle click
	//Sets the node path as well

	public async void CirclesPressed(String path)
	{
		//if previous stage != PlayerStatus.Dialogue (aka, coming from freeroam), make gui visible!
		if (SCENESTATEACCESS.PlayerStatus != SceneState.StatusOfPlayer.InDialogue)
		{
			DIALOGUE.ToggleGUIVisible();
		}

		SCENESTATEACCESS.PreviousState = SCENESTATEACCESS.PlayerStatus; //sets the previous state
		SCENESTATEACCESS.PlayerStatus = SceneState.StatusOfPlayer.LookingAtSomething;
		CURRENT_PATH_CIRCLES = path;


		//If it has a camera position then
		if (GetNode<ButtonOverwrite>(path).GetMeta("NewCamPos").AsVector3() != Vector3.Zero)
		{

			ToggleEventsDirection(GetNode<ButtonOverwrite>(path)); //turns off all the circles 
			SetCam(GetNode<ButtonOverwrite>(path).GetMeta("NewCamPos").AsVector3(), (Vector3)GetNode<ButtonOverwrite>(path).GetMeta("CamRotation")); //sets the camera to the position and rotation in the meta data
			ToggleBackButton(); //shows the back button
		}


		//Gets meta description of button clicked 
		var description = (Godot.Collections.Dictionary<string, string>)GetNode<ButtonOverwrite>(path).GetMeta("Description");
		
		//If has key then shows dialogue, if not, then break
		if (description.ContainsKey(SCENESTATEACCESS.CurrentStateAsString()))
		{
			DIALOGUE.Dialogue(PlayerData.Player.Name, description[SCENESTATEACCESS.CurrentStateAsString()], string.Format(PLAYER_AVATAR, PlayerData.Player.Avatar));
		}

		//Breaks out the function if it doesn't have the key
		else
		{
			if (GetNode<ButtonOverwrite>(path).GetMeta("Object").AsString() != "Door")
			{
				//Swaps back to dialogue mode
				SCENESTATEACCESS.PlayerStatus = SCENESTATEACCESS.PreviousState;
				return;
			}

			else
			{
				SCENESTATEACCESS.PlayerStatus = SceneState.StatusOfPlayer.FreeRoam; //swaps the status to in dialogue
				return;
			}
		}



		//Swap back to gui speech
		DIALOGUE.SwapOverlay();

		DIALOGUE.Visible = true; //toggles the speech gui

		//Awaits the dialogue progression
		await ToSignal(DIALOGUE, "LookProgress");


		// Swap back to normal view
		DIALOGUE.SwapOverlay();

		//If the camera isn't moved
		if (GetNode<ButtonOverwrite>(path).GetMeta("NewCamPos").AsVector3() == Vector3.Zero)
		{
			if (GetNode<ButtonOverwrite>(path).GetMeta("Object").AsString() != "Door")
			{
				//Swaps back to dialogue mode
				SCENESTATEACCESS.PlayerStatus = SCENESTATEACCESS.PreviousState;
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
	}



	/**
	* ----------------------------------------------------------------
	* Misc Handlers
	* ----------------------------------------------------------------
	**/

	//On back button click
	// Returns to camera origin
	private void OnBackPressed()
	{
		ToggleBackButton(); //hides the back button
		ResetCam(); //resets camera position

		ToggleParentNode(GetNode<ButtonOverwrite>(CURRENT_PATH_CIRCLES)); //hides the current node																	  
		ToggleEventsDirection(GetNode<ButtonOverwrite>(CURRENT_PATH_CIRCLES)); //Renables the circles

		//Swaps back to dialogue mode
		SCENESTATEACCESS.PlayerStatus = SCENESTATEACCESS.PreviousState;

		//If the previous state is freeroam
		if (SCENESTATEACCESS.PreviousState == SceneState.StatusOfPlayer.FreeRoam)
		{
			var parNode = (Control)GetNode<ButtonOverwrite>(CURRENT_PATH_CIRCLES).GetParent();

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
		if (SCENESTATEACCESS.PlayerStatus == SceneState.StatusOfPlayer.FreeRoam)
		{
			DIALOGUE.SwapOverlay();
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
		curCam.Position = PREVIOUS_POS; //Sets current camera to the position
		curCam.RotationDegrees = PREVIOUS_ANGLE; //rotates to the rotation
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