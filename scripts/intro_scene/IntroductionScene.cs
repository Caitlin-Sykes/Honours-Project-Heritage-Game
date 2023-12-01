using Godot;
using System;

public partial class IntroductionScene : Node3D
{

	[Signal]
	public delegate void StartSceneEventHandler();

	[Export]
	private SpeechGUI dialogue; //instance of speechGUI

	private String MOTHER_AVATAR = "res://resources/textures/sprites/main_char/12.svg"; //todo: replace

	private String PLAYER_AVATAR = "res://resources/textures/sprites/main_char/" + PlayerData.Player.Avatar + ".svg"; //players default avatar

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		EmitSignal(SignalName.StartScene);
	}

	//A function to handle the first dialogue
	public async void FirstDialogue() {

		await ToSignal(GetTree().CreateTimer(3), SceneTreeTimer.SignalName.Timeout); //timer so it waits out the animations
		dialogue.ToggleGUIVisible(); //toggles the gui visible

		//Sets up first bit of dialogue
		// dialogue.SetNameNode("Mum");
		// dialogue.SetSpeechNode(string.Format("{0}, Wake up! It's getting late and we have stuff to do!", PlayerData.Player.Name));
		// dialogue.SetAvatarNode(MOTHER_AVATAR);

        await ToSignal(dialogue, "DialogueProgress");


		// dialogue.SetNameNode(PlayerData.Player.Name);
		// dialogue.SetSpeechNode("Nghhhhhh...");
		// dialogue.SetAvatarNode(PLAYER_AVATAR);

		await ToSignal(dialogue, "DialogueProgress");

		// dialogue.SetNameNode("Mum");
		// dialogue.SetSpeechNode(string.Format("I mean it, {0}! Don't make me come in with the bucket of water!", PlayerData.Player.Name));
		// dialogue.SetAvatarNode(MOTHER_AVATAR);

		await ToSignal(dialogue, "DialogueProgress");

		// dialogue.SetNameNode(PlayerData.Player.Name);
		// dialogue.SetSpeechNode("I suppose I better get up..");
		// dialogue.SetAvatarNode(PLAYER_AVATAR);

		await ToSignal(dialogue, "DialogueProgress");

		dialogue.ToggleGUIVisible();

		// TODO: fix json parsing

	}
}
