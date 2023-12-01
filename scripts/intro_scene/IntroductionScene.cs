using Godot;
using System;

public partial class IntroductionScene : Node3D
{

	[Signal]
	public delegate void StartSceneEventHandler();

	[Export]
	private SpeechGUI dialogue; //instance of speechGUI

	private String MOTHER_AVATAR = "res://resources/textures/sprites/mother/" + PlayerData.Player.Avatar + ".svg";

	private String PLAYER_AVATAR = "res://resources/textures/sprites/main_char/" + PlayerData.Player.Avatar + ".svg"; //players avatar default avatar

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		EmitSignal(SignalName.StartScene);
	}

	//A function to handle the first dialogue
	public async void FirstDialogue() {

		await ToSignal(GetTree().CreateTimer(3), SceneTreeTimer.SignalName.Timeout); //timer so it waits out the animations
		dialogue.ToggleGUIVisible(); //toggles the gui visible


		//If not null then iterate through the dialogue
		if (JsonHandler.Speech.IntroductionScene != null) {

			foreach (var Speech in JsonHandler.Speech.IntroductionScene)
			{

				dialogue.SetNameNode(string.Format(Speech.Speaker, PlayerData.Player.Name));
				dialogue.SetSpeechNode(string.Format(Speech.Dialogue, PlayerData.Player.Name));
				dialogue.SetAvatarNode(string.Format(Speech.Avatar, PlayerData.Player.Avatar));
				await ToSignal(dialogue, "DialogueProgress");
			}
		}

		else {
			throw new InvalidOperationException("Error: something has gone wrong with parsing the dialogue.json");
		}
		

		dialogue.ToggleGUIVisible();
	}
}
