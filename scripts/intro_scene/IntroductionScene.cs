using Godot;
using System;

public partial class IntroductionScene : Node3D
{

	private Transitions TRANSITION; //Handles screen transitions

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		TRANSITION = GetNode<Transitions>("Transition");


	}
	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
