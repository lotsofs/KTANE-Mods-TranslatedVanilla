using System;
using System.Collections;
using System.Linq;
using NotVanillaModulesLib;
using KModkit;
using UnityEngine;
using Random = UnityEngine.Random;

public class NotButton : NotVanillaModule<NotButtonConnector> {
	public delegate int MashCountFormula(int a, int b, int c, int d, int e, int f, int g);

	public bool OpenCoverOnSelection;

	public ButtonColour Colour { get; set; }
	public ButtonLabel Label { get; set; }
	public ButtonLightColour LightColour { get; private set; }

	public bool Down { get; private set; }
	public bool Holding { get; private set; }
	public bool Mashing { get; private set; }
	public float InteractionTime { get; private set; }

	private KMSelectable button;
	private KMBombInfo bombInfo;

	public bool ShouldBeHeld () {
		if (this.Colour == ButtonColour.Blue && this.Label == ButtonLabel.Abort) return true;
		if (bombInfo.GetBatteryCount() > 1 && this.Label == ButtonLabel.Detonate) return false;
		if (this.Colour == ButtonColour.White && bombInfo.GetOnIndicators().Contains("CAR")) return true;
		if (bombInfo.GetBatteryCount() > 2 && bombInfo.GetOnIndicators().Contains("FRK")) return false;
		if (this.Colour == ButtonColour.Yellow) return true;
		if (this.Colour == ButtonColour.Red && this.Label == ButtonLabel.Hold) return false;
		else return true;
	}

	public override void Start () {
		base.Start();
		this.bombInfo = this.GetComponent<KMBombInfo>();

		// Sets the appearance of the button
		this.Connector.SetColour(this.Colour = (ButtonColour) Random.Range(0, 4));
		this.Log("Colour is " + this.Colour);
		this.Connector.SetLabel(this.Label = (ButtonLabel) Random.Range(0, 4));
		this.Log("Label is " + this.Label);
		this.Log("The button should be " + (ShouldBeHeld() ? "Held." : "Pressed and immediately released."));

		// Register button hold and released events
		this.Connector.Held += this.Button_In;
		this.Connector.Released += this.Button_Out;

		// Stuff regarding the cover
		var moduleSelectable = this.GetComponent<KMSelectable>();
		this.button = moduleSelectable.Children[0];
		moduleSelectable.OnHighlight = () => { if (this.OpenCoverOnSelection) this.Connector.OpenCover(); };
		this.button.OnHighlight = () => { if (this.OpenCoverOnSelection) this.Connector.OpenCover(); };
		moduleSelectable.OnHighlightEnded = () => { if (this.OpenCoverOnSelection) this.Connector.CloseCover(); };
		this.button.OnHighlightEnded = () => { if (this.OpenCoverOnSelection) this.Connector.CloseCover(); };
		moduleSelectable.OnCancel = () => { this.Connector.CloseCover(); return true; };  // Twitch Plays
		if (Application.isEditor) {
			// Things work a bit differently in the test harness from the actual game.
			// TODO: There is currently an issue whereby going from one module to another does not call any event.
			moduleSelectable.OnInteract = () => { if (!this.OpenCoverOnSelection) this.Connector.OpenCover(); return true; };
		} else {
			this.OpenCoverOnSelection = this.Connector.ShouldOpenCoverOnSelection;
			moduleSelectable.OnFocus = () => { if (!this.OpenCoverOnSelection) this.Connector.OpenCover(); };
			moduleSelectable.OnDefocus = () => this.Connector.CloseCover();
		}
	}

