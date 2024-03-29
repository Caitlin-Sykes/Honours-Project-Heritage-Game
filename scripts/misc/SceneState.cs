using Godot;
using System;
public partial class SceneState : Node {

	/**
	* ----------------------------------------------------------------
	* Actual Scene State Things
	* ----------------------------------------------------------------
	**/

	//Sets the current scene's state
	public enum CurrentSceneState
	{
		Tutorial, //tutorial scene, bunch of misc data
		Stage_1, // stage 2
		Stage_2, //stage 3
	}

	//What the current player is doing
	public enum StatusOfPlayer{
		InDialogue, //means player is in dialogue
		LookingAtSomething, //means player is looking at something
		FreeRoam, //player is moving about
	}

	//Converts CurrentSceneState to String
	public string CurrentStateAsString() {
		switch(sceneState) {
			case CurrentSceneState.Tutorial:
				return "Tutorial";
			case CurrentSceneState.Stage_1:
				return "Stage_1";
			case CurrentSceneState.Stage_2:
				return "Stage_2";
			default:
				return "Unknown";
		}
	}

	public CurrentSceneState sceneState;  //current scene state
	public StatusOfPlayer PlayerStatus; //current player state
	public StatusOfPlayer PreviousState; //previous player state

	public string CurrentObjective; //the current objective

}
