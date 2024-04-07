using Godot;
using System;

public partial class PuzzleStart : Control
{
	private WTP puzzleOne;

	private Safecracking puzzleTwo;

	[Export] private InteractCircles CIRCLES;


	public static string CURRENT_RED_CIRCLE {get;set;}

	public override void _Ready() {

		//Initialises the puzzles depending on the scene name
		switch (GetTree().CurrentScene.Name) {
			case "IntroductionScene":
				puzzleOne = GetNode<WTP>("PuzzlesPanel/South");
				break;
			case "Stonewall":
				// puzzleTwo = GetNode<Safecracking>("PuzzlesPanel/West");
				break;
			default:
				GD.PrintErr("Error with the scene, or the name in the switch doesn't match with the root");
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
	
	//Toggles puzzle visibility by enabling the puzzle circle itself, its parent, and the "Puzzles" control node
	public void TogglePuzzleVisibilityParentForceOn(ButtonOverwrite circ)
	{

		circ.Visible = true;

		GetNode<Control>("PuzzlesPanel/West/PuzzleCont/Components").Visible = true;
		
		//Toggles Puzzle Cont Visibility
		var parCirc = (Control)circ.GetParent();
		parCirc.Visible = true;

		//Toggles South visibility
		var compDir = (Control)parCirc.GetParent();
		compDir.Visible = true;

		//Toggles Puzzles master visiblity
		var puzzlesMaster = (Control)compDir.GetParent().GetParent();
		puzzlesMaster.Visible = true;
	}
	
	//Toggle specific circle
	public void ToggleSpecificCircleVisibility(ButtonOverwrite circ) {
		circ.Visible = !circ.Visible;
	}
	
	//Toggle specific circle by path
	public void ToggleSpecificCircleVisibilityPath(String circ)
	{
		ButtonOverwrite btn = GetNode<ButtonOverwrite>(circ);
		btn.Visible = !btn.Visible;
	}
	//Handlers all the clickable components
	public void EnableAllCircleComponents(String path)
	{
		Control components = GetNode<Control>(path);

		//Toggles visibility of the parent node
		components.Visible = !components.Visible;
		//For every circle/panel in the components container, toggles them
		foreach (var comp in components.GetChildren())
		{
			if (comp is ButtonOverwrite circle) {
				circle.Visible = !circle.Visible;
			}

			else if (comp is Panel panel) {
				panel.Visible = !panel.Visible; ;
			}
		}

		if (CIRCLES.BackButtonContainer.Visible == false)
		{
			CIRCLES.BackButtonContainer.Visible = true;
		}
	}

	//Handlers all the clickable components
	public void HideAllCircleComponents(String path)
	{

		Control components = GetNode<Control>(path);
		components.Visible = false;	
		//For every circle in the components container, toggles them
		foreach (var vari in components.GetChildren())
		{
			if (vari is ButtonOverwrite btn)
			{
				btn.Visible = false;
			}
			
			else if (vari is Panel pnl)
			{
				pnl.Visible = false;
			}
		}
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

			case 2:
				// puzzleTwo.InitPuzzle();
				break;
		}
	}
}