using System;
using System.Collections;
using System.Linq;
using NotVanillaModulesLib;
using KModkit;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Globalization;

public class NotButton : NotVanillaModule<NotButtonConnector> {
	public bool OpenCoverOnSelection;

	ButtonColour _color;
	ButtonLabel _label;
	ButtonLightColour _lightColor;

	bool _pressed;
	bool _holding;
	float _interactionTime;

	KMSelectable _buttonSelectable;
	KMBombInfo _bombInfo;
	KMGameInfo _gameInfo;
	TranslatedTheButton _translation;

	public bool ShouldBeHeld () {
		if (_color == ButtonColour.Blue && _label == ButtonLabel.Abort) return true;
		if (_bombInfo.GetBatteryCount() > 1 && _label == ButtonLabel.Detonate) return false;
		if (_color == ButtonColour.White && _bombInfo.GetOnIndicators().Contains("CAR")) return true;
		if (_bombInfo.GetBatteryCount() > 2 && _bombInfo.GetOnIndicators().Contains("FRK")) return false;
		if (_color == ButtonColour.Yellow) return true;
		if (_color == ButtonColour.Red && _label == ButtonLabel.Hold) return false;
		else return true;
	}

	public override void Start () {
		base.Start();
		_gameInfo = GetComponent<KMGameInfo>();
		_bombInfo = GetComponent<KMBombInfo>();
		_translation = GetComponent<TranslatedTheButton> ();
		LanguageTheButton language = _translation.Language;

		// Sets the appearance of the button
		Connector.SetColour(_color = (ButtonColour) Random.Range(0, 4));
		_label = (ButtonLabel) Random.Range(0,4);

		if (_translation.Language.DisplayMethod == LanguageTheButton.DisplayMethods.CustomTextMesh) {
			Connector.SetLabel(language.GetLabelFromEnglishName(_label.ToString()), language.Font, language.FontMaterial, language.GetSizeFromEnglishName(_label.ToString()));
		}
		else {
			Connector.SetLabel(language.GetLabelFromEnglishName(_label.ToString()));
		}
		
		LogFormat(language.RuleColorIs, language.GetLogFromEnglishName(_color.ToString()));
		LogFormat(language.RuleLabelIs, language.GetLogFromEnglishName(_label.ToString()));
		LogFormat(language.RuleButtonShouldBe, (ShouldBeHeld() ? language.RuleHeld : language.RulePressed));

		// Register button hold and released events
		Connector.Held += Button_In;
		Connector.Released += Button_Out;

		// If the game uses a fallback font, the label will shine in the dark. So we do it manually.
		if (_translation.Language.DisplayMethod != LanguageTheButton.DisplayMethods.Default) {
			if (_color == ButtonColour.Blue || _color == ButtonColour.Red) {
				_gameInfo.OnLightsChange += Connector.ToggleLabel;
				Connector.ToggleLabel(false);
			}
		}

		// Stuff regarding the cover
		var moduleSelectable = GetComponent<KMSelectable>();
		_buttonSelectable = moduleSelectable.Children[0];
		moduleSelectable.OnHighlight = () => { if (OpenCoverOnSelection) Connector.OpenCover(); };
		_buttonSelectable.OnHighlight = () => { if (OpenCoverOnSelection) Connector.OpenCover(); };
		moduleSelectable.OnHighlightEnded = () => { if (OpenCoverOnSelection) Connector.CloseCover(); };
		_buttonSelectable.OnHighlightEnded = () => { if (OpenCoverOnSelection) Connector.CloseCover(); };
		moduleSelectable.OnCancel = () => { Connector.CloseCover(); return true; };  // Twitch Plays
		if (Application.isEditor) {
			// Things work a bit differently in the test harness from the actual game.
			// TODO: There is currently an issue whereby going from one module to another does not call any event.
			moduleSelectable.OnInteract = () => { if (!OpenCoverOnSelection) Connector.OpenCover(); return true; };
		} else {
			OpenCoverOnSelection = Connector.ShouldOpenCoverOnSelection;
			moduleSelectable.OnFocus = () => { if (!OpenCoverOnSelection) Connector.OpenCover(); };
			moduleSelectable.OnDefocus = () => Connector.CloseCover();
		}
	}

	public void Update() {
		if (_pressed) {	// button being pressed
			if (!_holding) {	// button not counted as held yet
				_interactionTime += Time.deltaTime;
				if (_interactionTime >= 0.5f) {
					StartedHolding();
				}
			}
		}
	}

