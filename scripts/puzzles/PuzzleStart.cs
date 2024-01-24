using Godot;
using System;

public partial class PuzzleStart : Control
{
	// Called when the node enters the scene tree for the first time.
	// public override void _Ready()
	// {
	// }

	// // Called every frame. 'delta' is the elapsed time since the previous frame.
	// public override void _Process(double delta)
	// {
	// }

	/**
	* ----------------------------------------------------------------
	* Handlers for checking Puzzles
	* ----------------------------------------------------------------
	**/

	//A function to check if a puzzle is present, and whether to enable it or not
	public void CheckPuzzle(ButtonOverwrite circle) {
		try {
			var puzzleEnabled = (Boolean)circle.GetMeta("PuzzleEnabled");
			GD.Print(circle.Name);
			GD.Print(circle.GetParent().Name);
			//If = true
			if (puzzleEnabled) {
				
				//Gets the path by getting the circles parent's name and the circles own name (ie, /North/1)
				TogglePuzzleVisibility(GetNode<ButtonOverwrite>(String.Format("PuzzlesPanel/{0}/{1}", circle.GetParent().Name, circle.Name)));
			}

			else {
				GD.Print("No puzzle");
			}
		}

		catch {
			GD.PrintErr("Something has gone wrong with checking if there is a puzzle.");
		}
	}

	/**
	* ----------------------------------------------------------------
	* Handlers for puzzle visibility
	* ----------------------------------------------------------------
	**/

	//Toggles puzzle visibility by enabling the puzzle circle itself, its parent, and the "Puzzles" control node
	private void TogglePuzzleVisibility(ButtonOverwrite circ) {
		circ.Visible = !circ.Visible;
		var parCirc = (Control)circ.GetParent();
		parCirc.Visible = !parCirc.Visible;
		var puzzlesControl = (Control)parCirc.GetParent().GetParent();
		puzzlesControl.Visible = !puzzlesControl.Visible;

	}

	/**
	* ----------------------------------------------------------------
	* Toggle
	* ----------------------------------------------------------------
	**/

	// private void OnPuzzleBack
}