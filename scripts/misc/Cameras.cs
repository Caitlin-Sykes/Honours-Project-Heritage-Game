using Godot;
using System;


public partial class Cameras : Node
{

	//enum for directions
	public enum Direction
	{
		North,
		East,
		South,
		West,
		Up,
		Down,
	}

	// enum for state, used for when there's more than one pod of camera
	public enum State
	{
		Enabled,
		Disabled
	}


	[Export]
	public Godot.Collections.Array<Camera3D> CAMERAS { get; set; } //Array of cameras

	// public Direction dir; // instance of direction

	private SceneState SCENESTATEACCESS; //accesses the singleton for the 

	public State state; // instance of state

	public Direction previousDir { get; set; } // used only when going up and down
	private int cameraVisible { get; set; } //holds array position of camera to display

	public bool EnableEvents {get; set; } = false; //enables or disable events

	[Export]
	private InteractCircles INTERACT_CIRCLES; //instance of InteractCircles


	public override void _Ready() {
		state = State.Disabled;
		SCENESTATEACCESS = GetNode<SceneState>("/root/SceneStateSingleton"); //accesses the singleton for the scene state
	}
	/** 
	* ----------------------------------------------------------------
	*	Misc Handlers
	* ----------------------------------------------------------------
	**/

	//A function to convert string to type Direction
	private Direction ToDirection(string dir)
	{
		switch (dir)
		{
			case "North":
				return Direction.North;
			case "East":
				return Direction.East;
			case "South":
				return Direction.South;
			case "West":
				return Direction.West;
			case "Up":
				return Direction.Up;
			case "Down":
				return Direction.Down;
			default:
				throw new ArgumentException("Not of type Direction");
		}
	}

	//A function to convert direction to string
	private static string ToDirection(Direction dir)
	{
		switch (dir)
		{
			case Direction.North:
				return "North";
			case Direction.East:
				return "East";
			case Direction.South:
				return "South";
			case Direction.West:
				return "West";
			case Direction.Up:
				return "Up";
			case Direction.Down:
				return "Down";
			default:
				throw new ArgumentException("Not of type Direction");
		}
	}
	
	//A handler to set the arrows visible when in use
	private void SetArrowsVisible(Camera3D cam)
	{
		//If node matches "*Arrow_Parent" then enables input and visibility
		foreach (Node3D arrow in cam.FindChildren("*Arrow_Parent"))
		{
			//If node matches Up_Arrow_Parent or Down_Arrow_Parent and metadata for camera has UpDown as not enabled
			if ((arrow.Name == "Up_Arrow_Parent" || arrow.Name == "Down_Arrow_Parent") && !GetMeta("UpDownEnabled").AsBool())
			{
				break;
			}

			//Else sets visible
			else 
			{
				arrow.Visible = true;
				arrow.SetProcessInput(true);
			}
		}
	}

	//A handler to set the arrows invisble when not in use
	private void SetArrowsInvisible(Camera3D cam)
	{
		//If node matches "*Arrow_Parent" then disables input and visibility
		foreach (Node3D arrow in cam.FindChildren("*Arrow_Parent"))
		{
			arrow.Visible = false;
			arrow.SetProcessInput(false);
		}
	}


	//A handler for getting array index of direction 
	// returns -1 if invalid direction
	private int GetIndexOfDirection(Direction dir)
	{
		switch (dir)
		{
			case Direction.North:
				return 0;
			case Direction.East:
				return 1;
			case Direction.South:
				return 2;
			case Direction.West:
				return 3;
			case Direction.Up:
				return 4;
			case Direction.Down:
				return 5;
			default:
				GD.PrintErr("An invalid direction was passed through. Please check what you're passing in.");
				return -1;
		}
	}

	//A function to make a single arrow visible
	public void SetSingleArrowVisible(Camera3D cam, string arrowName) {
		//If node matches "*Arrow_Parent" then enables input and visibility
		foreach (Node3D arrow in cam.FindChildren(arrowName))
		{
			arrow.Visible = true; }
	}

	//A function to make a single arrow visible
	public void SetSingleArrowInvisible(Camera3D cam, string arrowName)
	{
		//If node matches "*Arrow_Parent" then enables input and visibility
		foreach (Node3D arrow in cam.FindChildren(arrowName))
		{
			arrow.Visible = false;
		}
	}

