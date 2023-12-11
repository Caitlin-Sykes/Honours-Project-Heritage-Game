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

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// attach unique meta data to each circle imported 
	// means no repetitve scripts
	// show specific node on command ping

	/**
	* ----------------------------------------------------------------
	* Handlers for enabling and disabling event circles
	* ----------------------------------------------------------------
	**/

	//Enables the events circle
	private void ToggleEventsDirection(NodePath path)
	{

		//Gets the direction to be enabled, passed in by path
		BoxContainer dir = GetNode<BoxContainer>(path);

		//For every circle in the direction container, toggles them
		foreach (TextureButton circle in dir.GetChildren())
		{
			circle.Visible = !circle.Visible;
		}
	}

	//Enables a specific event circle
	public void ToggleSpecificDirection(ButtonOverwrite cir)
	{

		//Gets the circle to be enabled, passed in by path
		try {
			cir.GetNode<PanelContainer>("..").Visible = true;
			cir.Visible = !cir.Visible;
		}

		catch (InvalidCastException e) {
			GD.Print("Cannot find the circle to be disabled. Is the path correct? Specific Error is: " + e.Message);
		}

		
	}


	/**
	* ----------------------------------------------------------------
	* Handles on circle click
	* ----------------------------------------------------------------
	**/

	//TODO: bug, wont trigger
	private void CirclesPressed(InputEvent @evnt, String path)
	{
		//If trigger is left click
		if (@evnt is InputEventMouseButton mouse)
		{
			GD.Print("ping");
			if (mouse.ButtonIndex == MouseButton.Left && @evnt.IsPressed())
			{
	
				//Gets meta of button clicked
				var meta = (Godot.Collections.Dictionary<string, string>)GetNode<ButtonOverwrite>(path).GetMeta("Description");

				//If the scene state is Tutorial
				if (SceneState.sceneState == SceneState.CurrentSceneState.Tutorial)
				{
					DIALOGUE.Dialogue(PlayerData.Player.Name, meta["Tutorial"], string.Format(PLAYER_AVATAR, PlayerData.Player.Avatar));
					DIALOGUE.SwapOverlay();
				}
			}
		}
	}


}
