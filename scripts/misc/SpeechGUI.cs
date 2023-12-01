using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;

public partial class SpeechGUI : Control
{

	public TextureRect AvatarNode; //node to hold avatar image
	public Label NameNode; //node to hold the name of the speaker

	public Label SpeechNode; //node to hold the speech of the speaker

	[Signal]
	public delegate void DialogueProgressEventHandler(); //handler for progressing scene text

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// ToggleGUIVisible();
		AvatarNode = GetNode<TextureRect>("Main_Dialogue/Avatar"); //Gets instance of Avatar
		NameNode = GetNode<Label>("Main_Dialogue/Name Container/Name_Box/Name_Label");
		SpeechNode = GetNode<Label>("Main_Dialogue/Speech_Container/Speech"); //Gets instance of Speech
		parseJSON();

	}

	/**
	* ----------------------------------------------------------------
	*	Misc handlers
	* ----------------------------------------------------------------
	**/

	//A function to toggle visibility
	public void ToggleGUIVisible() {
		this.Visible = !this.Visible;
	}

	/**
	* ----------------------------------------------------------------
	*	Speech Handlers
	* ----------------------------------------------------------------
	**/

	//A function to set the name of the speaker
	public void SetNameNode(string Name) {
		NameNode.Text = Name;
	}

	//A function to set the speech
	public void SetSpeechNode(string speech)
	{
		SpeechNode.Text = speech;
	}

	//A function to set the avatar
	public void SetAvatarNode(string path)
	{
		AvatarNode.Texture = (Texture2D) GD.Load(path);
	}

	/**
	* ----------------------------------------------------------------
	*	GUI Input Handlers
	* ----------------------------------------------------------------
	**/

	public void OnGUIClick(InputEvent @evnt) {
		if (@evnt is InputEventMouseButton mouse && @evnt.IsPressed()) {
			EmitSignal("DialogueProgress");
		}
	}

	/**
	* Dialogue testing stuff
	**/

	public void parseJSON() {
			string fileName = "resources\\dialogue.json";
			string jsonString = File.ReadAllText(fileName);
			DialogueStruct Speech = JsonSerializer.Deserialize<DialogueStruct>(jsonString)!;

		if (Speech != null)
		{
			foreach (var entry in Speech.IntroductionScene)
			{
				GD.Print($"ID: {entry.Id}");
				GD.Print($"Speaker: {entry.Speaker}");
				GD.Print($"Dialogue: {entry.Dialogue}");
				GD.Print($"Avatar: {entry.Avatar}");
			}
		}
	}



	//A struct to read in the json string from the corresponding scenes
	public class DialogueStruct
	{
		[JsonPropertyName("Introduction_Scene")]
		public DialogueStructData[] IntroductionScene { get; set; }
	}

	//A class to contain the dialogue read in from Dialogue.Json
	public class DialogueStructData {

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