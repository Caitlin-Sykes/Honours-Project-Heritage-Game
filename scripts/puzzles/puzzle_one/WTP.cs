using Godot;
using System;

public partial class WTP : Control
{
        private PuzzleStart PUZZLES;

        private InteractCircles CIRCLES;



    public override void _Ready()
    {
        PUZZLES = GetNode<PuzzleStart>("../../");
        CIRCLES = GetNode<InteractCircles>("../../../../../");

    }
    //Initialises all the components for the puzzle
    public void InitPuzzle() {
        
        //TODO: redcirc is confirmed ButtonOverwrite. Not control. Remove later
        //Gets the red circle
        ButtonOverwrite redCirc = GetNode<ButtonOverwrite>("PuzzleCont/2");

        //Changes the camera angle to the one set in the meta data
        CIRCLES.SetCam(redCirc.GetMeta("NewCamPos").AsVector3(), (float)redCirc.GetMeta("CamRotation"));

        //Toggles the specific components of the puzzle
        EnableAllCircleComponents(redCirc);

        //Toggles parent nodes
        PUZZLES.TogglePuzzleVisibility(redCirc);

        //Hides the red circles
        CIRCLES.ToggleSpecificDirection(redCirc);

        // ToggleEventsDirection(GetNode<ButtonOverwrite>(path)); //turns off all the circles 
        // SetCam(GetNode<ButtonOverwrite>(path).GetMeta("NewCamPos").AsVector3(), (float)GetNode<ButtonOverwrite>(path).GetMeta("CamRotation"));
    }

    /**
    * ----------------------------------------------------------------
    * Handlers for the puzzle components
    * ----------------------------------------------------------------
    **/

    //Handlers all the clickable components
    private void EnableAllCircleComponents(ButtonOverwrite redCirc) {
        
        Control components = GetNode<Control>("PuzzleCont/Components");

        //Toggles visibility of the parent node
        components.Visible = !components.Visible;
        //For every circle in the components container, toggles them
        foreach (ButtonOverwrite circle in components.GetChildren())
        {
            circle.Visible = !circle.Visible;
        }
    }




}
