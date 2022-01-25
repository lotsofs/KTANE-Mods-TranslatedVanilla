using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TranslatedVanillaModulesLib;
using UnityEngine;

public class WhosOnFirst : TranslatedVanillaModule<TranslatedMemoryConnector> {
	TranslatedWhosOnFirst _translation;
	
	private static readonly int[] step1Solutions = new[] { 
		2, 1, 5, 1, 5, 2,	// 0 - 5
		4, 3, 5, 2, 5, 3,	// 6 - 11
		3, 4, 4, 5, 3, 5,	// 12 - 17
		3, 3, 0, 5, 4, 3,	// 18 - 23
		   2, 5, 1, 5		// 24 - 27
	};	

	private static readonly int[,] step2Solutions = new[,] {
		{ 5, 11,  6, 10,  8, 13,  9,  3,  0,  2,  1,  7,  4, 12 }, //  0
		{ 8, 11,  5, 10,  2,  9,  4,  7, 12,  0,  3,  6, 13,  1	}, //  1
		{ 3,  7, 12,  1,  6,  0,  9,  5,  4,  8, 13, 11,  2, 10	}, //  2
		{12,  9, 11, 10,  3, 13,  0,  4,  2,  6,  8,  7,  5,  1	}, //  3
		{ 7,  9, 11, 10,  5,  3,  2, 13,  8,  6, 12,  1,  4,  0	}, //  4
		{11,  9,  7, 10,  1,  6, 13,  0,  4,  5,  8,  3,  2, 12	}, //  5
		{ 7,  6,  8,  4,  0,  3, 10,  2, 11,  1, 12,  5, 13,  9	}, //  6
		{ 0,  4,  8,  6, 11,  5,  9,  2, 13,  3,  7, 10, 12,  1	}, //  7
		{ 9,  8,  1,  2, 10,  5,  3,  6,  7, 12, 13,  0, 11,  4	}, //  8
		{ 5,  4,  0, 13,  2, 12,  6,  9, 10,  8,  7,  3, 11,  1	}, //  9
		{ 3,  0, 11,  6,  4, 13,  2, 12,  8, 10,  9,  1,  7,  5	}, // 10
		{10,  2,  1,  5,  7,  4, 12, 11,  8,  0,  3, 13,  6,  9	}, // 11
		{ 7,  2,  3, 11,  5,  8,  1, 13,  6, 12,  4,  0,  9, 10	}, // 12
		{ 9, 10,  5,  0, 13, 11,  4,  7,  3,  8,  1,  6,  2, 12	}, // 13
		{26, 15, 16, 17, 24, 20, 18, 25, 22, 14, 21, 27, 23, 19	}, // 14
		{16, 24, 27, 20, 22, 23, 21, 25, 14, 19, 17, 26, 18, 15	}, // 15
		{21, 15, 20, 16, 24, 18, 26, 19, 17, 14, 22, 25, 27, 23	}, // 16
		{14, 17, 18, 24, 21, 15, 19, 16, 22, 20, 26, 23, 27, 25	}, // 17
		{23, 19, 18, 20, 22, 26, 16, 25, 17, 27, 24, 21, 15, 14	}, // 18
		{20, 26, 24, 22, 17, 18, 21, 23, 19, 14, 27, 25, 15, 16	}, // 19
		{20, 16, 15, 14, 23, 25, 21, 24, 26, 27, 17, 18, 19, 22	}, // 20
		{18, 19, 15, 17, 24, 21, 23, 14, 20, 27, 16, 26, 25, 22	}, // 21
		{14, 25, 17, 16, 19, 23, 21, 27, 15, 20, 18, 24, 22, 26	}, // 22
		{26, 20, 24, 22, 16, 18, 17, 25, 27, 14, 19, 15, 21, 23	}, // 23
		{22, 20, 21, 16, 25, 26, 24, 27, 23, 15, 18, 17, 19, 14	}, // 24
		{15, 19, 23, 21, 14, 18, 26, 22, 17, 24, 25, 20, 16, 27	}, // 25
		{15, 23, 27, 17, 14, 25, 20, 18, 26, 19, 22, 24, 16, 21	}, // 26
		{17, 24, 19, 18, 25, 23, 21, 22, 20, 14, 27, 26, 15, 16 }, // 27
	};

	string[] _usedDisplays = new string[3];
	int _completedStages = 0;
	int _solution;
	string[] _buttonLabels;
	bool _activated = false;

	public override void Start() {
		base.Start();
		string name = string.Format("{0} #{1}", Connector.KMBombModule.ModuleDisplayName, Connector.ModuleID);
		Connector.KMBombModule.OnActivate += () => { Connector.Activate(); _activated = true; };
		Connector.ButtonPressed += Connector_ButtonPressed;
		Connector.ButtonsSunk += Connector_ButtonsSunk;

		_translation = GetComponent<TranslatedWhosOnFirst>();
		_translation.GenerateLanguage(name);

		Connector.Stage = 0;

		if (_translation.Language.ScreenDisplayMethod == LanguageWhosOnFirst.DisplayMethods.CustomTextMesh) {
			Connector.UseCustomDisplay(_translation.Language.DisplayFontSize, _translation.Language.DisplayOffset);
		}
		if (_translation.Language.ButtonsDisplayMethod == LanguageWhosOnFirst.DisplayMethods.CustomTextMesh) {
			Connector.UseCustomButtonLabels(_translation.Language.ButtonsFontSize, _translation.Language.ButtonsOffset);
		}

		// todo: verify that this only happens after the lights ocme on and the display isn't visible before then when using a custom text mesh.
		//StartCoroutine(CycleLabels());
		NewStage();
	}

