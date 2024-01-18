using Godot;
using System;

public partial class ButtonOverwrite : BaseButton
{
	private AudioStreamPlayer2D aux; //for handling on click audio
	public override void _Ready()
	{
		//following line is purely because of Godot messing up the bounding buttons
		if (this.Name != "Continue") {
			this.SetSize(new Vector2(200, 200), true);
		}
		aux = GetNode<AudioStreamPlayer2D>("ClickButton");
	}

	//Plays the audio click button then continues normal button behaviour
    public override void _Pressed()
    {
		aux.Play();
        base._Pressed();
		
		
    }
}