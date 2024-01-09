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

	/**
	* Shows Extra Information Handlers
	**/

	// Called from control, on J Key press
	public void ShowExtraInformation()
	{
		DIALOGUE.Dialogue((Godot.Collections.Dictionary<string, string>)GetNode<ButtonOverwrite>(CURRENT_PATH_CIRCLES).GetMeta("ExtraInformation"));
	}

	public void ShowSources() {
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
		if (cir != null) {
			GD.Print("cir: " + cir);
			//parNode is the parent of cir node
			var parNode = (Control)cir.GetParent();

			//Toggles parent node
			ToggleParentNode(cir);

			//For every circle in the direction container, toggles them
			foreach (ButtonOverwrite circle in parNode.GetChildren())
			{
				GD.Print(circle.Name);
				GD.Print("b4: "+ circle.Visible);
				circle.Visible = !circle.Visible;
				GD.Print("aftr: " + circle.Visible);

				GD.Print("---------------");

			}
		}
	}

	//Toggles the visibility of the parent node
	public void ToggleParentNode(ButtonOverwrite cir) {
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
			GD.Print("Cannot find the circle to be disabled. Is the path correct? Specific Error is: " + e.Message);
		}


	}


	// Handles on circle click
	//Sets the node path as well

	public async void CirclesPressed(String path)
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
			DIALOGUE.Dialogue(PlayerData.Player.Name, description[SceneState.CurrentStateAsString()], string.Format(PLAYER_AVATAR, PlayerData.Player.Avatar));

			//Swap back to gui speech
			DIALOGUE.SwapOverlay();

			//Awaits the dialogue progression
			await ToSignal(DIALOGUE, "LookProgress");


			// Swap back to normal view
			DIALOGUE.SwapOverlay();

			//If the camera isn't moved
			if (GetNode<ButtonOverwrite>(path).GetMeta("NewCamPos").AsVector3() == Vector3.Zero) {

				// Swap back to gui view
				DIALOGUE.SwapOverlay(); 

				SceneState.PlayerStatus = SceneState.StatusOfPlayer.InDialogue; //swaps the status to in dialogue
				DIALOGUE.SkipDialogue(); //skips dialogue
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

	/**
	* ----------------------------------------------------------------
	* Emit Signals Handlers
	* ----------------------------------------------------------------
	**/

	//Emits event signal depending on string
	public void EmitEvent(string evnt) {

		switch (evnt) {
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