	IEnumerator CycleLabels() {
		int a = UnityEngine.Random.Range(0, 28);
		while (true) {
			a++;
			//Connector.DisplayText = a + " " + _translation.Language.Displays[a % 28];
			Connector.DisplayText = _translation.Language.Displays[a % 28];
			for (int i = 0; i < 6; i++) {
				Connector.SetButtonLabel(i, _translation.Language.Labels[(a + i) % 28], _translation.Language.PerLabelButtonFontSizes[(a + i) % 28]);
			}
			yield return new WaitForSeconds(1f);
		}
	}

	private void Connector_ButtonsSunk(object sender, EventArgs e) {
		NewStage();
	}

	private void Connector_ButtonPressed(object sender, KeypadButtonEventArgs e) {
		if (_completedStages >= 3) {
			// strike when pressing a button after a solve, like in vanilla. Even the correct button strikes.
			Connector.KMBombModule.HandleStrike();
			Log("Strike!");
			return;
		}
		if (!_activated) {
			return;
		}
		if (_solution == e.ButtonIndex) { 
			// correct
			Log("Pressed {0}. Correct!", _buttonLabels[e.ButtonIndex]);
			_completedStages++;
			Connector.Stage = _completedStages;
			if (_completedStages >= 3) {
				Disarm();
			}
			else {
				Connector.AnimateButtons();
			}
		} 
		else {
			// wrong
			Log("Pressed {0}. Strike!", _buttonLabels[e.ButtonIndex]);
			Connector.KMBombModule.HandleStrike();
			Connector.AnimateButtons();
		}
	}

	private void NewStage() {
		Connector.Stage = _completedStages;

		// pick a display word
		int displayIndex = UnityEngine.Random.Range(0, _translation.Language.Displays.Length);
		Connector.DisplayText = _translation.Language.Displays[displayIndex];
		_usedDisplays[_completedStages] = _translation.Language.Displays[displayIndex];

		// pick whether to use the top or bottom list
		int listIndex = UnityEngine.Random.Range(0, 2) * 14;
		_buttonLabels = new string[14];
		Array.Copy(_translation.Language.Labels, listIndex, _buttonLabels, 0, 14);
		
		// pick the 6 buttons
		_buttonLabels.Shuffle();
		Array.Copy(_buttonLabels, _buttonLabels, 6);
		for (int i = 0; i < 6; ++i) {
			Connector.SetButtonLabel(i, _buttonLabels[i], _translation.Language.ButtonLabelSizes[_buttonLabels[i]]);
		}

		// figure out the reference button
		int referenceButtonIndex = step1Solutions[displayIndex];
		string referenceButtonLabel = _buttonLabels[referenceButtonIndex];
		int referenceLabelIndex = _translation.Language.Labels.IndexOf(l => l == referenceButtonLabel);

		int s = int.MaxValue;
		for (int i = 0; i < 14; i++) {
			int labelIndex = step2Solutions[referenceLabelIndex, i];
			string label = _translation.Language.Labels[labelIndex];
			s = _buttonLabels.IndexOf(l => l == label);
			if (s < 6) {
				_solution = s;
				break;
			}
		}
		Log(string.Format("Stage {0}", _completedStages + 1));
		Log(string.Format("Display is '{0}'", _translation.Language.Displays[displayIndex]));
		Log(string.Format("Labels are: '{0}', '{1}', '{2}', '{3}', '{4}', '{5}'", _buttonLabels[0], _buttonLabels[1], _buttonLabels[2], _buttonLabels[3], _buttonLabels[4], _buttonLabels[5]));
		Log(string.Format("Correct button to press: '{0}'", _buttonLabels[s]));
	}

	// Twitch Plays support
	public static readonly string TwitchHelpMessage
		= "!{0} what? - press the button that says 'WHAT?'; the phrase must match exactly | !{0} press 3 - press the third button in English reading order";
	public IEnumerator ProcessTwitchCommand(string command) {
		int n;
		command = command.Trim();
		if (command.StartsWith("press ", StringComparison.InvariantCultureIgnoreCase)) {
			if (!int.TryParse(command.Substring(6).TrimStart(), out n) || n < 1 || n > 6) yield break;
			yield return null;
			Connector.TwitchPress(n - 1);
		} else {
			//command = Regex.Replace(command, @"\s+", " ").Trim('\'', '"').ToUpperInvariant();
			//n = Array.IndexOf(buttonLabels, command);
			//if (n < 0) {
			//	yield return string.Format("sendtochaterror The label '{0}' is not present on the module.", command);
			//} else {
			//	yield return null;
			//	Connector.TwitchPress(n);
			//}
		}
	}

	public IEnumerator TwitchHandleForcedSolve() {
		while (!Solved) {
			if (Connector.Animating || !Connector.InputValid) {
				yield return true;
				continue;
			}
			Connector.TwitchPress(_solution);
		}
		yield break;
	}
}
