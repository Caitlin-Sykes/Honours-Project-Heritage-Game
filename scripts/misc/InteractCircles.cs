using Godot;
using System;



public partial class InteractCircles : Node3D
{
	[Signal]
	public delegate void ToggleNorthEventsEventHandler();

	[Signal]
	public delegate void ToggleEastEventsEventHandler();

	[Signal]
	public delegate void ToggleSouthEventsEventHandler(); // These all show/hide the corresponding events for the scene

	[Signal]
	public delegate void ToggleWestEventsEventHandler();

	[Signal]
	public delegate void ToggleUpEventsEventHandler();

	[Signal]
	public delegate void ToggleDownEventsEventHandler();

	[Export]

	private SpeechGUI DIALOGUE; //instance of speech gui

	private string PLAYER_AVATAR = "res://resources/textures/sprites/main_char/{0}.svg";

	private string CURRENT_PATH_CIRCLES { get; set; }

	private MarginContainer BackButtonContainer; //instance of the back button container

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BackButtonContainer = GetNode<MarginContainer>("Select_Items/Settings/Panel/Back_Button");
	}

	// attach unique meta data to each circle imported 
	// means no repetitve scripts
	// show specific node on command ping

	/**
	* Shows Extra Information Handlers
	**/

	// Called from control, on J Key press
	public void ShowExtraInformation()
	{
		DIALOGUE.Dialogue((Godot.Collections.Dictionary<string, string>)GetNode<ButtonOverwrite>(CURRENT_PATH_CIRCLES).GetMeta("ExtraInformation"));
	}

	/**
	* ----------------------------------------------------------------
	* Handlers for enabling and disabling event circles
	* ----------------------------------------------------------------
	**/

	//Enables the events circle
	private void ToggleEventsDirection(ButtonOverwrite cir)
	{

		//parNode is the parent of cir node
		var parNode = (Control)cir.GetParent();
		parNode.Visible = !parNode.Visible;

		//For every circle in the direction container, toggles them
		foreach (ButtonOverwrite circle in parNode.GetChildren())
		{
			circle.Visible = !circle.Visible;
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
			GD.Print("Cannot find the circle to be disabled. Is the path correct? Specific Error is: " + e.Message);
		}


	}


	// Handles on circle click
	//Sets the node path as well

	private async void CirclesPressed(String path)
	{
		SceneState.PlayerStatus = SceneState.StatusOfPlayer.LookingAtSomething;
		CURRENT_PATH_CIRCLES = path;
		//If it has a camera position then
		if (GetNode<ButtonOverwrite>(path).GetMeta("NewCamPos").AsVector3() != Vector3.Zero)
		{

			ToggleEventsDirection(GetNode<ButtonOverwrite>(path)); //turns off all the circles 
			SetCam(GetNode<ButtonOverwrite>(path).GetMeta("NewCamPos").AsVector3(), (float)GetNode<ButtonOverwrite>(path).GetMeta("CamRotation")); //sets the camera to the position and rotation in the meta data
			ToggleBackButton(); //shows the back button
		}

		//Gets meta description of button clicked
		var description = (Godot.Collections.Dictionary<string, string>)GetNode<ButtonOverwrite>(path).GetMeta("Description");

		//If the scene state is Tutorial
		if (SceneState.sceneState == SceneState.CurrentSceneState.Tutorial)
		{
			DIALOGUE.Dialogue(PlayerData.Player.Name, description["Tutorial"], string.Format(PLAYER_AVATAR, PlayerData.Player.Avatar));

			//Swap back to gui speech
			DIALOGUE.SwapOverlay();

			//Awaits the dialogue progression
			await ToSignal(DIALOGUE, "LookProgress");

			//Swap back to casual view
			DIALOGUE.SwapOverlay();


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

		//Renables the circles
		ToggleEventsDirection(GetNode<ButtonOverwrite>(CURRENT_PATH_CIRCLES));

		//Swaps back to dialogue mode
		SceneState.PlayerStatus = SceneState.StatusOfPlayer.InDialogue;

		//Skips dialogue so it doesnt repeat itself
		DIALOGUE.SkipDialogue();

		//Swap back to gui speech and unlock the dialogue
		DIALOGUE.SwapOverlay();

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
}
