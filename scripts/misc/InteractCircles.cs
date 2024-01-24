using Godot;
using System;



public partial class InteractCircles : Node3D
{
	[Export]

	private SpeechGUI DIALOGUE; //instance of speech gui

	private string PLAYER_AVATAR = "res://resources/textures/sprites/main_char/{0}.svg";

	private string CURRENT_PATH_CIRCLES { get; set; }

	private MarginContainer BackButtonContainer; //instance of the back button container

	private SceneState SCENESTATEACCESS; //accesses the singleton for the scenestate

	private PuzzleStart PUZZLES;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BackButtonContainer = GetNode<MarginContainer>("Select_Items/Settings/Panel/Back_Button");
		
		PUZZLES = GetNode<PuzzleStart>("Select_Items/Settings/Puzzles");
		SCENESTATEACCESS = GetNode<SceneState>("/root/SceneStateSingleton"); //accesses the singleton for the scene state
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
		if (cir != null)
		{
			//parNode is the parent of cir node
			var parNode = (Control)cir.GetParent();

			//Toggles parent node
			ToggleParentNode(cir);
			//For every circle in the direction container, toggles them
			foreach (ButtonOverwrite circle in parNode.GetChildren())
			{
				circle.Visible = !circle.Visible;
			}
		}
	}

	//Toggles the visibility of the parent node
	public void ToggleParentNode(ButtonOverwrite cir)
	{
		var parNode = (Control)cir.GetParent();
		parNode.Visible = !parNode.Visible;
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
			GD.PrintErr("Cannot find the circle to be disabled. Is the path correct? Specific Error is: " + e.Message);
		}


	}

	//TODO: dear me, this function needs to be refactored
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
			SetCam(GetNode<ButtonOverwrite>(path).GetMeta("NewCamPos").AsVector3(), (float)GetNode<ButtonOverwrite>(path).GetMeta("CamRotation")); //sets the camera to the position and rotation in the meta data
			ToggleBackButton(); //shows the back button
			PUZZLES.CheckPuzzle(GetNode<ButtonOverwrite>(path)); // checks if there is a puzzle, and whether to display
		}

		//Gets meta description of button clicked
		var description = (Godot.Collections.Dictionary<string, string>)GetNode<ButtonOverwrite>(path).GetMeta("Description");
		DIALOGUE.Dialogue(PlayerData.Player.Name, description[SCENESTATEACCESS.CurrentStateAsString()], string.Format(PLAYER_AVATAR, PlayerData.Player.Avatar));

		//Swap back to gui speech
		DIALOGUE.SwapOverlay();

		DIALOGUE.Visible = true; //toggles the speech gui g

		//Awaits the dialogue progression
		await ToSignal(DIALOGUE, "LookProgress");


		// Swap back to normal view
		DIALOGUE.SwapOverlay();

		//If the camera isn't moved
		if (GetNode<ButtonOverwrite>(path).GetMeta("NewCamPos").AsVector3() == Vector3.Zero)
		{
			if (GetNode<ButtonOverwrite>(path).GetMeta("Object").AsString() != "Door") {
				//Swaps back to dialogue mode
				SCENESTATEACCESS.PlayerStatus = SCENESTATEACCESS.PreviousState;
			} //todo: working on this

			//If tutorial
			if (SCENESTATEACCESS.sceneState == SceneState.CurrentSceneState.Tutorial) {
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
		SetCam(Vector3.Zero, 0); //resets camera position

		ToggleParentNode(GetNode<ButtonOverwrite>(CURRENT_PATH_CIRCLES)); //hides the current node																	  
		ToggleEventsDirection(GetNode<ButtonOverwrite>(CURRENT_PATH_CIRCLES)); //Renables the circles

		//Swaps back to dialogue mode
		SCENESTATEACCESS.PlayerStatus = SCENESTATEACCESS.PreviousState;

		//If the previous state is freeroam
		if (SCENESTATEACCESS.PreviousState == SceneState.StatusOfPlayer.FreeRoam) {
			var parNode = (Control)GetNode<ButtonOverwrite>(CURRENT_PATH_CIRCLES).GetParent();
			
			//Sets visible to true
			if (!parNode.Visible) {
				parNode.Visible = true;
				
			}
		}
		//Skips dialogue so it doesnt repeat itself
		DIALOGUE.SkipDialogue();

		//Swap back to gui speech and unlock the dialogue
		DIALOGUE.SwapOverlay();

		//TODO: Needs so much refractoring
		//BUG: still does not work.
		// this line is for when returning to freeroam after clicking the back button
		if (SCENESTATEACCESS.PlayerStatus == SceneState.StatusOfPlayer.FreeRoam) {
				DIALOGUE.SwapOverlay();
		}

	}

	//On back button click
	// Returns to camera origin
	private void ToggleBackButton()
	{
		BackButtonContainer.Visible = !BackButtonContainer.Visible;
	}

	//Sets current Camera position
	private void SetCam(Vector3 pos, float angle)
	{
		Camera3D curCam = GetViewport().GetCamera3D(); //Gets the current active camera
		curCam.Position = pos; //Sets current camera to the position
		curCam.Rotate(curCam.Transform.Origin, angle); //rotates to the rotation

	}

	/**
	* ----------------------------------------------------------------
	* Emit Signals Handlers
	* ----------------------------------------------------------------
	**/

	//Emits event signal depending on string
	public void EmitEvent(string evnt)
	{

		switch (evnt)
		{
			case "ToggleNorthEvents":
				ToggleEventsDirection(GetNode<ButtonOverwrite>("Select_Items/Settings/Panel/North/1"));
				return;
			case "ToggleEastEvents":
				ToggleEventsDirection(GetNode<ButtonOverwrite>("Select_Items/Settings/Panel/East/1"));
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