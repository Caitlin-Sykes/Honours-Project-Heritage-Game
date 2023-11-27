using Godot;
using System;

public partial class IntroductionScene : Node3D
{

	[Signal]
	public delegate void StartSceneEventHandler();

	[Export]
	private SpeechGUI dialogue; //instance of speechGUI

	private String MOTHER_AVATAR = "res://resources/textures/sprites/main_char/12.svg"; //todo: replace

	private String PLAYER_AVATAR = "res://resources/textures/sprites/main_char/" + PlayerData.Player.ID + ".svg"; //players default avatar

	// private PlayerData 

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		EmitSignal(SignalName.StartScene);
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	//A function to handle the first dialogue
	public async void FirstDialogue() {

		await ToSignal(GetTree().CreateTimer(3), SceneTreeTimer.SignalName.Timeout); //timer so it waits out the animations
		dialogue.ToggleGUIVisible(); //toggles the gui visible

		//Sets up first bit of dialogue
		dialogue.SetNameNode("Mum");
		dialogue.SetSpeechNode(string.Format("{0}, Wake up! It's getting late and we have stuff to do!", PlayerData.Player.Name));
		dialogue.SetAvatarNode(MOTHER_AVATAR);
	}
}
