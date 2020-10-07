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

	public override void Start() {
		base.Start();
		this.Connector.KMBombModule.OnActivate = this.KMBombModule_OnActivate;
		this.Connector.SubmitPressed += this.Connector_SubmitPressed;
		this.Connector.SubmitReleased += this.Connector_SubmitReleased;

		var letters = new char[26];
		for (int i = 0; i < 26; ++i) letters[i] = (char) ('A' + i);
		letters.Shuffle();

		var choices = new char[5];
		for (int i = 0; i < 5; ++i) {
			Array.Copy(letters, i * 5, choices, 0, 5);
			this.Connector.SetSpinnerChoices(i, choices);
		}

	}

	private void KMBombModule_OnActivate() {
		this.Connector.Activate();
	}

	private void Connector_SubmitPressed(object sender, EventArgs e) {
		// solve the module
	}

	private void Connector_SubmitReleased(object sender, EventArgs e) {
		// do nothing
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
