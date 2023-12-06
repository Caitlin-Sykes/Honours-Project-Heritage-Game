using Godot;
using System;
using System.IO;
using System.Text.Json.Serialization;

public static class JsonHandler
{

	private readonly static string FILE_PATH = "resources//dialogue.json";

	public static DialogueStruct Speech; //Holds all speech for the game


	/**
	* Dialogue testing stuff
	**/

	public static void PreLoadDialogue()
	{
		//Reads all in and stores in Speech variable
		string jsonString = File.ReadAllText(FILE_PATH);
		Speech = System.Text.Json.JsonSerializer.Deserialize<DialogueStruct>(jsonString)!;
	}



	//A struct to read in the json string from the corresponding scenes
	public class DialogueStruct
	{

		//An array to hold the Dialogue information matching property "Introduction_Scene"
		[JsonPropertyName("Introduction_Scene")]
		public DialogueStructData[] IntroductionScene { get; set; }

		//An array to hold the Dialogue information matching property "Testing"
		[JsonPropertyName("Controls")]
		public DialogueStructData[] Controls { get; set; }
}

	//A class to contain the variables to store dialogue read in from Dialogue.Json
	public class DialogueStructData
	{
		[JsonPropertyName("SceneName")]
		public string SceneName { get; set; }

		[JsonPropertyName("Id")]

		public string Id { get; set; }

		[JsonPropertyName("Speaker")]

		public string Speaker { get; set; }

		[JsonPropertyName("Dialogue")]

		public string Dialogue { get; set; }

		[JsonPropertyName("Avatar")]

		public string Avatar { get; set; }

	}

}
