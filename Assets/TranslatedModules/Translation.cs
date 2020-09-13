using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Translation : MonoBehaviour {

	[Tooltip("Disabled languages are never chosen, eg. because they're still a work in progress.")]
	public bool Disabled = false;
	[Header("Language Information")]
	public string Name;
	public string NativeName;
	public string Iso639;
	[Tooltip("Ticking this will swap the order of the Yes/No buttons. Used for right to left reading languages.")]
	public bool SwapButtons = false;
	[Space]
	public int Version = 1;
	public bool ManualAvailable = false;
	
	[Header("Display")]
	[Tooltip("Leave null to use the standard font for the module.")]
	public Font Font;
	[Tooltip("Leave null to use the standard font for the module.")]
	public Material FontMaterial;
	public float VerticalOffset = 0f;
	[Tooltip("Untick if TMP_SubMesh throws a log stating a FallBackMaterial is being used.")]
	public bool LatinScript = true;

	[Space]
	[Header("Module Text")]
	public string Press = "PRESS";
	public string Hold = "HOLD";
	public string Abort = "ABORT";
	public string Detonate = "DETONATE";

	[Header("Log File Text")]
	public string LogPress = "Press";
	public string LogHold = "Hold";
	public string LogAbort = "Abort";
	public string LogDetonate = "Detonate";

	[Header("Sprites")]
	[Tooltip("Tick this to use sprites instead of textmesh, for when textmesh doesn't support your script.")]
	public bool UseSprites = false;
	public Sprite SpritePress;
	public Sprite SpriteHold;
	public Sprite SpriteAbort;
	public Sprite SpriteDetonate;

	[Space]
	[Header("Log File Ruling Text")]
	public string LogRed = "Red";
	public string LogYellow = "Yellow";
	public string LogBlue = "Blue";
	public string LogWhite = "White";

	public string RuleColorIs = "Color is {0}.";
	public string RuleLabelIs = "Label is {0}.";
	public string RuleButtonShouldBe = "The button should be {0}.";
	public string RuleHeld = "held";
	public string RulePressed = "pressed and immediately released";
	public string LogHeldCorrect = "The button was held and released at {0}. That was correct.";
	public string LogHeldIncorrect = "The button was held and released at {0}. That was incorrect: It should have been pressed and immediately released.";
	public string LogHeldIncorrectRelease = "The button was held and released at {0}. That was incorrect, as {0} does not contain a {1}.";
	public string LogPressedCorrect = "The button was pressed and immediately released. That was correct.";
	public string LogPressedIncorrect = "The button was pressed and immediately released. That was incorrect: It should have been held.";
	public string LogHoldingCorrect = "The button is being held. That is correct. The light is {0}.";
	public string LogHoldingIncorrect = "The button is being held. That is incorrect. The light is {0}.";


	private Dictionary<string,string> _buttonLabels;
	private Dictionary<string,string> _logLabels;
	private Dictionary<string,Sprite> _spriteLabels;

	public void Choose() {
		_buttonLabels = new Dictionary<string, string> {
			{ "Press", Press },
			{ "Hold", Hold },
			{ "Abort", Abort },
			{ "Detonate", Detonate },
		};

		_logLabels = new Dictionary<string, string> {
			{ "Press", LogPress },
			{ "Hold", LogHold },
			{ "Abort", LogAbort },
			{ "Detonate", LogDetonate },
			{ "Red", LogRed },
			{ "Yellow", LogYellow },
			{ "Blue", LogBlue },
			{ "White", LogWhite },
		};

		_spriteLabels = new Dictionary<string, Sprite> {
			{ "Press", SpritePress },
			{ "Hold", SpriteHold },
			{ "Abort", SpriteAbort },
			{ "Detonate", SpriteDetonate },
		};
	}

	public string GetLabelFromEnglishName(string str) {
		return _buttonLabels[str];
	}

	public string GetLogFromEnglishName(string str) {
		return _logLabels[str];
	}

	public Sprite GetSpriteFromEnglishName(string str) {
		return _spriteLabels[str];
	}
}
