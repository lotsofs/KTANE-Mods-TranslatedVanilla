using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Language for Who's on First")]
public class LanguageWhosOnFirst : Language {

	public enum DisplayMethods {
		Default,
		//FallbackFont,
		CustomTextMesh,
		Sprite
	}

	[Header("Display")]
	[Tooltip("Set to NonLatin if TMP_SubMesh throws a log stating a FallBackMaterial is being used. Set to CustomTextMesh if the text requires a custom font below. Set to sprite to use sprites.")]
	public DisplayMethods ScreenDisplayMethod = DisplayMethods.Default;
	public DisplayMethods ButtonsDisplayMethod = DisplayMethods.Default;

	//[Header("Custom Text Meshes")]
	//public Font DialsFont;
	//public Material DialsFontMaterial;
	//public int DialsFontSize;
	//public Vector3 DialsOffset;

	//[Space]	// Not actually in use yet. Perhaps for future languages.
	//public Font ButtonFont;
	//public Material ButtonFontMaterial;
	//public int ButtonFontSize;

	[Header("Module Text")]
	public string[] Displays = new[] {
		"YES", "FIRST", "DISPLAY", "OKAY", "SAYS", "NOTHING",
		"", "BLANK", "NO", "LED", "LEAD", "READ",
		"RED", "REED", "LEED", "HOLD ON", "YOU", "YOU ARE",
		"YOUR", "YOU'RE", "UR", "THERE", "THEY'RE", "THEIR",
				"THEY ARE", "SEE", "C", "CEE"
	};

	public string[] Labels = new[] {
		"READY", "FIRST", "NO", "BLANK", "NOTHING", "YES", "WHAT",
		"UHHH", "LEFT", "RIGHT", "MIDDLE", "OKAY", "WAIT", "PRESS",
		"YOU", "YOU ARE", "YOUR", "YOU'RE", "UR", "U", "UH HUH",
		"UH UH", "WHAT?", "DONE", "NEXT", "HOLD", "SURE", "LIKE"
	};

	[Header("Twitch Plays")]
	//public string TPCycle = "cycle";
	//public string TPToggle = "toggle";
	//public string TPSubmit = "submit";
	[TextArea]
	//public string TPMessage = "!{0} {1} 1 3 5 [cycle through the letters in columns 1, 3, and 5] | !{0} {1} [cycle through all columns] | !{0} {2} [move all columns down one letter] | !{0} {3} {4} [try to submit a word]";
	public string TPMessage = "!{0} submit WHAT? -press the button that says 'WHAT?'; the phrase must match exactly | !{0} press MR - press the middle right button.";

	[Header("Log File Text")]
	public string LogStage = "Stage {0}";
	public string LogDisplay = "Display is '{0}'";
	public string LogLabels = "Labels are: '{0}', '{1}', '{2}', '{3}', '{4}', '{5}'";
	public string LogAnswer = "Correct button to press: '{0}'";
	public string LogPressedCorrect = "Pressed {0}. Correct!";
	public string LogPressedWrong = "Pressed {0}. Strike!";

	[ContextMenu("Generate Twitch Help Message")]
	void GenerateTwitchHelpMessage() {
		TwitchHelpMessage = TPMessage;
	}

	void OnEnable() {

		if (Displays.Length != 28) {
			Debug.LogErrorFormat("WOF {0}: Expected 28 display words, only got {1}", Name, Displays.Length);
		}
		if (Labels.Length != 28) {
			Debug.LogErrorFormat("WOF {0}: Expected 28 labels, only got {1}", Name, Labels.Length);
		}

		List<string> words = new List<string>();
		foreach (string word in Displays) {
			if (words.Contains(word)) {
				Debug.LogErrorFormat("WOF {0}: word {1} appears multiple times in displays", Name, word);
			}
			else {
				words.Add(word);
			}
		}

		words = new List<string>();
		foreach (string word in Labels) {
			if (words.Contains(word)) {
				Debug.LogErrorFormat("WOF {0}: word {1} appears multiple times in labels, at positions {2} and {3}", Name, word, words.Count, words.IndexOf(w => w == word));
			}
			else {
				words.Add(word);
			}
		}

		FixRightToLeft();
	}

	void FixRightToLeft() {
		if (RightToLeft && !flipped && !Application.isEditor) {
			for (int i = 0; i < 28; i++) {
				//PossibleWords[i] = ReverseReadingDirection(PossibleWords[i]);
				Labels[i] = ReverseReadingDirection(Labels[i]);
				Displays[i] = ReverseReadingDirection(Displays[i]);
			}
			flipped = true;
		}
		if (flipped) {
			Debug.Log("Totally intended secret easter egg message! Please contact LotsOfS if you see this.");
		}
	}
}