	public void Update() {
		if (this.Down) {	// button being pressed
			if (!this.Holding) {	// button not counted as held yet
				this.InteractionTime += Time.deltaTime;
				if (this.InteractionTime >= 0.5f) {
					this.StartedHolding();
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
		this.Down = true;
		this.InteractionTime = 0;
		this.GetComponent<KMSelectable>().AddInteractionPunch(0.5f);
	}

	/// <summary>
	/// Done as soon as the button is released
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Button_Out(object sender, EventArgs e) {
		this.GetComponent<KMSelectable>().AddInteractionPunch(-0.35f);
		// There's an issue with the test harness whereby pressing Enter to select the module raises this event.
		if (!this.Down) return;
		
		this.Down = false;
		this.Connector.SetLightColour(ButtonLightColour.Off); 

		if (this.Holding) {
			this.Holding = false;
			this.InteractionTime = 0;

			var timeString = this.GetComponent<KMBombInfo>().GetFormattedTime();
			if (this.ShouldBeHeld()) {
				char solution;
				switch (this.LightColour) {
					case ButtonLightColour.Blue: solution = '4'; break;
					case ButtonLightColour.White: solution = '1'; break;
					case ButtonLightColour.Yellow: solution = '5'; break;
					default: solution = '1'; break;
				}
				var time = this.GetComponent<KMBombInfo>().GetFormattedTime();
				if (time.Contains(solution)) {
					this.Log(string.Format("The button was held and released at {0}. That was correct.", timeString));
					this.Disarm();
				}
				else {
					this.Log(string.Format("The button was held and released at {0}. That was incorrect, as {0} does not contain a {1}.", timeString, solution));
					this.Connector.KMBombModule.HandleStrike();
				}
			} else {
				this.Log(string.Format("The button was held and released at {0}. That was incorrect: It should have been pressed and immediately released.", timeString));
				this.Connector.KMBombModule.HandleStrike();
			}
		}
		else {
			if (this.ShouldBeHeld()) {
				this.Log(string.Format("The button was pressed and immediately released. That was incorrect: It should have been held."));
				this.Connector.KMBombModule.HandleStrike();
			}
			else {
				this.Log(string.Format("The button was pressed and immediately released. That was correct."));
				this.Disarm();
			}
		}
	}

	/// <summary>
	/// The button is being held instead of immediately released
	/// </summary>
	private void StartedHolding() {
		this.Holding = true;
		this.Connector.SetLightColour(this.LightColour = (ButtonLightColour)(Random.Range(0, 4) + 1));
		this.Log(string.Format(this.ShouldBeHeld() == true ? "The button is being held. That is correct. The light is {0}." :
			"The button is being held. That is incorrect. The light is {0}.", this.LightColour));
	}

	#region twitch plays

	public static readonly string TwitchHelpMessage = "!{0} tap | !{0} hold | !{0} release 1:13 1:23 | !{0} mash 50";
	[NonSerialized]
	public bool ZenModeActive;
	[NonSerialized]
	public bool TwitchShouldCancelCommand;

	public IEnumerator ProcessTwitchCommand(string command) {
		if (this.TwitchColourblindModeCommand(command)) { yield return null; yield break; }

		var tokens = command.Split(new[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
		if (tokens.Length == 0) yield break;

		if (tokens[0].Equals("release", StringComparison.InvariantCultureIgnoreCase)) {
			if (!this.Down)
				yield return "sendtochaterror You must start holding the button first.";
			else if (tokens.Length > 1) {
				var time = this.ZenModeActive ? float.PositiveInfinity : float.NegativeInfinity;
				var bombTime = this.bombInfo.GetTime();
				float time2;
				foreach (var timeString in tokens.Skip(1)) {
					if (!GeneralExtensions.TryParseTime(timeString, out time2)) yield break;
					if (this.ZenModeActive ? (time2 < time && time2 > bombTime) : (time2 > time && time2 < bombTime))
						time = time2;
				}
				if (float.IsInfinity(time))
					yield return tokens.Length == 2 ? "sendtochaterror The specified time has already passed."
						: "sendtochaterror All of the specified times have already passed.";
				else {
					var timeInt = (int) time;
					this.Log(string.Format("Releasing the button at {0}", GeneralExtensions.FormatTime(time)));
					yield return null;
					if (Math.Abs(time - this.bombInfo.GetTime()) > 15) yield return "waiting music";

					while (timeInt != (int) this.bombInfo.GetTime()) {
						yield return "trycancel The button was not released due to a request to cancel.";
					}

					var wasHolding = this.Holding;
					this.Connector.TwitchRelease();
					if (!wasHolding) {
						// The button was held so briefly it didn't count as such... Not sure this is actually possible.
						yield return this.ShouldBeHeld() == false ? "solve" : "strike";
					}
				}
			}
		} 
		else if (this.Down)
			yield return "sendtochaterror The button is already being held.";
		else {
			if (tokens[0].Equals("press", StringComparison.InvariantCultureIgnoreCase) || tokens[0].Equals("tap", StringComparison.InvariantCultureIgnoreCase)) {
				this.Connector.TwitchPress();
				yield return new WaitForSeconds(1 / 12f);
				this.Connector.TwitchRelease();
				yield return new WaitForSeconds(1 / 12f);
			}
			else if (tokens[0].Equals("hold", StringComparison.InvariantCultureIgnoreCase)) {
				yield return null;
				this.Connector.TwitchPress();
				yield return new WaitForSeconds(1);
			}
		}
	}

	public IEnumerator TwitchHandleForcedSolve() {
		this.Connector.OpenCover();
		switch (this.ShouldBeHeld()) {
			case false:
				this.Connector.TwitchPress();
				yield return new WaitForSeconds(1 / 12f);
				this.Connector.TwitchRelease();
				break;
			case true:
				this.Connector.TwitchPress();
				yield return new WaitForSeconds(1);
				this.Connector.TwitchRelease();
				break;
			default:
				this.Disarm();
				break;
		}
		this.Connector.CloseCover();
	}

	#endregion
}
