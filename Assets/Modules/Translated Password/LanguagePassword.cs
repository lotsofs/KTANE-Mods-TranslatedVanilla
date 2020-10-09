﻿using UnityEngine;

[CreateAssetMenu(fileName = "Language for Password")]
public class LanguagePassword : Language {

	public enum DisplayMethods {
		Default,
		//FallbackFont,
		CustomTextMesh,
		Sprite
	}

	[Header("Display")]
	[Tooltip("Set to NonLatin if TMP_SubMesh throws a log stating a FallBackMaterial is being used. Set to CustomTextMesh if the text requires a custom font below. Set to sprite to use sprites.")]
	public DisplayMethods DisplayMethod = DisplayMethods.Default;

	[Tooltip("The unity editor does not properly show right to left languages, showing them as left to right instead. If this happens on the actual text mesh too, the letters must be manually switched around 'siht ekil'. Tick this to make the mod do that for you. This will not change anything in the editor and it will maintain the incorrect left-to-right look. It is recommended to copy paste the texts into a text editor and edit them there. This does not do anything in the editor.")]
	public bool RightToLeft = false;

	[Header("Module Text")]
	public string Submit = "SUBMIT";
	public string PossibleLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
	public string[] PossibleWords = new string[] {
			"ABOUT", "AFTER", "AGAIN", "BELOW", "COULD",
			"EVERY", "FIRST", "FOUND", "GREAT", "HOUSE",
			"LARGE", "LEARN", "NEVER", "OTHER", "PLACE",
			"PLANT", "POINT", "RIGHT", "SMALL", "SOUND",
			"SPELL", "STILL", "STUDY", "THEIR", "THERE",
			"THESE", "THING", "THINK", "THREE", "WATER",
			"WHERE", "WHICH", "WORLD", "WOULD", "WRITE"
		};

	[Header("Log File Text")]
	public string LogAnswer = "Correct answer: {0}";
	public string LogSubmitCorrect = "Correctly submitted {0}";
	public string LogSubmitWrong = "Strike: Submitted {0}";
	public string LogDial = "Dial {0}: {1}";

	public override void Choose() {
		if (PossibleWords.Length != 35) {
			Debug.LogErrorFormat("Password {0}: Expected 35 passwords, only got {1}", Name, PossibleWords.Length);
		}

		string letters = "";
		foreach (char letter in PossibleLetters) {
			if (letters.Contains(letter.ToString())) {
				Debug.LogErrorFormat("Password {0}: Character {1} appears multiple times in possible letters sequence", Name, letter);
			}
			else {
				letters += letter;
			}
		}

		for (int i = 0; i < PossibleWords.Length; i++) {
			string pw = PossibleWords[i];
			if (pw.Length != 5) {
				Debug.LogErrorFormat("Password {0}: Password '{1}' does not consist of 5 characters, but {2}", Name, pw, pw.Length);
			}
			for (int j = 0; j < pw.Length; j++) {
				if (!PossibleLetters.Contains(pw[j].ToString())) {
					Debug.LogErrorFormat("Password {0}: Password '{1}' contains character {2}, but this character does not show up in the string of possible letters", Name, pw, pw[j]);
				}
			}
		}

		if (RightToLeft && !_flipped && !Application.isEditor) {
			Submit = ReverseReadingDirection(Submit);
			for (int i = 0; i < PossibleWords.Length; i++) {
				PossibleWords[i] = ReverseReadingDirection(PossibleWords[i]);
			}
		}
	}
}
