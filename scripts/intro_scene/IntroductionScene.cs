using Godot;
using System;

public partial class IntroductionScene : Node3D
{

	[Signal]
	public delegate void StartSceneEventHandler();

	[Export]
	private SpeechGUI dialogue; //instance of speechGUI

	private BaseButton getUpButton; // a button used solely for getting up from the bed

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		getUpButton = GetNode<BaseButton>("CanvasLayer/Settings/OnItemSelect");
		EmitSignal(SignalName.StartScene);
	}

	//A function to handle the first dialogue
	public async void FirstDialogue() {

		await ToSignal(GetTree().CreateTimer(3), SceneTreeTimer.SignalName.Timeout); //timer so it waits out the animations
		dialogue.ToggleGUIVisible(); //toggles the gui visible


		//If not null then iterate through the dialogue
		if (JsonHandler.Speech.IntroductionScene != null) {

			//For every bit of speech in the scene
			foreach (var Speech in JsonHandler.Speech.IntroductionScene)
			{

				dialogue.SetNameNode(string.Format(Speech.Speaker, PlayerData.Player.Name));
				dialogue.SetSpeechNode(string.Format(Speech.Dialogue, PlayerData.Player.Name));
				dialogue.SetAvatarNode(string.Format(Speech.Avatar, PlayerData.Player.Avatar));

				if (Speech.Id == "5") {
					getUpButton.Visible = true;
				}

				await ToSignal(dialogue, "DialogueProgress");

			}
		}

		else {
			throw new InvalidOperationException("Error: something has gone wrong with parsing the dialogue.json");
		}

		

	}

	// The 2D selecting icon, different icon for 3D scenes
	// in this scene, handles getting up from the bed
	private void OnItemSelect()
	{
		
	}
}
