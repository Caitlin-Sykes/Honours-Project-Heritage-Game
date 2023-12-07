using Godot;
using System;

public partial class InteractCircles : Node3D
{
	[Signal]
	public delegate void ToggleNorthEventsEventHandler();

	[Signal]
	public delegate void ToggleEastEventsEventHandler();

	[Signal]
	public delegate void ToggleSouthEventsEventHandler(); // These all show/hide the corresponding events for the scene

	[Signal]
	public delegate void ToggleWestEventsEventHandler();
	
	[Signal]
	public delegate void ToggleUpEventsEventHandler();
	
	[Signal]
	public delegate void ToggleDownEventsEventHandler();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// attach unique meta data to each circle imported 
	// means no repetitve scripts
	// show specific node on command ping

	private void CirclesPressed()
	{
		throw new NotImplementedException();
	}



	/**
	* ----------------------------------------------------------------
	* Handlers for enabling and disabling event circles
	* ----------------------------------------------------------------
	**/

	//Enables the events circle
	private void ToggleEventsDirection(NodePath path)
	{

		//Gets the direction to be enabled, passed in by path
		BoxContainer dir = GetNode<BoxContainer>(path);

		//For every circle in the direction container, toggles them
		foreach (Node3D circle in dir.GetChildren())
		{
			circle.Visible = !circle.Visible;
		}
	}

	//Enables a specific event circle
	public void ToggleSpecificDirection(Area3D cir)
	{

		//Gets the circle to be enabled, passed in by path
		try {
			cir.Visible = !cir.Visible;
		}

		catch (InvalidCastException e) {
			GD.Print("Cannot find the circle to be disabled. Is the path correct? Specific Error is: " + e.Message);
		}

		
	}





}
