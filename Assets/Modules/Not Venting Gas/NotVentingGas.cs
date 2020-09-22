using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KModkit;
using NotVanillaModulesLib;
using UnityEngine;
using Random = UnityEngine.Random;

public class NotVentingGas : NotVanillaModule<NotVentingGasConnector> {
	public string DisplayText { get; private set; }

	private Coroutine coroutine;
	private int value;
	private VentingGasButton correctButton;

	public override void Start() {
		base.Start();
		this.Connector.KMNeedyModule.OnNeedyActivation = this.KMNeedyModule_OnNeedyActivation;
		this.Connector.KMNeedyModule.OnNeedyDeactivation = this.DisarmNeedy;
		this.Connector.KMNeedyModule.OnTimerExpired = this.KMNeedyModule_OnTimerExpired;
		this.Connector.ButtonPressed += this.Connector_ButtonPressed;
		this.value = this.GetComponent<KMBombInfo>().GetSerialNumberNumbers().LastOrDefault();
		this.Connector.InputText = "";
	}

	public void DisarmNeedy() {
		this.Connector.InputText = "";
		this.Connector.DisplayActive = false;
		if (this.coroutine != null) {
			this.StopCoroutine(this.coroutine);
			this.coroutine = null;
		}
	}

	private void KMNeedyModule_OnNeedyActivation() {
		if (Random.Range(0, 1) < 0.1f) {
			// detoante?
		}
		else {
			// vent gas?
		}
		
		this.DisplayText = "temp text";
		this.Connector.DisplayText = this.DisplayText;
		this.Connector.DisplayActive = true;

		this.correctButton = VentingGasButton.Y;
		this.value = value;
	}

	private void KMNeedyModule_OnTimerExpired() {
		this.Log("You didn't press the button in time.");
		this.Connector.KMNeedyModule.HandleStrike();
		this.DisarmNeedy();
	}

	private void Connector_ButtonPressed(object sender, VentingGasButtonEventArgs e) {
		if (this.Connector.DisplayActive && this.coroutine == null) this.coroutine = this.StartCoroutine(this.ButtonPressedCoroutine(e.Button));
	}

	private IEnumerator ButtonPressedCoroutine(VentingGasButton button) {
		var label = button == VentingGasButton.N ? "NO" : "YES";
		foreach (var c in label) {
			this.Connector.InputText += c;
			yield return new WaitForSeconds(0.5f);
		}
		yield return new WaitForSeconds(0.5f);
		if (this.Connector.DisplayActive) {
			if (button == this.correctButton)
				this.Log("You pressed {0}. That was correct.", button);
			else {
				this.Log("You pressed {0}. That was incorrect.", button);
				this.Connector.KMNeedyModule.HandleStrike();
			}
			this.Connector.KMNeedyModule.HandlePass();
			this.DisarmNeedy();
		}
	}

	#region TP

	// Twitch Plays support
	public static readonly string TwitchHelpMessage
		= "!{0} N | !{0} Y";

	public IEnumerator ProcessTwitchCommand(string command) {
		var tokens = command.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
		string buttonString; VentingGasButton button;
		switch (tokens.Length) {
			case 1: buttonString = tokens[0]; break;
			case 2:
				if (!tokens[0].EqualsIgnoreCase("press")) yield break;
				buttonString = tokens[1];
				break;
			default: yield break;
		}
		if (buttonString.Length == 0) yield break;
		switch (buttonString[0]) {
			case 'n': case 'N': button = VentingGasButton.N; break;
			case 'y': case 'Y': button = VentingGasButton.Y; break;
			default: yield break;
		}
		yield return null;
		this.Connector.TwitchPress(button);
	}

	#endregion
}
