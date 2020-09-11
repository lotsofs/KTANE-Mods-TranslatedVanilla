using System;
using System.Collections;
using System.Linq;
using NotVanillaModulesLib;
using KModkit;
using UnityEngine;
using Random = UnityEngine.Random;

public class NotButton : NotVanillaModule<NotButtonConnector> {
	public delegate int MashCountFormula(int a, int b, int c, int d, int e, int f, int g);

	public Light[] Lights;
	public bool OpenCoverOnSelection;

	public ButtonColour Colour { get; set; }
	public ButtonLabel Label { get; set; }
	public ButtonLightColour LightColour { get; private set; }

	public bool Down { get; private set; }
	public bool Holding { get; private set; }
	public bool Mashing { get; private set; }
	public float InteractionTime { get; private set; }

	public bool ShouldBeHeld { get; private set; }

	private KMSelectable button;
	private KMBombInfo bombInfo;
	private Coroutine animationCoroutine;

	public override void Start () {
		base.Start();
		this.bombInfo = this.GetComponent<KMBombInfo>();

		// Fixes lights.
		var lightScale = this.transform.lossyScale.x;
		foreach (var light in this.Lights) light.range *= lightScale;

		// Sets the appearance of the button
		this.Connector.SetColour(this.Colour = (ButtonColour) Random.Range(0, 4));
		this.Log("Colour is " + this.Colour);
		this.Connector.SetLabel(this.Label = (ButtonLabel) Random.Range(0, 4));
		this.Log("Label is " + this.Label);
		this.ShouldBeHeld = this.Label != ButtonLabel.Detonate;
		this.Log("The button should be " + (ShouldBeHeld ? "Held." : "Pressed."));		// todo: Actual puzzle logic

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
				if (this.InteractionTime >= 0.7f) {
					this.StartedHolding();
				}
			}
		}
		else {
			this.InteractionTime += Time.deltaTime;
			if (this.ShouldBeHeld) {
				this.Log(string.Format("The button was pressed. That was incorrect: it should have been held."));
				this.Connector.KMBombModule.HandleStrike();
			}
			else {
				this.Log("The button was pressed. That was correct.");
				this.Disarm();
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
		this.SetLightColour(ButtonLightColour.Off);

		if (this.Holding) {
			this.Holding = false;
			this.InteractionTime = 0;
			if (this.animationCoroutine != null) {
				this.StopCoroutine(this.animationCoroutine);
				this.animationCoroutine = null;
			}

			var timeString = this.GetComponent<KMBombInfo>().GetFormattedTime();
			if (this.ShouldBeHeld) {
				var time = this.GetComponent<KMBombInfo>().GetTime();
					this.Log(string.Format("The button was held and released at {0}. That was correct.", timeString));
					this.Disarm();
			} else {
				this.Log(string.Format("The button was held and released at {0}. That was incorrect: It should have been pressed.", timeString));
				this.Connector.KMBombModule.HandleStrike();
			}
		}
		else {
			if (this.ShouldBeHeld) {
				this.Log(string.Format("The button was pressed and immediately released. That was correct."));
				this.Disarm();
			}
			else {
				this.Log(string.Format("The button was pressed and immediately released. That was incorrect: It should have been held."));
				this.Connector.KMBombModule.HandleStrike();
			}
		}
	}

	/// <summary>
	/// The button is being held instead of immediately released
	/// </summary>
	private void StartedHolding() {
		this.Holding = true;
		this.SetLightColour((ButtonLightColour)(Random.Range(0, 4) + 1));
		this.animationCoroutine = this.StartCoroutine(this.LightAnimationCoroutine());
		this.Log(string.Format(this.ShouldBeHeld == true ? "The button is being held. That is correct. The light is {0}." :
			"The button is being held. That is incorrect. The light is {0}.", this.LightColour));
	}

	/// <summary>
	/// Turn on the colored strip
	/// </summary>
	/// <param name="colour"></param>
	private void SetLightColour(ButtonLightColour colour) {
		this.Connector.SetLightColour(this.LightColour = colour);
		if (colour == ButtonLightColour.Off) {
			foreach (var light in this.Lights)
				light.gameObject.SetActive(false);
		} 
		else {
			foreach (var light in this.Lights) { 
				light.gameObject.SetActive(true);
				switch (colour) {
					case ButtonLightColour.Red: light.color = Color.red; break;
					case ButtonLightColour.Yellow: light.color = Color.yellow; break;
					case ButtonLightColour.Blue: light.color = Color.blue; break;
					case ButtonLightColour.White: light.color = Color.white; break;
				}
			}
		}
	}

	/// <summary>
	/// Change the brightness of the strip
	/// </summary>
	/// <param name="brightness"></param>
	private void SetLightBrightness(float brightness) {
		this.Connector.SetLightBrightness(brightness);
		foreach (var light in this.Lights) light.intensity = brightness * 2;
	}

	private IEnumerator LightAnimationCoroutine() {
		float time = 0;
		while (true) {
			var r = time % 1.4f;
			this.SetLightBrightness(0.79f + 0.3f * Math.Abs(r - 0.7f));
			yield return null;
			time += Time.deltaTime;
		}
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
						yield return this.ShouldBeHeld == false ? "solve" : "strike";
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
		switch (this.ShouldBeHeld) {
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
