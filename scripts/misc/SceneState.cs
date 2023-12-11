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

	public static CurrentSceneState sceneState = CurrentSceneState.Tutorial; //current scene state
}
