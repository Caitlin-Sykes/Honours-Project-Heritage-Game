using Godot;
using System;


public partial class PlayerData : Node
{ 
	public PlayerDataStruct Player;

	public void CreatePlayer(String name, DateTime time, string pronouns, string id, int currentAvatar) {
		Player = new PlayerDataStruct(name.Trim(), time, pronouns, id, currentAvatar);
	}

	// Called when the node enters the scene tree for the first time.
	public PlayerData()
	{
		Player = new PlayerDataStruct("Player", DateTime.Now, "They/Them", GetRandomID(), 1);
	}


	private string GetRandomID() {

		string nbr = "";
		Random rdm = new Random();

		//Sets random id number
		for (int num = 0; num < 7; num++)
		{
			nbr += rdm.Next(0, 9).ToString();
		}

		return nbr;
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
