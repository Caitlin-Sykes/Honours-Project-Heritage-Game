using Godot;
using System;
using System.Linq;

public partial class Safecracking : Node
{
    private int[] code = new int[4]  {1,9,6,9}; //stores the correct code for the safe.
    private int[] attempt = new int [4]; //stores the attempted codes for the safe

    private int fullAttempt = 0;

    [Export]
    private AudioStreamPlayer beepReject;

    [Export]
    private AudioStreamPlayer beepGeneric;

// Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    private void OnButtonPress(int pin) { 
        
        
        //If the length
        if (code.Length !> 4) {
            code.Append<int>(pin).ToArray();
            beepGeneric.Play();
        }

        else {
            GD.Print("**********Eventually Play a Pissed Off Beeping Noise****************");
            GD.Print("text to say pin be four.");
        }
    }

    private void OnSubmitButton() {
        //If the code and array match exactly (inc. length)
        if (code.SequenceEqual(attempt)) {
            GD.Print("***Happy Beeps***");
            //opn safe//progress
        }

        else {
            GD.Print("*** sad beeps ***");
            //Clears the submitted array
            attempt = new int [4];
        }

    }

    //A function that when called, clears the numbers entered
    private void ClearNumbers() {
        //show dialogue to clear attempts
        attempt = new int[4];
    }
}
