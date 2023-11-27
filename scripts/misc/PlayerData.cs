using Godot;
using System;


public partial class PlayerData : Node
{ 
	public static PlayerDataStruct Player;

	public static void CreatePlayer(String name, DateTime time, string pronouns, string id, int currentAvatar) {
		Player = new PlayerDataStruct(name.Trim(), time, pronouns, id, currentAvatar);
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
