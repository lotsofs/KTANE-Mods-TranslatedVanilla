using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TranslatedVanillaModulesLib;
using UnityEngine;
using Random = UnityEngine.Random;

public class Password : TranslatedVanillaModule<TranslatedPasswordConnector> {
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
		string word = Connector.GetWord(_translation.Language.RightToLeft);
		if (_words.Contains(word)) {
			if (!Solved) {
				Log(_translation.Language.LogSubmitCorrect, word);
				Disarm();
			}
		}
		else {
			Log(_translation.Language.LogSubmitWrong, word);
			Connector.KMBombModule.HandleStrike();
		}
	}

	#region TP

	// Twitch Plays support
	public static readonly string TwitchHelpMessage
		= "!{0} cycle 1 3 5 [cycle through the letters in columns 1, 3, and 5] | " +
		"!{0} toggle [move all columns down one letter] | !{0} submit ABCDE [try to submit a word]";


	// `!{0} cycle` is deliberately excluded because it takes too long and its use is generally frowned on.
	[NonSerialized]
	public bool TwitchPlaysActive;
	[NonSerialized]
	public bool ZenModeActive;
	public IEnumerator ProcessTwitchCommand(string command) {
		var tokens = command.Split(new[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
		if (tokens.Length == 0) yield break;

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
						int connector = i - 1;
						if (_translation.Language.RightToLeft) {
							connector = 4 - connector;
						}
						for (int j = 0; j < 5; ++j) {
							this.Connector.TwitchMoveDown(connector);
							yield return "trywaitcancel 1";
						}
					}
				}
			} 
			else if (tokens[0].EqualsIgnoreCase("toggle")) {
				if (tokens.Length == 1) {
					yield return null;
					if (_translation.Language.RightToLeft) {
						for (int i = 4; i >= 0; --i) {
							this.Connector.TwitchMoveDown(i);
							yield return new WaitForSeconds(0.1f);
						}
					}
					else {
						for (int i = 0; i < 5; ++i) {
							this.Connector.TwitchMoveDown(i);
							yield return new WaitForSeconds(0.1f);
						}
					}
				}
			} 
			else if (tokens[0].EqualsIgnoreCase("submit")) {
				if (tokens.Length == 1) {
					//this.Connector.TwitchPressSubmit();
					//yield return null;
					yield break;
				}
				string password;
				int index = Array.IndexOf(_translation.Language.WordsManual, tokens[1]);
				if (index != -1) {
					password = _translation.Language.PossibleWords[index];
				}
				else if (tokens[1].Length != 5) {
					yield break;
				}
				else {
					password = tokens[1];
				}
				password = password.ToLowerInvariant();
				for (int i = 0; i < 5; ++i) {
					int connector = i;
					if (_translation.Language.RightToLeft) {
						connector = 4 - connector;
					}
					bool next = false;
					for (int j = 0; j < 6; ++j) {
						string word = Connector.GetWord(_translation.Language.RightToLeft).ToLowerInvariant();
						if (word[i] == password[i]) {
							next = true;
							break;
						}
						yield return new WaitForSeconds(0.1f);
						this.Connector.TwitchMoveDown(connector);
					}
					if (next) {
						continue;
					}
					else {
						yield return "sendtochaterror Incorrect password.";
						yield break;
					}
				}
				yield return new WaitForSeconds(0.1f);
				this.Connector.TwitchPressSubmit();
			}
		}
	}

	public int CountMatches(string a, string b) {
		int matches = 0;
		for (int i = 0; i < 5; i++) {
			matches += a[i] == b[i] ? 1 : 0;
		}
		return matches;
	}

	public IEnumerator TwitchHandleForcedSolve() {
		LogFormat("Forcing solve (Twitch Plays)");
		bool rtl = _translation.Language.RightToLeft;
		float delay = 0.1f;

		Dictionary<string, int> passwordLikelihood = new Dictionary<string, int>();

		List<List<char>> memorizedDials = new List<List<char>> { null, null, null, null, null };
		int lastChangedDial = -1;
		List<string> keys;
		int presses = 0;

		// start
		string currentWord = Connector.GetWord() ;
		//LogFormat("Starting display: {0}", currentWord);

		// populate the dictionary and compensate for RTL languages
		foreach (string pw in _translation.Language.PossibleWords) {
			passwordLikelihood.Add(pw, -1);
		}
		if (rtl) {
			keys = new List<string>(passwordLikelihood.Keys);
			foreach (string pw in keys) {
				passwordLikelihood.Add(_translation.Language.ReverseReadingDirection(pw), -1);
				passwordLikelihood.Remove(pw);
			}
		}

		// memorize the current letters on display
		for (int i = 0; i < 5; i++) {
			memorizedDials[i] = new List<char> { currentWord[i] };
		}

		// check if any of the dials are set to an unused letter (eg. X in English) and toggle them down if so
		for (int i = 0; i < 5; i++) {
			int connector = i;
			if (_translation.Language.RightToLeft) {
				connector = 4 - connector;
			}
			bool letterExistsInTable = false;
			while (!letterExistsInTable) {
				foreach (string pw in _translation.Language.PossibleWords) {
					if (pw[i] == currentWord[connector]) {
						letterExistsInTable = true;
						break;
					}
				}
				if (!letterExistsInTable) {
					Connector.TwitchMoveDown(connector);
					presses++;
					lastChangedDial = connector;
					currentWord = Connector.GetWord();
					//LogFormat("UNUSED: Current display: {0}", currentWord);
					memorizedDials[connector].Add(currentWord[connector]);
					yield return new WaitForSeconds(delay);
				}
			}
		}

		// count how many characters are already matching and pick the most likely word as the target
		string targetWord = "";
		int feasibilityScore = -1;
		keys = new List<string>(passwordLikelihood.Keys);
		foreach (string pw in keys) {
			int matchingLetters = CountMatches(currentWord, pw);
			if (matchingLetters > feasibilityScore) {
				targetWord = pw;
				feasibilityScore = matchingLetters;
			}
			passwordLikelihood[pw] = CountMatches(currentWord, pw);
		}
		//LogFormat("Aiming for '{0}' as it already has {1} matching characters", targetWord, feasibilityScore.ToString());

		while (presses < 50) {
			// change an incorrect letter in a column that we're not sure contains a letter from this password
			bool pressed = false;
			for (int i = 0; i < 5; i++) {
				if (!memorizedDials[i].Contains(targetWord[i])) {   // currentWord[i] != targetWord[i]
					Connector.TwitchMoveDown(i);
					lastChangedDial = i;
					presses++;
					pressed = true;
					currentWord = Connector.GetWord();
					//LogFormat("NOMATCH: Current display: {0}", currentWord);
					break;
				}
			}
			// all columns contain letters from this password. We know the right password, just need to enter it.
			if (!pressed) {
				for (int i = 0; i < 5; i++) {
					if (currentWord[i] == targetWord[i]) {
						continue;
					}
					int indexCurrent = Connector.GetSpinnerChoices(i).IndexOf(c => c == currentWord[i]);
					int indexTarget = Connector.GetSpinnerChoices(i).IndexOf(c => c == targetWord[i]);
					int difference = indexTarget - indexCurrent;
					if (difference < 0) difference += 6;
					if (difference < 3 ) {
						Connector.TwitchMoveDown(i);
					}
					else {
						Connector.TwitchMoveUp(i);
					}
					lastChangedDial = i;
					presses++;
					currentWord = Connector.GetWord();
					//LogFormat("CERTAIN: Current display: {0}", currentWord);
					break;
				}
			}
			yield return new WaitForSeconds(delay);

			// check if module is solved.
			if (_translation.Language.PossibleWords.Contains(Connector.GetWord(rtl))) {
				Connector.TwitchPressSubmit();
				//LogFormat("solved in {0} presses", presses.ToString());
				yield break;
			}

			// check if we've seen this letter before and if so, increase all passwords with a matching letter's likelihood
			if (!memorizedDials[lastChangedDial].Contains(currentWord[lastChangedDial])) {
				memorizedDials[lastChangedDial].Add(currentWord[lastChangedDial]);
				keys = new List<string>(passwordLikelihood.Keys);
				foreach (string pw in keys) {
					if (pw[lastChangedDial] == currentWord[lastChangedDial]) {
						passwordLikelihood[pw]++;
					}
				}
			}

			// check if we've seen every letter in this dial, and if so, clear out impossible words.
			if (memorizedDials[lastChangedDial].Count >= 6) {
				keys = new List<string>(passwordLikelihood.Keys);
				string pws = "";
				foreach (string pw in keys) {
					if (!memorizedDials[lastChangedDial].Contains(pw[lastChangedDial])) {
						passwordLikelihood.Remove(pw);
						pws += pw + ", ";
					}
				}
				//LogFormat("Excluding {0}as they have no matching letters in dial {1}.", pws, lastChangedDial.ToString());
			}

			// check what is now the most suitable word to go for.
			keys = new List<string>(passwordLikelihood.Keys);
			string oldTarget = targetWord;
			feasibilityScore = -1;
			foreach (string pw in keys) {
				int matchingLetters = CountMatches(currentWord, pw);
				if (matchingLetters + passwordLikelihood[pw] > feasibilityScore) {
					targetWord = pw;
					feasibilityScore = matchingLetters + passwordLikelihood[pw];
				}
			}
			if (oldTarget != targetWord) {
				//LogFormat("Now aiming for '{0}' as it has {1} currently matching characters and at least {2} known to exist in the dials.", targetWord, CountMatches(currentWord, targetWord).ToString(), passwordLikelihood[targetWord].ToString());
			}
		}
	}

	#endregion

}
