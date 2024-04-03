using Godot;
using System;

public partial class Controls : Node3D
{

    [Export]
    private Cameras CAMERAS; // Instance of cameras script

    private Transitions TRANSITION; //Handles screen transitions

    [Export]
    private InteractCircles CIRCLES; //Instance of InteractCircles

    [Export]
    private SpeechGUI DIALOGUE; //Instance of SpeechGUI

    private SceneState SCENESTATEACCESS; //accesses the singleton for the scenestate
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		SCENESTATEACCESS = GetNode<SceneState>("/root/SceneStateSingleton"); //accesses the singleton for the scene state

        TRANSITION = GetNode<Transitions>("../../Transition");
	}

    //Handles inputs
    public override void _Input(InputEvent @event)
    {
        //Checks for key presses
		if (Input.IsKeyPressed(Key.A) && @event.IsPressed() && SCENESTATEACCESS.PlayerStatus == SceneState.StatusOfPlayer.FreeRoam)
        {
            OnLeftArrow();
        }

		else if (Input.IsKeyPressed(Key.D) && @event.IsPressed() && SCENESTATEACCESS.PlayerStatus == SceneState.StatusOfPlayer.FreeRoam)
        {
            OnRightArrow();
        }

        else if (Input.IsKeyPressed(Key.W) && @event.IsPressed() && CAMERAS.GetMeta("UpDownEnabled").AsBool() && SCENESTATEACCESS.PlayerStatus == SceneState.StatusOfPlayer.FreeRoam)
        {
            OnUpArrow();
        }

        else if (Input.IsKeyPressed(Key.S) && @event.IsPressed() && CAMERAS.GetMeta("UpDownEnabled").AsBool() && SCENESTATEACCESS.PlayerStatus == SceneState.StatusOfPlayer.FreeRoam)
        {
           OnDownArrow();
        }

        //Shows extra info about an object
        else if (Input.IsKeyPressed(Key.J) && @event.IsPressed() && SCENESTATEACCESS.PlayerStatus == SceneState.StatusOfPlayer.LookingAtSomething)
        {
            CIRCLES.ShowExtraInformation();
        }

        //Shows sources if looking at something
        else if (Input.IsKeyPressed(Key.K) && @event.IsPressed() && SCENESTATEACCESS.PlayerStatus == SceneState.StatusOfPlayer.LookingAtSomething)
        {
            CIRCLES.ShowSources();
        }
        
        else if (Input.IsKeyPressed(Key.K) && @event.IsPressed() && SCENESTATEACCESS.PlayerStatus == SceneState.StatusOfPlayer.InDialogue)
        {
            CIRCLES.ShowDialogueSources();
        }

        //Reminds player what to do
         else if (Input.IsKeyPressed(Key.L) && @event.IsPressed())
         { 
             DIALOGUE.CalledOutwidthDialogue = true; 
             DIALOGUE.ShowObjective();
        }

    }

    /** 
    *   ----------------------------------------------------------------
	*	Start of "W" and Up Gui Buttons
	*	----------------------------------------------------------------
	**/

    // A handler to control clicking on the up gui arrow
    private void OnUpArrow(InputEvent @evnt) {

        //If trigger is left click
        if (@evnt is InputEventMouseButton mouse && SCENESTATEACCESS.PlayerStatus == SceneState.StatusOfPlayer.FreeRoam && mouse.ButtonIndex == MouseButton.Left && @evnt.IsPressed()) {
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
    private void OnRightArrow(InputEvent @evnt) {

        //If trigger is left click
        if (@evnt is InputEventMouseButton mouse && SCENESTATEACCESS.PlayerStatus == SceneState.StatusOfPlayer.FreeRoam && mouse.ButtonIndex == MouseButton.Left && @evnt.IsPressed()) {
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
    private void OnDownArrow(InputEvent @evnt) {

        //If trigger is left click
        if (@evnt is InputEventMouseButton mouse && SCENESTATEACCESS.PlayerStatus == SceneState.StatusOfPlayer.FreeRoam && (mouse.ButtonIndex == MouseButton.Left && @evnt.IsPressed())) {
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
    private void OnLeftArrow(InputEvent @evnt) {

        //If trigger is left click
        if (@evnt is InputEventMouseButton mouse && SCENESTATEACCESS.PlayerStatus == SceneState.StatusOfPlayer.FreeRoam && mouse.ButtonIndex == MouseButton.Left && @evnt.IsPressed()) {
                    OnLeftArrow();
        }
    }

    
    // A function to handle left arrow movement, prompted by the A key
    private void OnLeftArrow() {
        CAMERAS.TurnLeft();
    }
}

