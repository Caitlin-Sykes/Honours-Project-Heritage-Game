using Godot;
using System;

public partial class Controls : Node3D
{

    private Cameras CAMERAS; // Instance of cameras script
	
    // Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        CAMERAS = GetNode<Cameras>("Cameras");
	}
		
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		//Checks for key presses
		if (Input.IsKeyPressed(Key.W))
        {
            GD.Print("W key pressed!");
        }

		else if (Input.IsKeyPressed(Key.A))
        {
            OnLeftArrow();
        }
        else if (Input.IsKeyPressed(Key.S))
        {
            GD.Print("S key pressed!");
        }

		else if (Input.IsKeyPressed(Key.D))
        {
            GD.Print("D key pressed!");
        }
	}

    /** ----------------------------------------------------------------
	*	Start of "A" and Left Gui Buttons
	*	----------------------------------------------------------------
	**/   

    // A handler to control clicking on the left gui arrow
    private void OnLeftArrow(Node camera, InputEvent @evnt, Vector3 position, Vector3 normal, int shape_idx) {

        //If trigger is left click
        if (@evnt is InputEventMouseButton mouse) {
            if (mouse.ButtonIndex == MouseButton.Left) {
                    OnLeftArrow();
                }
    }}

    
    // A function to handle left arrow movement, prompted by the A key
    private void OnLeftArrow() {

        // Cameras/Controls/UI/North
        //Changes camera depending on what the current direction is
        switch(CAMERAS.dir) {
            case Cameras.Direction.North:


                //Sets current camera to West
                CAMERAS.dir = Cameras.Direction.West;
                break;
            case Cameras.Direction.East:
                break;
            case Cameras.Direction.South:
                break;
            case Cameras.Direction.West:
                break;
            default:
                throw new ArgumentException("Not a valid direction");
        }
    }
}

