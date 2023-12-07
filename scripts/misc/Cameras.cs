using Godot;
using System;


public partial class Cameras : Node
{

	public enum Direction
	{
		North,
		East,
		South,
		West,
		Up,
		Down,
	} //enum for directions


	[Export]
	private Godot.Collections.Array<Camera3D> CAMERAS { get; set; }

	public Direction dir; // instance of direction
	public Direction previousDir { get; set; } // used only when going up and down
	private int cameraVisible { get; set; } //holds array position of camera to display

	public bool EnableEvents {get; set; } = false; //;

	[Export]
	private InteractCircles INTERACT_CIRCLES; //instance of InteractCircles
	

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
			if ((arrow.Name == "Up_Arrow_Parent" || arrow.Name == "Down_Arrow_Parent") && GetMeta("UpDownEnabled").AsBool() == false)
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
		dir = ToDirection(cam.Name);

		if (EnableEvents == true) {
			EmitSignal(string.Format("Toggle{}Events", (dir)));
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
	//TODO: much later- but when selecting something, show a circle which slowly gets smaller.
	//BUG: camera moving can be a bit buggy if clicked fast in quick succession
	//Method to handle turning left
	public void TurnLeft()
	{
		//Changes camera depending on what the current direction is
		switch (dir)
		{
			//If current camera is north, set current camera to west
			case Direction.North:
				dir = Direction.West;
				SetArrowsInvisible(CAMERAS[0]);
				SetArrowsVisible(CAMERAS[3]);
				if (EnableEvents == true) {
					EmitSignal("ToggleNorthEvents");
					EmitSignal("ToggleWestEvents");
				}
				CAMERAS[3].Current = true;
				break;

			//If current camera is east, set current camera to north
			case Direction.East:
				dir = Direction.North;
				SetArrowsInvisible(CAMERAS[1]);
				SetArrowsVisible(CAMERAS[0]);
				if (EnableEvents == true)
				{
					EmitSignal("ToggleEastEvents");
					EmitSignal("ToggleNorthEvents");
				}
				CAMERAS[0].Current = true;
				break;

			//If current camera is south, set current camera to East
			case Direction.South:
				dir = Direction.East;
				SetArrowsInvisible(CAMERAS[2]);
				SetArrowsVisible(CAMERAS[1]);
				if (EnableEvents == true)
				{
					EmitSignal("ToggleSouthEvents");
					EmitSignal("ToggleEastEvents");
				}
				CAMERAS[1].Current = true;
				break;

			//If current camera is west, set current camera to south
			case Direction.West:
				dir = Direction.South;
				SetArrowsInvisible(CAMERAS[3]);
				SetArrowsVisible(CAMERAS[2]);
				if (EnableEvents == true)
				{
					EmitSignal("ToggleWestEvents");
					EmitSignal("ToggleSouthEvents");
				}
				CAMERAS[2].Current = true;
				break;

			default:
				throw new ArgumentException("Not a valid direction");
		}

	}

	//Method to handle turning right
	public void TurnRight()
	{
		//Changes camera depending on what the current direction is
		switch (dir)
		{
			//If current camera is north, set current camera to east
			case Direction.North:
				dir = Direction.East;
				SetArrowsInvisible(CAMERAS[0]);
				SetArrowsVisible(CAMERAS[1]);
				if (EnableEvents == true)
				{
					EmitSignal("ToggleNorthEvents");
					EmitSignal("ToggleEastEvents");
				}
				CAMERAS[1].Current = true;
				break;

			//If current camera is east, set current camera to north
			case Direction.East:
				dir = Direction.South;
				SetArrowsInvisible(CAMERAS[1]);
				SetArrowsVisible(CAMERAS[2]);
				if (EnableEvents == true)
				{
					EmitSignal("ToggleEastEvents");
					EmitSignal("ToggleSouthEvents");
				}
				CAMERAS[2].Current = true;
				break;

			//If current camera is south, set current camera to East
			case Direction.South:
				dir = Direction.West;
				SetArrowsInvisible(CAMERAS[2]);
				SetArrowsVisible(CAMERAS[3]);
				if (EnableEvents == true)
				{
					EmitSignal("ToggleSouthEvents");
					EmitSignal("ToggleWestEvents");
				}
				CAMERAS[3].Current = true;
				break;

			//If current camera is west, set current camera to south
			case Direction.West:
				dir = Direction.North;
				SetArrowsInvisible(CAMERAS[3]);
				SetArrowsVisible(CAMERAS[0]);
				if (EnableEvents == true)
				{
					EmitSignal("ToggleWestEvents");
					EmitSignal("ToggleNorthEvents");
				}
				CAMERAS[0].Current = true;
				break;

			default:
				throw new ArgumentException("Not a valid direction");
		}

	}

	//Method to handle looking up (for scenes that are allowed)
	public void TurnUp()
	{
		//If direction is down, moves camera to previous position
		if (dir == Direction.Down)
		{

			//Enables camera arrows and makes it main camera
			SetArrowsVisible(CAMERAS[GetIndexOfDirection(previousDir)]);
			CAMERAS[GetIndexOfDirection(previousDir)].Current = true;

			if (EnableEvents == true)
			{
				EmitSignal("ToggleDownEvents");
				EmitSignal(string.Format("Toggle{}Events", ToDirection(previousDir)));
			}

			//Sets camera arrows for down camera invisible
			SetArrowsInvisible(CAMERAS[5]);

			//Sets current dir to previousDir
			dir = previousDir;
		}

		//If direction is not up
		else if (dir != Direction.Up)
		{

			//Sets down camera visible
			SetSingleArrowVisible(CAMERAS[4], "*Down_Arrow_Parent");

			if (EnableEvents == true)
			{
				EmitSignal("ToggleUpEvents");
				EmitSignal(string.Format("Toggle{}Events", ToDirection(dir)));
			}

			//Sets current direction arrows invisible
			SetArrowsInvisible(CAMERAS[GetIndexOfDirection(dir)]);

			//Sets down camera to visible
			CAMERAS[4].Current = true;
			previousDir = dir;
			dir = Direction.Up;
		}

		//todo: else later play "oof doesnt work" sound, or a sound indicative of that
	}




	//Method to handle looking down
	public void TurnDown()
	{
		//If direction is up, moves camera to previous position
		if (dir == Direction.Up)
		{

			//Enables camera arrows and makes it main camera
			SetArrowsVisible(CAMERAS[GetIndexOfDirection(previousDir)]);
			CAMERAS[GetIndexOfDirection(previousDir)].Current = true;

			//Sets camera arrows for down camera invisible
			SetArrowsInvisible(CAMERAS[4]);

			if (EnableEvents == true)
			{
				EmitSignal("ToggleUpEvents");
				EmitSignal(string.Format("Toggle{}Events", ToDirection(previousDir)));
			}


			//Sets new current dir
			dir = previousDir;
		}

		//If direction is not down
		else if (dir != Direction.Down)
		{

			//Sets down camera visible
			SetSingleArrowVisible(CAMERAS[5], "*Up_Arrow_Parent");

			//Sets current direction arrows invisible
			SetArrowsInvisible(CAMERAS[GetIndexOfDirection(dir)]);

			if (EnableEvents == true)
			{
				EmitSignal("ToggleUpEvents");
				EmitSignal(string.Format("Toggle{}Events", ToDirection(dir)));
			}

			//Sets down camera to visible
			CAMERAS[5].Current = true;
			previousDir = dir;
			dir = Direction.Down;
		}

		//todo: else later play "oof doesnt work" sound, or a sound indicative of that
	}

}











