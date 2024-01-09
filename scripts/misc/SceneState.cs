using Godot;
using System;

public partial class SceneState : Node
{
	// Called when the node enters the scene tree for the first time.
	public enum CurrentSceneState
	{
		Tutorial, //tutorial scene, bunch of misc data
		Stage_1, // stage 2
		Stage_2, //stage 3
	}

	public enum StatusOfPlayer{
		InDialogue, //means player is in dialogue
		LookingAtSomething, //means player is looking at something
		FreeRoam, //player is moving about
	}

	//Converts CurrentSceneState to String
	public static string CurrentStateAsString() {
		switch(sceneState) {
			case CurrentSceneState.Tutorial:
				return "Tutorial";
			case CurrentSceneState.Stage_1:
				return "Stage 1";
			case CurrentSceneState.Stage_2:
				return "Stage 2";
			default:
				return "Unknown";
		}
	}

	public static CurrentSceneState sceneState = CurrentSceneState.Tutorial; //current scene state
	public static StatusOfPlayer PlayerStatus = StatusOfPlayer.FreeRoam; //current scene state

}
