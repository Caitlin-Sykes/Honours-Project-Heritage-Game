using Godot;
using System;

public partial class PuzzleStart : Control
{
	private WTP puzzleOne;

	public static string CURRENT_RED_CIRCLE {get;set;}

	public override void _Ready() {

		//Initialises the puzzles depending on the scene name
		switch (GetTree().CurrentScene.Name) {
			case "IntroductionScene":
				puzzleOne = GetNode<WTP>("PuzzlesPanel/South");
				break;
			default:
				GD.PrintErr("Either not found the scene, or the name in the switch doesn't match with the root");
				break;
		}
		
		
	}

	/**
	* ----------------------------------------------------------------
	* Handlers for checking Puzzles
	* ----------------------------------------------------------------
	**/

	//A function to check if a puzzle is present, and whether to enable it or not
	public void CheckPuzzle(ButtonOverwrite circle) {
		GD.Print(circle.Name);
		try {
			//If = true and has the meta
			if (circle.HasMeta("PuzzleEnabled") && (bool)circle.GetMeta("PuzzleEnabled")) {

				//Saves Current Red Circles Path
				CURRENT_RED_CIRCLE = String.Format("PuzzlesPanel/{0}/PuzzleCont/{1}", circle.GetParent().Name, circle.Name);

				//Gets the path by getting the circles parent's name and the circles own name (ie, /North/1)
				TogglePuzzleVisibility(GetNode<ButtonOverwrite>(CURRENT_RED_CIRCLE));
			}
		}

		catch (Exception e) {
			GD.PrintErr("Something has gone wrong with checking if there is a puzzle." + e.Message);
		}
	}

	/**
	* ----------------------------------------------------------------
	* Handlers for puzzle visibility
	* ----------------------------------------------------------------
	**/

	//Toggles puzzle visibility by enabling the puzzle circle itself, its parent, and the "Puzzles" control node
	public void TogglePuzzleVisibility(ButtonOverwrite circ) {
		//Toggles Red Circle Visibility
		circ.Visible = !circ.Visible; 

		//Toggles Puzzle Cont Visibility
		var parCirc = (Control)circ.GetParent();
		parCirc.Visible = !parCirc.Visible;

		//Toggles South visibility
		var compDir = (Control)parCirc.GetParent();
		compDir.Visible = !compDir.Visible;

		//Toggles Puzzles master visiblity
		var puzzlesMaster = (Control)compDir.GetParent().GetParent();
		puzzlesMaster.Visible = !puzzlesMaster.Visible;
	}

	//Toggle specific circle
	public void ToggleSpecificCircleVisibility(ButtonOverwrite circ) {
		circ.Visible = !circ.Visible;
	}
		
	/**
	* ----------------------------------------------------------------
	* Initialize Puzzle
	* ----------------------------------------------------------------
	**/
	private void InitialisePuzzle(int puzzID) {
		
		//Hides the red circle
		TogglePuzzleVisibility(GetNode<ButtonOverwrite>(CURRENT_RED_CIRCLE));


		switch (puzzID) {
			case 1:
				puzzleOne.InitPuzzle();
				break;
		}
	}

	// private void OnPuzzleBack

	// once gets the book, --> click on desk --> initiate mum dialogue --> woah, look kid, book magic dialogue --> woah, transported to stonewall scene.
}