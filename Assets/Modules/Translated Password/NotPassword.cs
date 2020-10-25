using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NotVanillaModulesLib;
using UnityEngine;
using Random = UnityEngine.Random;

public class NotPassword : NotVanillaModule<NotPasswordConnector> {
	public int SolutionCount { get; private set; }
	public int SolutionDelay { get; private set; }

	public bool Down { get; private set; }
	public bool Holding { get; private set; }
	public int MashCount { get; private set; }
	public bool WasPressed { get; private set; }
	public int InteractionTickCount { get; private set; }
	public bool PressedIncorrectly { get; private set; }

	private bool twitchStruck;

	TranslatedPassword _translation;

	string _letters;
	string[] _words;

	public override void Start() {

		base.Start();
		this.Connector.KMBombModule.OnActivate = this.KMBombModule_OnActivate;
		this.Connector.SubmitPressed += this.Connector_SubmitPressed;

		string name = string.Format("{0} #{1}", Connector.KMBombModule.ModuleDisplayName, Connector.ModuleID);
		_translation = GetComponent<TranslatedPassword>();
		_translation.GenerateLanguage(name);

		if (_translation.Language.ButtonDisplayMethod == LanguagePassword.DisplayMethods.CustomTextMesh) {
			this.Connector.UseCustomButtonLabel();
		}
		this.Connector.SetButtonLabel(_translation.Language.Submit);
		_letters = _translation.Language.PossibleLetters;
		_words = _translation.Language.PossibleWords;

		// fill all dials with all letters
		List<char>[] dials = new List<char>[5];
		for (int i = 0; i < 5; i++) {
			dials[i] = _letters.ToList();
		}

		// select a word
		string correctWord = _words[Random.Range(0, _words.Length)];
		Log(_translation.Language.LogAnswer, correctWord);

		PruneFalseMatches(dials, correctWord);
		ReduceToCharacterCount(dials, correctWord);
		if (_translation.Language.RightToLeft) { 
			dials = dials.Reverse().ToArray();
		}

		if (_translation.Language.DialsDisplayMethod == LanguagePassword.DisplayMethods.CustomTextMesh) {
			Connector.UseCustomSpinners(_translation.Language.DialsFontSize, _translation.Language.DialsOffset, _translation.Language.DialsFont, _translation.Language.DialsFontMaterial);
		}

		for (int i = 0; i < 5; i++) {
			int j = _translation.Language.RightToLeft ? 5 - 1 - i : i;

			string letters = "";
			foreach (char c in dials[j]) {
				letters += c;
				letters += " ";
			}
			Log(_translation.Language.LogDial, i + 1, letters);
			Connector.SetSpinnerChoices(i, dials[i]);
		}
	}

	#region password generation

	void ReduceToCharacterCount(List<char>[] dials, string correctWord) {
		for (int i = 0; i < dials.Length; i++) {
			dials[i].Remove(correctWord[i]);
			while (dials[i].Count > 5) {
				dials[i].RemoveAt(Random.Range(0, dials[i].Count));
			}
			dials[i].Add(correctWord[i]);
			dials[i].Shuffle();
		}
	}

	void PruneFalseMatches(List<char>[] dials, string correctWord) {
		List<string> falseMatches = GetFalseMatches(dials, correctWord);
		while (falseMatches.Count > 0) {
			string match = falseMatches[0];
			List<int> incorrectCharPositions = GetIncorrectCharPositions(match, correctWord);
			if (incorrectCharPositions.Count > 0) {
				int dialIndexToRemoveLetterFrom = incorrectCharPositions[Random.Range(0, incorrectCharPositions.Count)];
				char letterToRemove = match[dialIndexToRemoveLetterFrom];
				dials[dialIndexToRemoveLetterFrom].Remove(letterToRemove);
			}
			falseMatches = GetFalseMatches(dials, correctWord);
		}
	}

