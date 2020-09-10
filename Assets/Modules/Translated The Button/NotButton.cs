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

	public ButtonAction SolutionAction { get; private set; }

	const ButtonAction Press = ButtonAction.Press;
	const ButtonAction Hold = ButtonAction.Hold;
	private static readonly ButtonAction[,] defaultActionTable = new[,] {
		{ Press, Press , Hold , Press, Hold , Hold , Press, Press , Press },
		{ Press , Press, Press, Hold , Press , Press , Press , Press , Press  },
		{ Hold , Press, Press , Press , Press, Hold , Press, Press, Hold  },
		{ Press, Hold , Press, Press , Press , Hold , Press, Press, Press },
		{ Hold , Press , Press , Press, Hold , Press, Hold , Press, Press  },
		{ Press, Hold , Press, Press , Press, Hold , Press , Hold , Press },
		{ Press , Hold , Hold , Press, Press , Press , Hold , Press , Hold  },
		{ Press , Press, Hold , Press, Press, Press, Press , Hold , Press  },
		{ Press, Press , Press, Hold , Press , Press, Press, Press, Hold  },
		{ Hold , Hold , Press , Press , Press, Press , Hold , Press , Press  }
	};
	private static readonly MashCountFormula[] defaultMashCountFormulas = new MashCountFormula[] {
		(a, b, c, d, e, f, g) => a + 2 * b - d,
		(a, b, c, d, e, f, g) => 2 * b + 1 - g,
		(a, b, c, d, e, f, g) => 2 * a + d - c,
		(a, b, c, d, e, f, g) => d + 2 * f - b,
		(a, b, c, d, e, f, g) => e + f + g - b,
		(a, b, c, d, e, f, g) => 2 * c + d - 1,
		(a, b, c, d, e, f, g) => 2 * (f - a) + d,
		(a, b, c, d, e, f, g) => 3 * g - a - 3,
		(a, b, c, d, e, f, g) => (f + a * c) * (e + d),
		(a, b, c, d, e, f, g) => a * b + c * d - g * (e - f)
	};

	private KMSelectable button;
	private KMBombInfo bombInfo;
	private Coroutine animationCoroutine;

	private static string SolutionActionPastTense(ButtonAction action) {
		switch (action) {
			case ButtonAction.Press: return "pressed";
			case ButtonAction.Hold: return "held";
			default: throw new ArgumentException();
		}
	}

	public override void Start () {
		base.Start();
		this.bombInfo = this.GetComponent<KMBombInfo>();

		// Fix lights.
		var lightScale = this.transform.lossyScale.x;
		foreach (var light in this.Lights) light.range *= lightScale;

		this.Connector.SetColour(this.Colour = (ButtonColour) Random.Range(0, 10));
		this.Log("Colour is " + this.Colour);
		this.Connector.SetLabel(this.Label = (ButtonLabel) Random.Range(0, 9));
		this.Log("Label is " + this.Label);
		this.SolutionAction = defaultActionTable[(int) this.Colour, (int) this.Label];
		this.Log("The button should be " + SolutionActionPastTense(this.SolutionAction) + ".");

		this.Connector.Held += this.Button_Held;
		this.Connector.Released += this.Button_Released;

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

	public void OpenCover() { this.Connector.OpenCover(); }

	public void Update() {
		if (this.Down) {
			if (!this.Holding) {
				this.InteractionTime += Time.deltaTime;
				if (this.InteractionTime >= 0.7f) {
					this.StartedHolding();
				}
			}
		}
		else {
			this.InteractionTime += Time.deltaTime;
			if (this.SolutionAction == ButtonAction.Press) {
				this.Log("The button was pressed. That was correct.");
				this.Disarm();
			}
			else {
				this.Log(string.Format("The button was pressed. That was incorrect: it should have been {0}.", SolutionActionPastTense(this.SolutionAction)));
				this.Connector.KMBombModule.HandleStrike();
			}
		}
	}

	private void Button_Held(object sender, EventArgs e) {
		this.Down = true;
		this.InteractionTime = 0;
		this.GetComponent<KMSelectable>().AddInteractionPunch(0.5f);
	}

	private void Button_Released(object sender, EventArgs e) {
		this.GetComponent<KMSelectable>().AddInteractionPunch(-0.35f);
		// There's an issue with the test harness whereby pressing Enter to select the module raises this event.
		if (!this.Down) return;
		this.Down = false;
		this.SetLightColour(ButtonLightColour.Off);
		if (this.Solved) {
			if (this.Holding) {
				this.Holding = false;
				this.InteractionTime = 0;
				if (this.animationCoroutine != null) {
					this.StopCoroutine(this.animationCoroutine);
					this.animationCoroutine = null;
				}
			} else {

			}
			return;
		}

		if (this.Holding) {
			this.Holding = false;
			this.InteractionTime = 0;
			if (this.animationCoroutine != null) {
				this.StopCoroutine(this.animationCoroutine);
				this.animationCoroutine = null;
			}

			var timeString = this.GetComponent<KMBombInfo>().GetFormattedTime();
			if (this.SolutionAction == ButtonAction.Hold) {
				var time = this.GetComponent<KMBombInfo>().GetTime();
					this.Log(string.Format("The button was held and released at {0}. That was correct.", timeString));
					this.Disarm();
			} else {
				this.Log(string.Format("The button was held and released at {0}. That was incorrect: it should have been {1}.", timeString, SolutionActionPastTense(this.SolutionAction)));
				this.Connector.KMBombModule.HandleStrike();
			}
		}
	}

	private void StartedHolding() {
		this.Holding = true;
		if (this.Solved) {
			this.animationCoroutine = this.StartCoroutine(this.LightShowCoroutine());
		} else {
			this.SetLightColour((ButtonLightColour) (Random.Range(0, 15) + 1));
			this.animationCoroutine = this.StartCoroutine(this.LightAnimationCoroutine());
			this.Log(string.Format(this.SolutionAction == ButtonAction.Hold ? "The button is being held. The light is {0}. The button should be released." :
				"The button is being held. The light is {0}.", this.LightColour));
		}
	}

	private void SetLightColour(ButtonLightColour colour) {
		this.Connector.SetLightColour(this.LightColour = colour);
		if (colour == ButtonLightColour.Off) {
			foreach (var light in this.Lights)
				light.gameObject.SetActive(false);
		} else {
			foreach (var light in this.Lights)
				light.gameObject.SetActive(true);
			for (int i = 0; i < this.Lights.Length; i += 1) {
				switch (colour) {
					case ButtonLightColour.Red: this.Lights[i].color = Color.red; break;
					case ButtonLightColour.Yellow: this.Lights[i].color = Color.yellow; break;
					case ButtonLightColour.Blue: this.Lights[i].color = Color.blue; break;
					case ButtonLightColour.White: this.Lights[i].color = Color.white; break;
				}
			}
		}
	}

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

	private IEnumerator LightShowCoroutine() {
		ButtonLightColour[] colours = new[] { ButtonLightColour.White, ButtonLightColour.Red, ButtonLightColour.Yellow, ButtonLightColour.Red, ButtonLightColour.Blue };
		while (true) {
			var time = this.bombInfo.GetTime();
			this.SetLightColour(colours[(int) time % 5]);
			this.SetLightBrightness(this.ZenModeActive ? time - (float) Math.Floor(time) : (float) Math.Ceiling(time) - time);
			yield return null;
		}
	}

	private IEnumerator DisplayShowCoroutine() {
		var roll = Random.Range(0, 4);
		switch (roll) {
			case 0:
				while (true) {
					yield return null;
				}
			case 1:
				while (true) {
					yield return null;
				}
			case 2:
				while (true) {
					yield return null;
				}
			default:
				while (true) {
					for (int i = 0; i < 4; ++i) {
					}
					for (int i = 0; i < 4; ++i) {
					}
					for (int i = 0; i < 4; ++i) {
					}
				}
		}
	}

	// Twitch Plays support
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
						yield return this.SolutionAction == ButtonAction.Press ? "solve" : "strike";
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
		switch (this.SolutionAction) {
			case ButtonAction.Press:
				this.Connector.TwitchPress();
				yield return new WaitForSeconds(1 / 12f);
				this.Connector.TwitchRelease();
				break;
			case ButtonAction.Hold:
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
}
