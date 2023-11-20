using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class Transitions : ColorRect
{
	[Export]
	public static  AnimationPlayer ANIMATION_PLAYER;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ANIMATION_PLAYER = (AnimationPlayer)GetNode("AnimationPlayer");
		ANIMATION_PLAYER.Play("Transition_Fade");
	}

	/**
	*	A function to change to the next scene
	* @param - path to the scene
	**/
	public async void nextScene(String path) {
		ANIMATION_PLAYER.PlayBackwards("Transition_Fade");
		await ToSignal(ANIMATION_PLAYER, "animation_finished");
		GetTree().ChangeSceneToFile(path);
	}
}
