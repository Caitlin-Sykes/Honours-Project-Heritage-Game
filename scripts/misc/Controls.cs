using Godot;
using System;

public partial class Controls : Node3D
{

    private Cameras CAMERAS; // Instance of cameras script

    private Transitions TRANSITION; //Handles screen transitions

    [Export]
    private InteractCircles CIRCLES; //Instance of SpeechGUI


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        TRANSITION = GetNode<Transitions>("../../Transition");

        CAMERAS = GetNode<Cameras>("../../Cameras");
	}

    //Handles inputs
    public override void _Input(InputEvent @event)
    {
        GD.Print("Player Status: " + SceneState.PlayerStatus);
        //Checks for key presses
		if (Input.IsKeyPressed(Key.A) && SceneState.PlayerStatus == SceneState.StatusOfPlayer.FreeRoam)
        {
            OnLeftArrow();
        }

		else if (Input.IsKeyPressed(Key.D) && SceneState.PlayerStatus == SceneState.StatusOfPlayer.FreeRoam)
        {
            OnRightArrow();
        }

        else if (Input.IsKeyPressed(Key.W) && CAMERAS.GetMeta("UpDownEnabled").AsBool() && SceneState.PlayerStatus == SceneState.StatusOfPlayer.FreeRoam)
        {
            OnUpArrow();
        }

        else if (Input.IsKeyPressed(Key.S) && CAMERAS.GetMeta("UpDownEnabled").AsBool() && SceneState.PlayerStatus == SceneState.StatusOfPlayer.FreeRoam)
        {
           OnDownArrow();
        }

        //Shows extra info about an object
        else if (Input.IsKeyPressed(Key.J) && SceneState.PlayerStatus == SceneState.StatusOfPlayer.LookingAtSomething)
        {
            CIRCLES.ShowExtraInformation();
        }

        //Shows sources
        else if (Input.IsKeyPressed(Key.K) && SceneState.PlayerStatus == SceneState.StatusOfPlayer.LookingAtSomething)
        {
            CIRCLES.ShowSources();
        }

        //Reminds player what to do
         else if (Input.IsKeyPressed(Key.L) && SceneState.PlayerStatus == SceneState.StatusOfPlayer.LookingAtSomething)
        {
            //Im stuck func
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
        if (@evnt is InputEventMouseButton mouse && SceneState.PlayerStatus == SceneState.StatusOfPlayer.FreeRoam && mouse.ButtonIndex == MouseButton.Left && @evnt.IsPressed()) {
                    OnUpArrow();
                }
    }

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
        if (@evnt is InputEventMouseButton mouse && SceneState.PlayerStatus == SceneState.StatusOfPlayer.FreeRoam && (mouse.ButtonIndex == MouseButton.Left && @evnt.IsPressed())) {
                    OnRightArrow();
                }
    }

    
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
        if (@evnt is InputEventMouseButton mouse && SceneState.PlayerStatus == SceneState.StatusOfPlayer.FreeRoam && (mouse.ButtonIndex == MouseButton.Left && @evnt.IsPressed())) {
                    OnDownArrow();
                }
    }

    
    // A function to handle down arrow movement, prompted by the S key
    private void OnDownArrow() {
        CAMERAS.TurnDown();
    }

    /** 
    *   ----------------------------------------------------------------
	*	Start of "A" and Left Gui Buttons
	*	----------------------------------------------------------------
	**/   

    // A handler to control clicking on the left gui arrow
    private void OnLeftArrow(Node camera, InputEvent @evnt, Vector3 position, Vector3 normal, int shape_idx) {

        //If trigger is left click
        if (@evnt is InputEventMouseButton mouse && SceneState.PlayerStatus == SceneState.StatusOfPlayer.FreeRoam && mouse.ButtonIndex == MouseButton.Left && @evnt.IsPressed()) {
                    OnLeftArrow();
                }
    }

    
    // A function to handle left arrow movement, prompted by the A key
    private void OnLeftArrow() {
        CAMERAS.TurnLeft();
    }
}

