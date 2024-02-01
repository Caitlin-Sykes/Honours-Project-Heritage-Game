using Godot;
public partial class WTP : Control
{
        private PuzzleStart PUZZLES; //instance of PuzzleStart

        private InteractCircles CIRCLES; //instance of interact circles

        [Export]
        private SpeechGUI DIALOGUE; //Instance of SpeechGUI

        [Export]
        private Node3D F_BOOK; //the family book object

        [Export]
        private Cameras CAMERAS;

        private AnimationPlayer ANIM_PLAYER; //the animation player

    public override void _Ready()
    {
        PUZZLES = GetNode<PuzzleStart>("../../");
        CIRCLES = GetNode<InteractCircles>("../../../../../");
        ANIM_PLAYER = GetNode<AnimationPlayer>("PuzzleCont/Components/c6/AnimationPlayer"); //the animation player
    }
//Initialises all the components for the puzzle
public void InitPuzzle() {
        
        //Gets the red circle
        ButtonOverwrite redCirc = GetNode<ButtonOverwrite>("PuzzleCont/2");

        //Changes the camera angle to the one set in the meta data
        CIRCLES.SetCam(redCirc.GetMeta("NewCamPos").AsVector3(), (float)redCirc.GetMeta("CamRotation"));

        //Toggles the specific components of the puzzle
        EnableAllCircleComponents();

        //Toggles parent nodes
        PUZZLES.TogglePuzzleVisibility(redCirc);

        //Toggle Red Circle
        PUZZLES.ToggleSpecificCircleVisibility(redCirc);

        //Hides the red circles
        CIRCLES.ToggleSpecificDirection(redCirc);
    }

    /**
    * ----------------------------------------------------------------
    * Handlers for the puzzle components
    * ----------------------------------------------------------------
    **/

    //Handlers all the clickable components
    private void EnableAllCircleComponents() {
        
        Control components = GetNode<Control>("PuzzleCont/Components");

        //Toggles visibility of the parent node
        components.Visible = !components.Visible;
        //For every circle in the components container, toggles them
        foreach (ButtonOverwrite circle in components.GetChildren())
        {
            circle.Visible = !circle.Visible;
        }
    }

    /**
    * ----------------------------------------------------------------
    * Moving the Books
    * ----------------------------------------------------------------
    **/

    //Plays the hide book animation
    private void HideBookAnimation() {
        ANIM_PLAYER.Play("ShowBook");
    }
    //Toggles visibility of the family book
    private void ToggleFamilyBook() {
        F_BOOK.Visible = !F_BOOK.Visible;
    }

    /**
    * ----------------------------------------------------------------
    * Specifically on Puzzle Access
    * ----------------------------------------------------------------
    **/

    private async void PuzzleAnswer() {
        CIRCLES.ToggleBackButton();
        //Triggers the dialogue and awaits the signal to progress
        CIRCLES.CirclesPressed("Select_Items/Settings/Puzzles/PuzzlesPanel/South/PuzzleCont/Components/c6");
        await ToSignal(DIALOGUE, "LookProgress");
        //Hides the c6 circle
        CIRCLES.ToggleSpecificDirectionPath("Select_Items/Settings/Puzzles/PuzzlesPanel/South/PuzzleCont/Components/c6");
        //Plays the hide book animation and waits for it to be finished
        HideBookAnimation();
        await ToSignal(ANIM_PLAYER, "animation_finished");
        //Hides the book
        ToggleFamilyBook();
        // CAMERAS.SetCam


    }

    }
