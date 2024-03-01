using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class EventDict : Node2D
{
    public Dictionary<string, string[]> EventLookup = new Dictionary<string, string[]>();


    //Constructor inits the event lookup dict
    public override void _Ready()
    {
        EventLookup.Add("Introduction_Scene", new string[] {"7"});
        EventLookup.Add("Controls", new string[] {"1", "3", "7", "8"});
        EventLookup.Add("Mum_Dialogue_1", new string[] { "5" });
        EventLookup.Add("Mum_Dialogue_2", new string[] { "8" });
        EventLookup.Add("Stonewall_Dialogue", new string[] { "11" });
    }

    //A function to check if the id is present 
    public bool CheckIfSceneTrigger(string name, string trigger) {
        //If the scene is present
        return EventLookup.ContainsKey(name) && CheckIfTriggerInArray(name, trigger);
    }

    // A function to check if the trigger is present in the dict
    private bool CheckIfTriggerInArray(string name, string trigger) {
        return EventLookup.Where(x => x.Key == name).SelectMany(x => x.Value).Contains(trigger);
    }
}