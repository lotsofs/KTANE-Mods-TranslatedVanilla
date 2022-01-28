using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TranslatedVanillaModulesLib;
using Random = UnityEngine.Random;
using System;
using System.Linq;

public class MorseCode : TranslatedVanillaModule<TranslatedMorseCodeConnector> {
	TranslatedMorseCode _translation;
	
	private static readonly int[] frequencies = new[] {
		505, 515, 522, 532, 
		535, 542, 545, 552, 
		555, 565, 572, 575, 
		582, 592, 595, 600
	};
	private Dictionary<char, Symbol[]> _codeTable = new Dictionary<char, Symbol[]>();

	private int _correctChannelIndex;
	private int _channelIndex;


	public int WordsCorrectlySubmitted { get; private set; }

	private Coroutine playWordCoroutine;
	private bool activated;

	private const float DotLength = 0.25f;

	private float holdTime;

	public override void Start () {
		base.Start();
		this.Connector.KMBombModule.OnActivate = this.KMBombModule_OnActivate;
		this.Connector.DownPressed += this.Connector_DownPressed;
		this.Connector.UpPressed += this.Connector_UpPressed;
		this.Connector.SubmitPressed += this.Connector_SubmitPressed;

		string name = string.Format("{0} #{1}", Connector.KMBombModule.ModuleDisplayName, Connector.ModuleID);
		_translation = GetComponent<TranslatedMorseCode>();
		_translation.GenerateLanguage(name);

		//if (_translation.Language.ButtonDisplayMethod == TranslatedMorseCode.DisplayMethods.CustomTextMesh) {
			//this.Connector.UseCustomButtonLabel();
		//}
		//this.Connector.SetButtonLabel(_translation.Language.Submit);

		// add all letters to morse conversions to the table from the language file.
		for (int i = 0; i < _translation.Language.Characters.Length; i++) {
			Symbol[] symbols = new Symbol[_translation.Language.MorseSymbols[i].Length];
			for (int c = 0; c < symbols.Length; c++) {
				char ch = _translation.Language.MorseSymbols[i][c];
				switch (ch) {
					case '.':
						symbols[c] = Symbol.Dot;
						break;
					case '-':
						symbols[c] = Symbol.Dash;
						break;
				}
			}
			_codeTable.Add(_translation.Language.Characters[i], symbols);
		}

		// select a word
		_correctChannelIndex = Random.Range(0, _translation.Language.PossibleWords.Length);
		string correctWord;
		if (_translation.Language.WordsManual.Length > 0) {
			correctWord = _translation.Language.WordsManual[_correctChannelIndex];
		}
		else {
			correctWord = _translation.Language.PossibleWords[_correctChannelIndex];
		}
		string signal = "";
		string solution = frequencies[_correctChannelIndex].ToString();
		foreach (char c in correctWord) {
			foreach (Symbol s in _codeTable[c]) {
				if (s == Symbol.Dash) signal += '-';
				else signal += '.';
			}
			signal += ' ';
		}
		Log(_translation.Language.LogAnswer, correctWord, signal, solution);

		this.ChangeChannel();
	}

	public override void Disarm() {
		base.Disarm();
		if (this.playWordCoroutine != null) {
			this.StopCoroutine(this.playWordCoroutine);
			this.Connector.SetLight(false);
			this.playWordCoroutine = null;
		}
	}

	private void Connector_SubmitPressed(object sender, EventArgs e) {
		// todo: press button animation and immediately release (holding isnt a thing in vanilla morse)

		if (this._channelIndex == _correctChannelIndex) {
			this.Log(_translation.Language.LogSubmitCorrect, _channelIndex);
			if (!Solved) this.Disarm();
		}
		else {
			this.Log(_translation.Language.LogSubmitWrong, _channelIndex);
			this.Connector.KMBombModule.HandleStrike();
		}
	}

	private void KMBombModule_OnActivate() {
		this.Connector.Activate();
		this.activated = true;
		if (!this.Solved) this.playWordCoroutine = this.StartCoroutine(this.PlayWord(_translation.Language.PossibleWords[_correctChannelIndex]));
	}

	private void Connector_DownPressed(object sender, EventArgs e) {
		if (this._channelIndex > 0) {
			--this._channelIndex;
			this.ChangeChannel();
		}
	}

	private void Connector_UpPressed(object sender, EventArgs e) {
		if (this._channelIndex < frequencies.Length - 1) {
			++this._channelIndex;
			this.ChangeChannel();
		}
	}

	private void ChangeChannel() {
		this.Connector.SetSlider(frequencies[this._channelIndex]);
		this.Connector.SetDisplay(frequencies[this._channelIndex].ToString());
	}

	public IEnumerator PlayWord(string word) {
		while (true) {
			foreach (var c in word) {
				var code = _codeTable[char.ToUpper(c)];
				foreach (var symbol in code) {
					this.Connector.SetLight(true);
					yield return new WaitForSeconds(symbol == Symbol.Dot ? DotLength : DotLength * 3);
					this.Connector.SetLight(false);
					yield return new WaitForSeconds(DotLength);
				}
				yield return new WaitForSeconds(DotLength * 3);  // 4 dots total
			}
			yield return new WaitForSeconds(DotLength * 6);  // 10 dots total
		}
	}

	#region tp

	// Twitch Plays support
	public static readonly string TwitchHelpMessage
		= "!{0} transmit 3.573, !{0} trans 573, !{0} tx 3.573 MHz, !{0} transmit 573 [transmit frequency 3.573]";

	public IEnumerator ProcessTwitchCommand(string command) {
		var tokens = command.Split(new[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
		if (tokens.Length == 0) yield break;
		switch (tokens[0].ToLowerInvariant()) {
			case "transmit": case "trans": case "tx":
				if (tokens.Length > 1) {
					var channels = new List<int>();
					foreach (var token in tokens.Skip(1)) {
						if (token.EqualsIgnoreCase("MHz")) continue;
						var freq = token;
						if (freq.StartsWith("3.")) freq = freq.Substring(2);
						int n;
						if (!int.TryParse(freq, out n)) yield break;
						
						var index = Array.IndexOf(frequencies, n);
						if (index < 0) {
							yield return string.Format("sendtochaterror 3.{0} MHz is not an available channel.", n);
							yield break;
						}
						channels.Add(index);
						
					}
					foreach (var index in channels) {
						foreach (var o in this.TwitchTuneTo(index)) yield return o;
						this.Connector.TwitchSubmit();
						yield return "trywaitcancel 0.1";
					}
				} else {
					yield return null;
					this.Connector.TwitchSubmit();
				}
				break;
		}
	}

	private IEnumerable<object> TwitchTuneTo(int channelIndex) {
		if (this._channelIndex < channelIndex) {
			do {
				this.Connector.TwitchMoveUp();
				yield return "trywaitcancel 0.1";
			} while (this._channelIndex < channelIndex);
		} else if (this._channelIndex > channelIndex) {
			do {
				this.Connector.TwitchMoveDown();
				yield return "trywaitcancel 0.1";
			} while (this._channelIndex > channelIndex);
		}
	}

	public IEnumerator TwitchHandleForcedSolve() {
		while (!this.Solved) {
			this.TwitchTuneTo(frequencies[_correctChannelIndex]);
			this.Connector.TwitchSubmit();
			yield return new WaitForSeconds(0.1f);
		}
	}

	#endregion

	private enum Symbol {
		Dot,
		Dash
	}
}
