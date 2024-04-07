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
		Stage_1, // stage 1
		Stage_2, //stage 2
		Stage_3 // stage 3
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
			case CurrentSceneState.Stage_3:
				return "Stage_3";
			default:
				return "Unknown";
		}
	}

	public CurrentSceneState sceneState;  //current scene state
	public StatusOfPlayer PlayerStatus; //current player state
	public StatusOfPlayer PreviousState; //previous player state

	public string CURRENT_PATH_CIRCLES; //holds the path to the circles

	public Cameras.Direction DIR;

	public Cameras.Direction PREVIOUS_DIR;

	public string CurrentObjective; //the current objective
	
	public int TimesStuck = -1; //keeps track of the number of times stuck


}