	//A function to set the current camera
	public void SetCurrentCamera(Camera3D cam) {
		SetArrowsInvisible(GetViewport().GetCamera3D());
		cam.Current = true;
		SetArrowsVisible(cam);
		SCENESTATEACCESS.DIR = ToDirection(cam.Name);

		//If events are enabled
		if (EnableEvents) {
			INTERACT_CIRCLES.EmitEvent(String.Format("Toggle{0}Events", (SCENESTATEACCESS.DIR)));
	}
	}

	//A function to toggle events
	public void ToggleEvents() {
		EnableEvents = !EnableEvents;
	}
	/** 
	* ----------------------------------------------------------------
	*	Handles moving the cameras
	* ----------------------------------------------------------------
	**/
	
	//Method to handle turning left
	public void TurnLeft()
	{	
		if (this.state != State.Disabled) {
		//Changes camera depending on what the current direction is
		switch (SCENESTATEACCESS.DIR)
		{
			//If current camera is north, set current camera to west
			case Direction.North:
				SCENESTATEACCESS.DIR = Direction.West;
				SetArrowsInvisible(CAMERAS[0]);
				SetArrowsVisible(CAMERAS[3]);
				if (EnableEvents) {
					INTERACT_CIRCLES.EmitEvent("ToggleNorthEvents");
					INTERACT_CIRCLES.EmitEvent("ToggleWestEvents");
				}
				CAMERAS[3].Current = true;
				break;

			//If current camera is east, set current camera to north
			case Direction.East:
				SCENESTATEACCESS.DIR = Direction.North;
				SetArrowsInvisible(CAMERAS[1]);
				SetArrowsVisible(CAMERAS[0]);
				if (EnableEvents)
				{
					INTERACT_CIRCLES.EmitEvent("ToggleEastEvents");
					INTERACT_CIRCLES.EmitEvent("ToggleNorthEvents");
				}
				CAMERAS[0].Current = true;
				break;

			//If current camera is south, set current camera to East
			case Direction.South:
				SCENESTATEACCESS.DIR = Direction.East;
				SetArrowsInvisible(CAMERAS[2]);
				SetArrowsVisible(CAMERAS[1]);
				if (EnableEvents)
				{
					INTERACT_CIRCLES.EmitEvent("ToggleSouthEvents");
					INTERACT_CIRCLES.EmitEvent("ToggleEastEvents");
				}
				CAMERAS[1].Current = true;
				break;

			//If current camera is west, set current camera to south
			case Direction.West:
				SCENESTATEACCESS.DIR = Direction.South;
				SetArrowsInvisible(CAMERAS[3]);
				SetArrowsVisible(CAMERAS[2]);
				if (EnableEvents)
				{
					INTERACT_CIRCLES.EmitEvent("ToggleWestEvents");
					INTERACT_CIRCLES.EmitEvent("ToggleSouthEvents");
				}
				CAMERAS[2].Current = true;
				break;

			default:
				throw new ArgumentException("Not a valid direction");
		}
		}
	}

	//Method to handle turning right
	public void TurnRight()
	{

		if (state != State.Disabled)
		{

			//Changes camera depending on what the current direction is
			switch (SCENESTATEACCESS.DIR)
		{
			//If current camera is north, set current camera to east
			case Direction.North:
				SCENESTATEACCESS.DIR = Direction.East;
				SetArrowsInvisible(CAMERAS[0]);
				SetArrowsVisible(CAMERAS[1]);
				if (EnableEvents)
				{
					INTERACT_CIRCLES.EmitEvent("ToggleNorthEvents");
					INTERACT_CIRCLES.EmitEvent("ToggleEastEvents");
				}
				CAMERAS[1].Current = true;
				break;

			//If current camera is east, set current camera to north
			case Direction.East:
				SCENESTATEACCESS.DIR = Direction.South;
				SetArrowsInvisible(CAMERAS[1]);
				SetArrowsVisible(CAMERAS[2]);
				if (EnableEvents)
				{
					INTERACT_CIRCLES.EmitEvent("ToggleEastEvents");
					INTERACT_CIRCLES.EmitEvent("ToggleSouthEvents");
				}
				CAMERAS[2].Current = true;
				break;

			//If current camera is south, set current camera to East
			case Direction.South:
				SCENESTATEACCESS.DIR = Direction.West;
				SetArrowsInvisible(CAMERAS[2]);
				SetArrowsVisible(CAMERAS[3]);
				if (EnableEvents)
				{
					INTERACT_CIRCLES.EmitEvent("ToggleSouthEvents");
					INTERACT_CIRCLES.EmitEvent("ToggleWestEvents");
				}
				CAMERAS[3].Current = true;
				break;

			//If current camera is west, set current camera to south
			case Direction.West:
				SCENESTATEACCESS.DIR = Direction.North;
				SetArrowsInvisible(CAMERAS[3]);
				SetArrowsVisible(CAMERAS[0]);
				if (EnableEvents)
				{
					INTERACT_CIRCLES.EmitEvent("ToggleWestEvents");
					INTERACT_CIRCLES.EmitEvent("ToggleNorthEvents");
				}
				CAMERAS[0].Current = true;
				break;

			default:
				GD.PrintErr("An invalid direction was passed through. Please confirm your input.");
				throw new ArgumentException("Not a valid direction");
		}}

	}