	List<int> GetIncorrectCharPositions(string falseWord, string correctWord) {
		List<int> list = new List<int>();
		for (int i = 0; i < falseWord.Length; i++) {
			if (falseWord[i] != correctWord[i]) {
				list.Add(i);
			}
		}
		return list;
	}

	List<string> GetFalseMatches(List<char>[] dials, string correctWord) {
		List<string> matches = GetMatches(dials);
		matches.Remove(correctWord);
		return matches;
	}

	List<string> GetMatches(List<char>[] dials) {
		List<string> matches = new List<string>();
		foreach (string word in _words) {
			char[] charArray = word.ToCharArray();
			bool match = true;
			for (int i = 0; i < dials.Length; i++) {
				if (!dials[i].Contains(charArray[i])) {
					match = false;
					break;
				}
			}
			if (match) {
				matches.Add(word);
			}
		}
		return matches;
	}

	#endregion

	private void KMBombModule_OnActivate() {
		this.Connector.Activate();
	}

	private void Connector_SubmitPressed(object sender, EventArgs e) {
		string word = Connector.GetSpinnerChoices(_translation.Language.RightToLeft);
		if (_words.Contains(word)) {
			if (!Solved) {
				Log(_translation.Language.LogSubmitCorrect, word);
				Disarm();
			}
		}
		else {
			if (!Solved) {
				Log(_translation.Language.LogSubmitWrong, word);
				Connector.KMBombModule.HandleStrike();
			}
		}
	}

	#region TP

	// Twitch Plays support
	public static readonly string TwitchHelpMessage
		= "!{0} cycle 1 3 5 - cycle the letters in columns 1, 3 and 5 | !{0} toggle - move all columns down one letter | " +
		"!{0} tap - tap once | !{0} tap on 5 - tap when the timer contains a 5 | !{0} tap 5 - tap 5 times | !{0} tap 5:59 | !{0} tap 5:59 then 5:54 | " +
		"!{0} mash - tap until something happens | !{0} hold | !{0} hold on 5 | !{0} hold for 5 | !{0} release | !{0} release on 2";
	// `!{0} cycle` is deliberately excluded because it takes too long and its use is generally frowned on.
	[NonSerialized]
	public bool TwitchPlaysActive;
	[NonSerialized]
	public bool ZenModeActive;
	public IEnumerator ProcessTwitchCommand(string command) {
		var tokens = command.Split(new[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
		if (tokens.Length == 0) yield break;

		this.twitchStruck = false;

		var bombInfo = this.GetComponent<KMBombInfo>();

		if (true) {
			if (tokens[0].EqualsIgnoreCase("cycle")) {
				if (tokens.Length == 1)
					yield return "sendtochaterror You must specify one or more columns to cycle.";
				else {
					var indices = new List<int>();
					foreach (var token in tokens.Skip(1)) {
						int i;
						if (int.TryParse(token, out i) && i > 0 && i <= 5)
							indices.Add(i);
						else
							yield break;
					}
					yield return null;
					if (indices.Count >= 3) yield return "waiting music";
					foreach (var i in indices) {
						for (int j = 0; j < 5; ++j) {
							this.Connector.TwitchMoveDown(i - 1);
							yield return "trywaitcancel 1";
						}
					}
				}
			} 
			else if (tokens[0].EqualsIgnoreCase("toggle")) {
				if (tokens.Length == 1) {
					yield return null;
					for (int i = 0; i < 5; ++i) {
						this.Connector.TwitchMoveDown(i);
						yield return new WaitForSeconds(0.1f);
					}
				}
			} 
			else if (tokens[0].EqualsIgnoreCase("tap") || tokens[0].EqualsIgnoreCase("press")) {
				this.Connector.TwitchPressSubmit();
				this.Connector.TwitchReleaseSubmit();
				// todo: turn this into a normal press, or actually, make it "!{0} submit <word>"
			}
		}
	}

	public IEnumerator TwitchHandleForcedSolve() {
		// todo: make this
		yield return null;
	}

	#endregion

}
