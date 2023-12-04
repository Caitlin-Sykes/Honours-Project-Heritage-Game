using Godot;
using System;

public partial class Controls : Node3D
{

    private Cameras CAMERAS; // Instance of cameras script

    private Transitions TRANSITION; //Handles screen transitions

	
    // Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        TRANSITION = GetNode<Transitions>("../../Transition");

        CAMERAS = GetNode<Cameras>("../../Cameras");
	}
		
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		
	}

    //Handles inputs
    public override void _Input(InputEvent @event)
    {
        //Checks for key presses
		if (Input.IsKeyPressed(Key.A))
        {
            OnLeftArrow();
        }

		else if (Input.IsKeyPressed(Key.D))
        {
            OnRightArrow();
        }

        else if (Input.IsKeyPressed(Key.W) && CAMERAS.GetMeta("UpDownEnabled").AsBool() == true)
        {
            OnUpArrow();
        }

        else if (Input.IsKeyPressed(Key.S) && CAMERAS.GetMeta("UpDownEnabled").AsBool() == true)
        {
           OnDownArrow();
        }

    }

    /** 
    *   ----------------------------------------------------------------
	*	Start of "W" and Up Gui Buttons
	*	----------------------------------------------------------------
	**/

    // A handler to control clicking on the up gui arrow

    private void OnUpArrow(Node camera, InputEvent @evnt, Vector3 position, Vector3 normal, int shape_idx) {

        //If trigger is left click
        if (@evnt is InputEventMouseButton mouse) {
            if (mouse.ButtonIndex == MouseButton.Left && @evnt.IsPressed()) {
                    OnUpArrow();
                }
    }}

    
    // A function to handle Up arrow movement, prompted by the W key
    private void OnUpArrow() {
        CAMERAS.TurnUp();
    }

    /** 
    *   ----------------------------------------------------------------
	*	Start of "D" and Right Gui Buttons
	*	----------------------------------------------------------------
	**/   

    // A handler to control clicking on the left gui arrow
    private void OnRightArrow(Node camera, InputEvent @evnt, Vector3 position, Vector3 normal, int shape_idx) {

        //If trigger is left click
        if (@evnt is InputEventMouseButton mouse) {
            if (mouse.ButtonIndex == MouseButton.Left && @evnt.IsPressed()) {
                    OnRightArrow();
                }
    }}

    
    // A function to handle right arrow movement, prompted by the D key
    private void OnRightArrow() {
        CAMERAS.TurnRight();
    }

    /** ----------------------------------------------------------------
	*	Start of "S" and Down Gui Buttons
	*	----------------------------------------------------------------
	**/   

    // A handler to control clicking on the bottom gui arrow
    private void OnDownArrow(Node camera, InputEvent @evnt, Vector3 position, Vector3 normal, int shape_idx) {

        //If trigger is left click
        if (@evnt is InputEventMouseButton mouse) {
            if (mouse.ButtonIndex == MouseButton.Left && @evnt.IsPressed()) {
                    OnDownArrow();
                }
    }}

    
    // A function to handle down arrow movement, prompted by the S key
    private void OnDownArrow() {
        CAMERAS.TurnDown();
    }

    /** ----------------------------------------------------------------
	*	Start of "A" and Left Gui Buttons
	*	----------------------------------------------------------------
	**/   

    // A handler to control clicking on the left gui arrow
    private void OnLeftArrow(Node camera, InputEvent @evnt, Vector3 position, Vector3 normal, int shape_idx) {

        //If trigger is left click
        if (@evnt is InputEventMouseButton mouse) {
            if (mouse.ButtonIndex == MouseButton.Left && @evnt.IsPressed()) {
                    OnLeftArrow();
                }
    }}

    
    // A function to handle left arrow movement, prompted by the A key
    private void OnLeftArrow() {
        CAMERAS.TurnLeft();
    }
}