	//Method to handle looking up (for scenes that are allowed)
	public void TurnUp()
	{
		if (state != State.Disabled)
		{

			//If direction is down, moves camera to previous position
			if (SCENESTATEACCESS.DIR == Direction.Down)
		{

			//Enables camera arrows and makes it main camera
			SetArrowsVisible(CAMERAS[GetIndexOfDirection(previousDir)]);
			CAMERAS[GetIndexOfDirection(previousDir)].Current = true;

			if (EnableEvents)
			{
				INTERACT_CIRCLES.EmitEvent("ToggleDownEvents");
				INTERACT_CIRCLES.EmitEvent(string.Format("Toggle{0}Events", ToDirection(previousDir)));
			}

			//Sets camera arrows for down camera invisible
			SetArrowsInvisible(CAMERAS[5]);

				//Sets current dir to previousDir
				SCENESTATEACCESS.DIR = previousDir;
		}

		//If direction is not up
		else if (SCENESTATEACCESS.DIR != Direction.Up)
		{

			//Sets down camera visible
			SetSingleArrowVisible(CAMERAS[4], "*Down_Arrow_Parent");

			if (EnableEvents)
			{
				INTERACT_CIRCLES.EmitEvent("ToggleUpEvents");
				INTERACT_CIRCLES.EmitEvent(string.Format("Toggle{0}Events", ToDirection(SCENESTATEACCESS.DIR)));
			}

			//Sets current direction arrows invisible
			SetArrowsInvisible(CAMERAS[GetIndexOfDirection(SCENESTATEACCESS.DIR)]);

			//Sets down camera to visible
			CAMERAS[4].Current = true;
			previousDir = SCENESTATEACCESS.DIR;
			SCENESTATEACCESS.DIR = Direction.Up;
		}}
	}




	//Method to handle looking down
	public void TurnDown()
	{
		if (state != State.Disabled)
		{

			//If direction is up, moves camera to previous position
			if (SCENESTATEACCESS.DIR == Direction.Up)
		{

			//Enables camera arrows and makes it main camera
			SetArrowsVisible(CAMERAS[GetIndexOfDirection(previousDir)]);
			CAMERAS[GetIndexOfDirection(previousDir)].Current = true;

			//Sets camera arrows for down camera invisible
			SetArrowsInvisible(CAMERAS[4]);

			if (EnableEvents)
			{
				INTERACT_CIRCLES.EmitEvent("ToggleUpEvents");
				INTERACT_CIRCLES.EmitEvent(string.Format("Toggle{0}Events", ToDirection(previousDir)));
			}


			//Sets new current dir
			SCENESTATEACCESS.DIR = previousDir;
		}

		//If direction is not down
		else if (SCENESTATEACCESS.DIR != Direction.Down)
		{

			//Sets down camera visible
			SetSingleArrowVisible(CAMERAS[5], "*Up_Arrow_Parent");

			//Sets current direction arrows invisible
			SetArrowsInvisible(CAMERAS[GetIndexOfDirection(SCENESTATEACCESS.DIR)]);

			if (EnableEvents)
			{
				INTERACT_CIRCLES.EmitEvent("ToggleUpEvents");
				INTERACT_CIRCLES.EmitEvent(string.Format("Toggle{0}Events", ToDirection(SCENESTATEACCESS.DIR)));
			}

			//Sets down camera to visible
			CAMERAS[5].Current = true;
			previousDir = SCENESTATEACCESS.DIR;
			SCENESTATEACCESS.DIR = Direction.Down;
		}
	}}
}











