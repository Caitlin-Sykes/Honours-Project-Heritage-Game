using Godot;
using System;


public partial class Cameras : Node
{

	public enum Direction
	{
		North,
		East,
		South,
		West
	} //enum for directions

	[Export]
	private Godot.Collections.Array<Camera3D> CAMERAS { get; set; }

	public Direction dir; // instance of direction

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		dir = Direction.North; // starts off north by default but can be overriden by calling SetDefaultDirection
		SetArrowsVisible(CAMERAS[0]); //turns on North's GUI arrows
	}


	/** 
	* ----------------------------------------------------------------
	*	Misc Handlers
	* ----------------------------------------------------------------
	**/

	//Will overwrite default direction if called
	public void SetDefaultDirection(String direction)
	{
		dir = ToDirection(direction);
	}

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
			if ((arrow.Name == "Up_Arrow_Parent" || arrow.Name == "Down_Arrow_Parent") && GetMeta("UpDownEnabled").AsBool() == false) {
				break;
			}

			//Else sets visible
			else {
				GD.Print(arrow.Name);
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
				CAMERAS[3].Current = true;
				break;

			//If current camera is east, set current camera to north
			case Direction.East:
				dir = Direction.North;
				SetArrowsInvisible(CAMERAS[1]);
				SetArrowsVisible(CAMERAS[0]);
				CAMERAS[0].Current = true;
				break;

			//If current camera is south, set current camera to East
			case Cameras.Direction.South:
				dir = Direction.East;
				SetArrowsInvisible(CAMERAS[2]);
				SetArrowsVisible(CAMERAS[1]);
				CAMERAS[1].Current = true;
				break;

			//If current camera is west, set current camera to south
			case Cameras.Direction.West:
				dir = Direction.South;
				SetArrowsInvisible(CAMERAS[3]);
				SetArrowsVisible(CAMERAS[2]);
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
				CAMERAS[1].Current = true;
				break;

			//If current camera is east, set current camera to north
			case Direction.East:
				dir = Direction.South;
				SetArrowsInvisible(CAMERAS[1]);
				SetArrowsVisible(CAMERAS[2]);
				CAMERAS[2].Current = true;
				break;

			//If current camera is south, set current camera to East
			case Cameras.Direction.South:
				dir = Direction.West;
				SetArrowsInvisible(CAMERAS[2]);
				SetArrowsVisible(CAMERAS[3]);
				CAMERAS[3].Current = true;
				break;

			//If current camera is west, set current camera to south
			case Cameras.Direction.West:
				dir = Direction.North;
				SetArrowsInvisible(CAMERAS[3]);
				SetArrowsVisible(CAMERAS[0]);
				CAMERAS[0].Current = true;
				break;

			default:
				throw new ArgumentException("Not a valid direction");
		}

	}

	//TODO: Method to handle looking up (for scenes that are allowed)
	internal void TurnUp()
	{
		throw new NotImplementedException();
	}

	//TODO: Method to handle looking down
	internal void TurnDown()
	{
		throw new NotImplementedException();
	}

}

//TODO: implement camera switch
// will need to disable previous cam
// will need to enable current cam
// will need to swap to correct camera

// either on arrow click or on d press
// use an enum to get previous direction and change depending on that?
// e.g
/**
*	public void OnRightButtonClick() {
		if enum currentDir == North {
			then go to camera east
			set CurrentDir to East
		}
		else if currentDir == east {
			then go to camera south
			set CurrentDir to South

		}

		etc
}

//
// little bit repetive but all right buttons ccan use rightbuttonclick
*
*
**/
//TODO: Directions for Camera angles
// North, Right arrow -> East
// North, left arrow -> west
// North, down arrow -> floor -- these wont ever change
// North, up arrow -> ceiling -- these wont ever change

// East, Right arrow -> South
// East, left arrow -> North
// East, down arrow -> floor -- these wont ever change
// East, up arrow -> ceiling -- these wont ever change

// South, Right arrow -> West
// South, left arrow -> East
// South, down arrow -> floor -- these wont ever change
// South, up arrow -> ceiling -- these wont ever change

// West, Right arrow -> North
// West, left arrow -> South
// West, down arrow -> floor -- these wont ever change
// West, up arrow -> ceiling -- these wont ever change