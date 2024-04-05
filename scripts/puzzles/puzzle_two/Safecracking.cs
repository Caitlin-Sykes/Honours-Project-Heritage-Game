using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Safecracking : Control
{
    private List<int> code = new List<int>() { 1, 9, 6, 9 }; //stores the correct code for the safe.
    private List<int> attempt = new List<int>(); //stores the attempted codes for the safe

    private SceneState SCENESTATEACCESS; //accesses the singleton for the scene state

    private int fullAttempt = 0;

    private PuzzleStart PUZZLES; //instance of PuzzleStart

    [Export]
    private InteractCircles CIRCLES_TWO; //instance of interact circles

    [Export]
    private AudioStreamPlayer beepReject;

    [Export]
    private AudioStreamPlayer beepAccept;

    [Export]
    private Label PinScreen; //for showing the numbers entered

    private ButtonOverwrite redBtn; //to store the main puzzle button

    public Vector3 BACKUP_POS; //for the angle and position in a puzzle
    public Vector3 BACKUP_ANGLE;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        PUZZLES = GetNode<PuzzleStart>("../../");
        SCENESTATEACCESS = GetNode<SceneState>("/root/SceneStateSingleton"); //accesses the singleton for the scene state
    }

    /** 
    * ----------------------------------------------------------------
    *	Puzzle Init Handlers
    * ----------------------------------------------------------------
    **/

    public void InitPuzzle()
    {

        //Gets the red circle
        ButtonOverwrite redCirc = GetNode<ButtonOverwrite>("PuzzleCont/2");
        redBtn = redCirc; //sets the red circle

        //Changes the camera angle to the one set in the meta data
        CIRCLES_TWO.SetCam(redCirc.GetMeta("NewCamPos").AsVector3(), (Vector3)redCirc.GetMeta("CamRotation"));

        GetNode<Control>("PuzzleCont").Visible = false;

        //Toggles the specific components of the puzzle
        PUZZLES.EnableAllCircleComponents("PuzzlesPanel/West/PuzzleCont/Components");
    }

    //Set the very specific position
    private void SetCurrentPos()
    {
        Camera3D curCam = GetViewport().GetCamera3D(); //Gets the current active camera
		
        BACKUP_POS = curCam.Position;
        BACKUP_ANGLE = curCam.RotationDegrees;
    }

    /** 
    * ----------------------------------------------------------------
    *	Button Entry Functions
    * ----------------------------------------------------------------
    **/

    //On num pad press
    private void OnButtonPress(int pin)
    {

        //If the length is < 4 then add to list and screen
        if (attempt.ToList().Count < 4)
        {
            attempt.Add(pin);
            AppendToScreenText(pin);
        }

        //Play reject beep and set screen to this
        else
        {
            beepReject.Play();
            PinScreen.Text = "*Pin = 4*";
            SCENESTATEACCESS.PlayerStatus = SceneState.StatusOfPlayer.InDialogue;

            CIRCLES_TWO.DialogueByString("Safe_Incorrect_TMD");
        }
    }

    // On submit button press
    private void OnSubmitButton()
    {

        if (attempt.Count != 0)
        {
            //If the code and array match exactly (inc. length)
            if (code.SequenceEqual(attempt))
            {
                PinScreen.Text = "*Opening*";
                beepAccept.Play();
                //PuzzleAnswer();
            }
        
            else
            {
                // Displays wrong pin to screen
                PinScreen.Text = "*Wrong Pin*";

                SCENESTATEACCESS.PlayerStatus = SceneState.StatusOfPlayer.InDialogue;
                
                // Plays dialogue for wrong pin
                CIRCLES_TWO.DialogueByString("Safe_Incorrect_WrongCode");
            
                // Plays reject noise
                beepReject.Play();

                //Clears the submitted array
                attempt = new List<int>();
            }

        }
    }

    //A function that when called, clears the numbers entered
    private void ClearNumbers()
    {
        //show dialogue to clear attempts
        attempt = new List<int>();
        ResetScreen();
    }

    // A function that when called will delete the latest number entered
    private void DeleteLatest()
    {
        // Creates an array one shorted the the current as long as it has >1 num
        if (attempt.Count != 1)
        {

            //Gets the last number entered
            int numRemove = attempt.Last();

            //Reverses the attempts so the last number is the first    
            attempt.Reverse();

            //Removes the last entered number (remove removes the first instance only)
            attempt.Remove(numRemove);

            //Puts it the right way around again
            attempt.Reverse();

            // Updates the screen
            UpdateScreenText();
        }

        else
        {
            ClearNumbers();
        }
    }

    /** 
    * ----------------------------------------------------------------
    *	Screen Display Functions
    * ----------------------------------------------------------------
    **/

    //A function that will append the screen to show what numbers are entered
    private void AppendToScreenText(int pin)
    {

        //If the number of attempts is != 1 then append
        if (attempt.Count != 1) {
            string currentText = PinScreen.Text;
            currentText += pin;
            PinScreen.Text = currentText;
        }

        // Otherwise reset PinScreen and just add the num
        else {
            PinScreen.Text = pin.ToString();
        }
       
    }

    //A function that will update the screen to remove the deleted number
    private void UpdateScreenText()
    {
        PinScreen.Text = "";
        foreach (int pin in attempt) {
            PinScreen.Text += string.Format("{0}", pin.ToString());
        }
    }

    //A function that will reset the screen to the default
    private void ResetScreen()
    {
        PinScreen.Text = "*Enter Pin*";
    }


    /** 
    * ----------------------------------------------------------------
    *	Button Entry Functions
    * ----------------------------------------------------------------
    **/

    // A function that executes what happens after the puzzle is solved
    private void PuzzleAnswer() {
        
    }


}
