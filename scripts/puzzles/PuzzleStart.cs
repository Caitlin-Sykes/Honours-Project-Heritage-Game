using Godot;
using System;

public partial class PuzzleStart : Control
{
	private WTP puzzleOne;

	private static string CURRENT_RED_CIRCLE;

	public override void _Ready() {
		puzzleOne = new WTP();
	}

	/**
	* ----------------------------------------------------------------
	* Handlers for checking Puzzles
	* ----------------------------------------------------------------
	**/

	//A function to check if a puzzle is present, and whether to enable it or not
	public void CheckPuzzle(ButtonOverwrite circle) {
		try {
			
			//If = true and has the meta
			if (circle.HasMeta("PuzzleEnabled") && (bool)circle.GetMeta("PuzzleEnabled")) {

				//Saves Current Red Circles Path
				CURRENT_RED_CIRCLE = String.Format("PuzzlesPanel/{0}/{1}", circle.GetParent().Name, circle.Name);

				//Gets the path by getting the circles parent's name and the circles own name (ie, /North/1)
				TogglePuzzleVisibility(GetNode<ButtonOverwrite>(CURRENT_RED_CIRCLE));

				
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
	* Initialize Puzzle
	* ----------------------------------------------------------------
	**/
	private void InitializePuzzle(int puzzID) {
		//Hides the red circle
		TogglePuzzleVisibility(GetNode<ButtonOverwrite>(CURRENT_RED_CIRCLE));
		
		switch(puzzID) {
			case 1:
				puzzleOne.InitPuzzle();
				break;


		}
	}

	// private void OnPuzzleBack

	// once gets the book, --> click on desk --> initiate mum dialogue --> woah, look kid, book magic dialogue --> woah, transported to stonewall scene.
}