using Godot;
using System;


public partial class PlayerData : Node
{ 
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}


//A struct for holding player data
public struct PlayerDataStruct
{

	public PlayerDataStruct(String name, DateTime dateTime, String pronoun, string id, int avatar)
	{
		Name = name;
		Date = dateTime;
		Pronouns = pronoun;
		ID = id;
		Avatar = avatar;
	}

	public string Name { get; init; }
	public DateTime Date { get; init; }
	public string Pronouns { get; init; }

	public string ID {get; init;}
	public int Avatar { get; init; }



}
