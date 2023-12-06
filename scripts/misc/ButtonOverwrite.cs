using Godot;
using System;

public partial class ButtonOverwrite : BaseButton
{
	public AudioStreamPlayer2D aux; //for handling on click audio
	public override void _Ready()
	{
		aux = GetNode<AudioStreamPlayer2D>("ClickButton");
	}

	//Plays the audio click button then continues normal button behaviour
    public override void _Pressed()
    {
		aux.Play();
        base._Pressed();
		
		
    }
}