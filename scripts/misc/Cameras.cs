using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Cameras : Node
{

	public enum Direction {
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
	}


	/** 
	* ----------------------------------------------------------------
	*	Misc Handlers
	* ----------------------------------------------------------------
	**/   

	//Will overwrite default direction if called
	public void SetDefaultDirection(String direction) {
		dir = ToDirection(direction);
	}

	//A function to convert string to type Direction
	private Direction ToDirection(string dir) {
		switch(dir) {
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

	/** 
	* ----------------------------------------------------------------
	*	Handles moving the cameras
	* ----------------------------------------------------------------
	**/
	//TODO: much later- but when selecting something, show a circle which slowly gets smaller.
	//BUG: cameras in top constructor are not defined. Actual function is being pinged correctly. Check how it is getting the camera.
	//Method to handle turning left
	public void TurnLeft() {
		// Cameras/Controls/UI/North
        GD.Print("Should be turning left");
		//Changes camera depending on what the current direction is
        switch(dir) {
			//If current camera is north, set current camera to west
            case Direction.North:
                dir = Direction.West;
				CAMERAS[3].Current = true;
                break;

			//If current camera is east, set current camera to north
            case Direction.East:
				dir = Direction.North;
				CAMERAS[0].Current = true;
                break;
			
			//If current camera is south, set current camera to East
            case Cameras.Direction.South:
				dir = Direction.East;
				CAMERAS[1].Current = true;
                break;

			//If current camera is west, set current camera to south
            case Cameras.Direction.West:
				dir = Direction.South;
				CAMERAS[2].Current = true;
                break;
            
			default:
                throw new ArgumentException("Not a valid direction");
        }

	}

	//TODO: Method to handle looking right
    internal void TurnRight()
    {
        throw new NotImplementedException();
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