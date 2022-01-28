using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Language for Morse Code")]
public class LanguageMorseCode : Language {

	public enum DisplayMethods {
		Default,
		//FallbackFont,
		CustomTextMesh,
		//Sprite
	}

	[Header("Display")]
	[Tooltip("Set to NonLatin if TMP_SubMesh throws a log stating a FallBackMaterial is being used. Set to CustomTextMesh if the text requires a custom font below. Set to sprite to use sprites.")]
	public DisplayMethods DialsDisplayMethod = DisplayMethods.Default;
	public DisplayMethods ButtonDisplayMethod = DisplayMethods.Default;

	[Header("Custom Text Meshes")]
	public Font TxFont;
	public Material TxFontMaterial;
	public int TxFontSize;
	public Vector3 TxOffset;
	//[Space]	// Not actually in use yet. Perhaps for future languages.
	//public Font ButtonFont;
	//public Material ButtonFontMaterial;
	//public int ButtonFontSize;

	[Header("Module Text")]
	public string Transmit = "TX";
	public string MegaHerz = "MHz";

	public char[] Characters = new char[] { 
			'A', 'B', 'C', 'D', 'E', 'F', 'G', 
			'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 
			'Q', 'R', 'S', 'T', 'U', 'V', 
			'W', 'X', 'Y', 'Z' };

	public string Digits = "0123456789";

	public string[] MorseSymbols = new string[] {
			".-", "-...", "-.-.", "-..", ".", "..-.", "--.",
			"....", "..", ".---", "-.-", ".-..", "--", "-.", "---", ".--.", 
			"--.-", ".-.", "...", "-", "..-", "...-", 
			".--", "-..-", "-.--", "--.." };


	public string[] PossibleWords = new string[] {
			"SHELL", "HALLS", "SLICK", "TRICK", 
			"BOXES", "LEAKS", "STROBE", "BISTRO", 
			"FLICK", "BOMBS", "BREAK", "BRICK", 
			"STEAK", "STING", "VECTOR", "BEATS"
		};
	public string[] WordsManual;

	[Header("Twitch Plays")]
	public string[] TPTx = new string[] { "transmit", "trans", "tx" };
	
	[TextArea]
	public string TPMessage = "!{0} {1} 3.575, !{0} {2} 575, !{0} {3} 3.575 {5}, !{0} {4} 575 [transmit frequency 3.575]";

	[Header("Log File Text")]
	public string LogAnswer = "Chosen word is: {0}; Signal is: {1}; Correct frequency is {2} MHz";
	public string LogSubmitCorrect = "Transmit button pressed when selected frequency is {0}. Correct!";
	public string LogSubmitWrong = "Transmit button pressed when selected frequency is {0}. Strike!";

	[ContextMenu("Generate Twitch Help Message")]
	void GenerateTwitchHelpMessage() {
		TwitchHelpMessage = string.Format(TPMessage,
			"{0}",
			TPTx[0 % TPTx.Length],
			TPTx[1 % TPTx.Length],
			TPTx[2 % TPTx.Length],
			TPTx[3 % TPTx.Length],
			MegaHerz
		);
	}

	void OnEnable() {

		if (Characters.Length != MorseSymbols.Length) {
			Debug.LogErrorFormat("Morse Code {0}: Character table and morse symbol table length do not match");
		}

		if (PossibleWords.Length != 16) {
			Debug.LogErrorFormat("Morse Code {0}: Expected 16 morse words, only got {1}", Name, PossibleWords.Length);
		}

		string letters = "";
		foreach (char letter in Characters) {
			if (letters.Contains(letter.ToString())) {
				Debug.LogErrorFormat("Morse Code {0}: Character {1} appears multiple times in possible letters sequence", Name, letter);
			}
			else {
				letters += letter;
			}
		}

		for (int i = 0; i < PossibleWords.Length; i++) {
			string pw = PossibleWords[i];
			for (int j = 0; j < pw.Length; j++) {
				if (!Characters.Contains(pw[j])) {
					Debug.LogErrorFormat("Morse Code {0}: Word '{1}' contains character {2}, but this character does not show up in the string of possible letters", Name, pw, pw[j]);
				}
			}
		}

		FixRightToLeft();
	}

	void FixRightToLeft() {
		if (RightToLeft && !flipped && !Application.isEditor) {
			Transmit = ReverseReadingDirection(Transmit);
			flipped = true;
		}
		if (flipped) {
			Debug.Log("Totally intended secret easter egg message! Please contact LotsOfS if you see this.");
		}
	}
}