	/// <summary>
	/// Done as soon as the button is pressed in.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Button_In(object sender, EventArgs e) {
		_pressed = true;
		_interactionTime = 0;
		GetComponent<KMSelectable>().AddInteractionPunch(0.5f);
	}

	/// <summary>
	/// Done as soon as the button is released
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Button_Out(object sender, EventArgs e) {
		GetComponent<KMSelectable>().AddInteractionPunch(-0.35f);
		// There's an issue with the test harness whereby pressing Enter to select the module raises this event.
		if (!_pressed) return;
		
		_pressed = false;
		Connector.SetLightColour(ButtonLightColour.Off); 

		if (_holding) {
			_holding = false;
			_interactionTime = 0;

			var timeString = GetComponent<KMBombInfo>().GetFormattedTime();
			if (ShouldBeHeld()) {
				char solution;
				switch (_lightColor) {
					case ButtonLightColour.Blue: solution = '4'; break;
					case ButtonLightColour.White: solution = '1'; break;
					case ButtonLightColour.Yellow: solution = '5'; break;
					default: solution = '1'; break;
				}
				var time = GetComponent<KMBombInfo>().GetFormattedTime();
				if (time.Contains(solution)) {
					LogFormat(_translation.Language.LogHeldCorrect, timeString);
					Disarm();
				}
				else {
					LogFormat(_translation.Language.LogHeldIncorrectRelease, timeString, solution.ToString());
					Connector.KMBombModule.HandleStrike();
				}
			} else {
				LogFormat(_translation.Language.LogHeldIncorrect, timeString);
				Connector.KMBombModule.HandleStrike();
			}
		}
		else {
			if (ShouldBeHeld()) {
				LogFormat(_translation.Language.LogPressedIncorrect);
				Connector.KMBombModule.HandleStrike();
			}
			else {
				LogFormat(_translation.Language.LogPressedCorrect);
				Disarm();
			}
		}
	}

	/// <summary>
	/// The button is being held instead of immediately released
	/// </summary>
	private void StartedHolding() {
		_holding = true;
		Connector.SetLightColour(_lightColor = (ButtonLightColour)(Random.Range(0, 4) + 1));
		LogFormat(ShouldBeHeld() ? _translation.Language.LogHoldingCorrect : _translation.Language.LogHoldingIncorrect, _translation.Language.GetLogFromEnglishName(_lightColor.ToString()));
	}

	#region twitch plays

	public static readonly string TwitchHelpMessage = "!{0} tap | !{0} hold | !{0} release 1:13 1:23";
	[NonSerialized]
	public bool TwitchShouldCancelCommand;

	public IEnumerator ProcessTwitchCommand(string command) {
		if (TwitchColourblindModeCommand(command)) { yield return null; yield break; }

		var tokens = command.Split(new[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
		if (tokens.Length == 0) yield break;

		if (tokens[0].Equals("release", StringComparison.InvariantCultureIgnoreCase)) {
			if (!_pressed)
				yield return "sendtochaterror You must start holding the button first.";

			else if (tokens.Length == 2) {
				var bombTime = _bombInfo.GetTime();
				float time2;
				if (tokens[1].Length != 1 || !char.IsDigit(tokens[1][0])) {
					yield break;
				}
				while (!_bombInfo.GetFormattedTime().Contains(tokens[1][0])) {
					yield return "trycancel Aborting release.";
				}
				Connector.TwitchRelease();

			}
		}
		else if (_pressed)
			yield return "sendtochaterror The button is already being held.";
		else {
			if (tokens[0].Equals("press", StringComparison.InvariantCultureIgnoreCase) || tokens[0].Equals("tap", StringComparison.InvariantCultureIgnoreCase)) {
				Connector.TwitchPress();
				yield return new WaitForSeconds(1 / 12f);
				Connector.TwitchRelease();
			}
			else if (tokens[0].Equals("hold", StringComparison.InvariantCultureIgnoreCase)) {
				yield return null;
				Connector.TwitchPress();
				yield return new WaitForSeconds(0.5f);
			}
		}
	}

	public IEnumerator TwitchHandleForcedSolve() {
		Connector.OpenCover();
		if (_holding && !ShouldBeHeld()) {
			// uh oh, module is already being held when it should be pressed. Use cheats.
			_holding = false;
			_interactionTime = 0;
			Connector.SetLightColour(0);
			yield return new WaitForSeconds(0.4f);
			Connector.TwitchRelease();
		}
		else if (ShouldBeHeld()) {
			if (!_pressed) {
				Connector.TwitchPress();
			}
			while (!_holding) {
				yield return true;
			}
			char solution;
			switch (_lightColor) {
				case ButtonLightColour.Blue: solution = '4'; break;
				case ButtonLightColour.White: solution = '1'; break;
				case ButtonLightColour.Yellow: solution = '5'; break;
				default: solution = '1'; break;
			}
			while (!_bombInfo.GetFormattedTime().Contains(solution)) {
				yield return true;
			}
			Connector.TwitchRelease();
		}
		else {
			Connector.TwitchPress();
			yield return new WaitForSeconds(0.1f);
			Connector.TwitchRelease();
		}
		Connector.CloseCover();
	}

	#endregion
}
