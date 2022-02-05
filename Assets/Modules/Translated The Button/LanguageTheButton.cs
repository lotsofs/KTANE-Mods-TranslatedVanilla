using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Language for The Button")]
public class LanguageTheButton : Language {

	public enum DisplayMethods {
		Default,
		CustomTextMesh,
		//Sprite
	}

	[Header("Display")]
	[Tooltip("Set to NonLatin if TMP_SubMesh throws a log stating a FallBackMaterial is being used. Set to CustomTextMesh if the text requires a custom font below. Set to sprite to use sprites.")]
	public DisplayMethods DisplayMethod = DisplayMethods.Default;

	[Header("Module Text")]
	[TextArea] public string Press = "PRESS";
	[TextArea] public string Hold = "HOLD";
	[TextArea] public string Abort = "ABORT";
	[TextArea] public string Detonate = "DETONATE";

	[Header("Log File Text")]
	public string LogPress = "Press";
	public string LogHold = "Hold";
	public string LogAbort = "Abort";
	public string LogDetonate = "Detonate";

	[Header("Custom Text Mesh Settings")]
	public Font Font;
	public Material FontMaterial;
	public float VerticalOffset = 0f;
	
	[Space]
	public int SizePress = -1;
	public int SizeHold = -1;
	public int SizeAbort = -1;
	public int SizeDetonate = -1;

	[Header("Sprites")]
	public Sprite SpritePress;
	public Sprite SpriteHold;
	public Sprite SpriteAbort;
	public Sprite SpriteDetonate;

	//[Header("Color Blind Support (Not Implemented)")]
	//public Material ButtonRed;
	//public Material ButtonYellow;
	//public Material ButtonBlue;
	//public Material ButtonWhite;
	//[TextArea] public string StripRed;
	//[TextArea] public string StripYellow;
	//[TextArea] public string StripBlue;
	//[TextArea] public string StripWhite;

	[Header("Twitch Plays")]
	public string TPTap = "tap";
	public string TPHold = "hold";
	public string TPRelease = "release";
	public string TPMessage = "!{0} {1} [tap the button] | !{0} {2} [hold the button] | !{0} {3} 7 [release when the digit shows 7]";

	[Header("Log File Ruling Text")]
	public string LogRed = "Red";
	public string LogYellow = "Yellow";
	public string LogBlue = "Blue";
	public string LogWhite = "White";

	[Space]

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


	private Dictionary<string, string> _buttonLabels;
	private Dictionary<string, string> _logLabels;
	private Dictionary<string, Sprite> _spriteLabels;
	private Dictionary<string, int> _sizeLabels;

	//[ContextMenu("Generate Twitch Help Message")]
	//void GenerateTwitchHelpMessage() {
	//	TwitchHelpMessage = string.Format(TPMessage,
	//		"{0}",
	//		TPTap,
	//		TPHold,
	//		TPRelease
	//	);
	//}

	void OnEnable() {
		FixRightToLeft();

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

		_sizeLabels = new Dictionary<string, int> {
			{ "Press", SizePress },
			{ "Hold", SizeHold },
			{ "Abort", SizeAbort },
			{ "Detonate", SizeDetonate },
		};
	}

	void FixRightToLeft() {
		if (RightToLeft && !flipped && !Application.isEditor) {
			Press = ReverseReadingDirection(Press);
			Hold = ReverseReadingDirection(Hold);
			Abort = ReverseReadingDirection(Abort);
			Detonate = ReverseReadingDirection(Detonate);
			flipped = true;
		}
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

	public int GetSizeFromEnglishName(string str) {
		return _sizeLabels[str];
	}
}
