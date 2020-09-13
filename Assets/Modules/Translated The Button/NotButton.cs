using System;
using System.Collections;
using System.Linq;
using NotVanillaModulesLib;
using KModkit;
using UnityEngine;
using Random = UnityEngine.Random;

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
	TranslatedModule _translation;

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
		_bombInfo = GetComponent<KMBombInfo>();
		_translation = GetComponent<TranslatedModule>();
		Translation language = _translation.Language;

		// Sets the appearance of the button
		Connector.SetColour(_color = (ButtonColour) Random.Range(0, 4));
		_label = (ButtonLabel) Random.Range(0,4);
		Connector.SetLabel(language.GetLabelFromEnglishName(_label.ToString()));
		
		LogFormat(language.RuleColorIs, language.GetLogFromEnglishName(_color.ToString()));
		LogFormat(language.RuleLabelIs, language.GetLogFromEnglishName(_label.ToString()));
		LogFormat(language.RuleButtonShouldBe, (ShouldBeHeld() ? language.RuleHeld : language.RulePressed));

		// Register button hold and released events
		Connector.Held += Button_In;
		Connector.Released += Button_Out;

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

	public static readonly string TwitchHelpMessage = "!{0} tap | !{0} hold | !{0} release 1:13 1:23 | !{0} mash 50";
	[NonSerialized]
	public bool ZenModeActive;
	[NonSerialized]
	public bool TwitchShouldCancelCommand;

	public IEnumerator ProcessTwitchCommand(string command) {
		if (TwitchColourblindModeCommand(command)) { yield return null; yield break; }

		var tokens = command.Split(new[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
		if (tokens.Length == 0) yield break;

		if (tokens[0].Equals("release", StringComparison.InvariantCultureIgnoreCase)) {
			if (!_pressed)
				yield return "sendtochaterror You must start holding the button first.";
			else if (tokens.Length > 1) {
				var time = ZenModeActive ? float.PositiveInfinity : float.NegativeInfinity;
				var bombTime = _bombInfo.GetTime();
				float time2;
				foreach (var timeString in tokens.Skip(1)) {
					if (!GeneralExtensions.TryParseTime(timeString, out time2)) yield break;
					if (ZenModeActive ? (time2 < time && time2 > bombTime) : (time2 > time && time2 < bombTime))
						time = time2;
				}
				if (float.IsInfinity(time))
					yield return tokens.Length == 2 ? "sendtochaterror The specified time has already passed."
						: "sendtochaterror All of the specified times have already passed.";
				else {
					var timeInt = (int) time;
					Log(string.Format("Releasing the button at {0}", GeneralExtensions.FormatTime(time)));
					yield return null;
					if (Math.Abs(time - _bombInfo.GetTime()) > 15) yield return "waiting music";

					while (timeInt != (int) _bombInfo.GetTime()) {
						yield return "trycancel The button was not released due to a request to cancel.";
					}

					var wasHolding = _holding;
					Connector.TwitchRelease();
					if (!wasHolding) {
						// The button was held so briefly it didn't count as such... Not sure this is actually possible.
						yield return ShouldBeHeld() == false ? "solve" : "strike";
					}
				}
			}
		} 
		else if (_pressed)
			yield return "sendtochaterror The button is already being held.";
		else {
			if (tokens[0].Equals("press", StringComparison.InvariantCultureIgnoreCase) || tokens[0].Equals("tap", StringComparison.InvariantCultureIgnoreCase)) {
				Connector.TwitchPress();
				yield return new WaitForSeconds(1 / 12f);
				Connector.TwitchRelease();
				yield return new WaitForSeconds(1 / 12f);
			}
			else if (tokens[0].Equals("hold", StringComparison.InvariantCultureIgnoreCase)) {
				yield return null;
				Connector.TwitchPress();
				yield return new WaitForSeconds(1);
			}
		}
	}

	public IEnumerator TwitchHandleForcedSolve() {
		Connector.OpenCover();
		switch (ShouldBeHeld()) {
			case false:
				Connector.TwitchPress();
				yield return new WaitForSeconds(1 / 12f);
				Connector.TwitchRelease();
				break;
			case true:
				Connector.TwitchPress();
				yield return new WaitForSeconds(1);
				Connector.TwitchRelease();
				break;
			default:
				Disarm();
				break;
		}
		Connector.CloseCover();
	}

	#endregion
}
