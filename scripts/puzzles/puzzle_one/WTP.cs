using System;
using Godot;
public partial class WTP : Control
{
        [Export]
        private PuzzleStart PUZZLES; //instance of PuzzleStart

        private InteractCircles CIRCLES; //instance of interact circles

        [Export]
        private SpeechGUI DIALOGUE; //Instance of SpeechGUI

        [Export]
        private Node3D F_BOOK; //the family book object

        [Export]
        private Cameras CAMERAS;

        private AnimationPlayer ANIM_PLAYER; //the animation player

        private SceneState SCENESTATEACCESS; //accesses the singleton for the scenestate

        private ButtonOverwrite redBtn; //to store the main puzzle button

        [Export]
        private AudioStreamPlayer aux; //audio player

        [Signal]
        public delegate void FinalDialogueEventHandler();


    public override void _Ready()
    {
        CIRCLES = GetNode<InteractCircles>("../../../../../");
        ANIM_PLAYER = GetNode<AnimationPlayer>("PuzzleCont/Components/c6/AnimationPlayer"); //the animation player
        SCENESTATEACCESS = GetNode<SceneState>("/root/SceneStateSingleton"); //accesses the singleton for the scene state
    }
    //Initialises all the components for the puzzle
    public void InitPuzzle() {
        
        
        SCENESTATEACCESS.TimesStuck = 0;

        //Gets the red circle
        ButtonOverwrite redCirc = GetNode<ButtonOverwrite>("PuzzleCont/2");
        redBtn = redCirc; //sets the red circle

        //Changes the camera angle to the one set in the meta data
        CIRCLES.SetCam(redCirc.GetMeta("NewCamPos").AsVector3(), (Vector3)redCirc.GetMeta("CamRotation"));

        //Toggles the specific components of the puzzle
        PUZZLES.EnableAllCircleComponents("PuzzlesPanel/South/PuzzleCont/Components");

        //Toggles parent nodes
        PUZZLES.TogglePuzzleVisibility(redCirc);

        //Toggle Red Circle
        PUZZLES.ToggleSpecificCircleVisibility(redCirc);

        //Hides the red circles
        CIRCLES.ToggleSpecificDirection(redCirc);
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

    //Plays the show book animation
    private void ShowBookAnimation()
    {
        ANIM_PLAYER.Play("FadeInBook");
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

        //Progresses the Puzzle
        ProgressPuzzle();
    }

    //Handles placing the book and the rest of IntroductionScene
    private async void PlaceBookScene()
    {
        //Triggers the dialogue and awaits the signal to progress
        CIRCLES.CirclesPressed("Select_Items/Settings/Puzzles/PuzzlesPanel/East/PuzzleCont/5");
        await ToSignal(DIALOGUE, "LookProgress");
        
        //Hides the pressed circle circle
        CIRCLES.ToggleSpecificDirectionPath("Select_Items/Settings/Puzzles/PuzzlesPanel/East/PuzzleCont/5");
        //Plays the hide book animation and waits for it to be finished
        ShowBookAnimation();
        await ToSignal(ANIM_PLAYER, "animation_finished");

        //Plays knocking sound
        aux.Play();

        //Waits for audio to finish before emitting signal
        await ToSignal(aux, "finished");
        EmitSignal("FinalDialogue");

    }

    /**
    * ----------------------------------------------------------------
    * Reset Camera for Puzzles
    * ----------------------------------------------------------------
    **/

    private void ProgressPuzzle() {

    CIRCLES.PREVIOUS_POS = Vector3.Zero;
    CIRCLES.PREVIOUS_ANGLE = new Vector3(0, -180, 0);


    CIRCLES.ResetCam(); //resets camera position
    PUZZLES.HideAllCircleComponents("PuzzlesPanel/South/PuzzleCont/Components"); //hides the red circles

    SCENESTATEACCESS.PlayerStatus = SceneState.StatusOfPlayer.FreeRoam; //sets the status of the player to free roam
    SCENESTATEACCESS.sceneState = SceneState.CurrentSceneState.Stage_3;
    SCENESTATEACCESS.CurrentObjective = "Place the book on the desk."; //Sets the new objective

    CIRCLES.EmitEvent(string.Format("Toggle{0}Events", GetViewport().GetCamera3D().Name)); //Triggers the south camera circles
    PUZZLES.TogglePuzzleVisibility(redBtn); //disable the parents visibility
    GetNode<ButtonOverwrite>("../../../Panel/South/2").SetMeta("PuzzleEnabled", false); //disables the puzzle for the middle bookshelf
    GetNode<ButtonOverwrite>("../../../Panel/East/5").SetMeta("PuzzleEnabled", true); //enables placing the book on the desk
    
    }
}
