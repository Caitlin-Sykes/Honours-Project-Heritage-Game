using Godot;
using System;
using System.IO;
using System.Text.Json.Serialization;

public partial class JsonHandler : Node
{

	private readonly string FILE_PATH = "resources//dialogue.json";

	public DialogueStruct Speech; //Holds all speech for the game

	public override void _Ready() {
		PreLoadDialogue();
	}
	/**
	* Dialogue testing stuff
	**/

	public void PreLoadDialogue()
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

  
		//An array to hold the Dialogue information matching property "Controls" 
		[JsonPropertyName("Controls")]
		public DialogueStructData[] Controls { get; set; }

		//An array to hold the Dialogue information matching property "Mum_Dialogue_1" 
		[JsonPropertyName("Mum_Dialogue_1")]
		public DialogueStructData[] Mum_Dialogue_1 { get; set; }

		//An array to hold the Dialogue information matching property "Mum_Dialogue_2" 
		[JsonPropertyName("Mum_Dialogue_2")]
		public DialogueStructData[] Mum_Dialogue_2 { get; set; }

		//An array to hold the Dialogue information matching property "Mum_Dialogue_3" 
		[JsonPropertyName("Mum_Dialogue_3")]
		public DialogueStructData[] Mum_Dialogue_3 { get; set; }

		//An array to hold the Dialogue information matching property "Stonewall_Dialogue" 
		[JsonPropertyName("Stonewall_Dialogue")] 
		public DialogueStructData[] Stonewall_Dialogue { get; set; }

		//An array to hold the Dialogue information matching property "Stonewall_Dialogue_Inside" 
		[JsonPropertyName("Stonewall_Dialogue_Inside")]
		public DialogueStructData[] Stonewall_Dialogue_Inside { get; set; }

		[JsonPropertyName("Stonewall_Yvonne")]
		public DialogueStructData[] Stonewall_Yvonne { get; set; }

		[JsonPropertyName("Stonewall_Martha")]
		public DialogueStructData[] Stonewall_Martha{ get; set; }

		[JsonPropertyName("Stonewall_Leitsch")]
		public DialogueStructData[] Stonewall_Leitsch { get; set; }

		[JsonPropertyName("Safe_Incorrect_TooManyDigits")]
		public DialogueStructData[] Safe_Incorrect_TMD { get; set; }

		[JsonPropertyName("Safe_Incorrect_WrongCode")]
		public DialogueStructData[] Safe_Incorrect_WC { get; set; }
	}

	//A class to contain the variables to store dialogue read in from Dialogue.Json
	public class DialogueStructData
	{
		[JsonPropertyName("Id")]

		public string Id { get; set; }

		[JsonPropertyName("Speaker")]

		public string Speaker { get; set; }

		[JsonPropertyName("Dialogue")]

		public string Dialogue { get; set; }

		[JsonPropertyName("Avatar")]

		public string Avatar { get; set; }

		[JsonPropertyName("Source")]

		public string Source { get; set; }

	}

}
