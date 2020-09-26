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

	private Coroutine coroutine;
	private VentingGasButton correctButton;

	public override void Start() {
		base.Start();
		Connector.KMNeedyModule.OnNeedyActivation = KMNeedyModule_OnNeedyActivation;
		Connector.KMNeedyModule.OnNeedyDeactivation = DisarmNeedy;
		Connector.KMNeedyModule.OnTimerExpired = KMNeedyModule_OnTimerExpired;
		Connector.ButtonPressed += Connector_ButtonPressed;

		Connector.SetDisplayText(NotVentingGasConnector.Texts.VentGas, "VENT GAS?");
		Connector.SetDisplayText(NotVentingGasConnector.Texts.Detonate, "DETONATE?");
		Connector.SetDisplayText(NotVentingGasConnector.Texts.VentingComplete, "VENTING\nCOMPLETE");
		Connector.SetDisplayText(NotVentingGasConnector.Texts.VentingPreventsExplosions, "VENTING\nPREVENTS\nEXPLOSIONS");
		Connector.SetDisplayText(NotVentingGasConnector.Texts.VentYN, "Y/N");
		Connector.SetDisplayText(NotVentingGasConnector.Texts.DetonateYN, "Y/N");
		Connector.InputText = "";
	}

	public void DisarmNeedy() {
		Connector.InputText = "";
		Connector.DisableDisplay();
		if (coroutine != null) {
			StopCoroutine(coroutine);
			coroutine = null;
		}
	}

	private void KMNeedyModule_OnNeedyActivation() {
		if (Random.Range(0f, 1f) < 0.1f) {
			correctButton = VentingGasButton.N;
			Connector.SetDisplayActive(NotVentingGasConnector.Texts.Detonate, true);
			Connector.SetDisplayActive(NotVentingGasConnector.Texts.DetonateYN, true);
		}
		else {
			correctButton = VentingGasButton.Y;
			Connector.SetDisplayActive(NotVentingGasConnector.Texts.VentGas, true);
			Connector.SetDisplayActive(NotVentingGasConnector.Texts.VentYN, true);
		}
		Connector.SetDisplayActive(NotVentingGasConnector.Texts.InputText, true);
	}

	private void KMNeedyModule_OnTimerExpired() {
		Log("You didn't press the button in time.");
		Connector.KMNeedyModule.HandleStrike();
		DisarmNeedy();
	}

	private void Connector_ButtonPressed(object sender, VentingGasButtonEventArgs e) {
		if (coroutine == null) coroutine = StartCoroutine(ButtonPressedCoroutine(e.Button));
	}

	private IEnumerator ButtonPressedCoroutine(VentingGasButton button) {
		var label = button == VentingGasButton.N ? "NO" : "YES";
		float inputTime = button == VentingGasButton.N ? 1.0f : 1.5f;
		float charInputDelay = inputTime / label.Length;
		foreach (var c in label) {
			Connector.InputText += c;
			yield return new WaitForSeconds(charInputDelay);
		}
		yield return new WaitForSeconds(0.5f);
		// todo: VNETING PREVENTS E$XPLOSIONS
		if (button == correctButton)
			Log("You pressed {0}. That was correct.", button);
		else {
			Log("You pressed {0}. That was incorrect.", button);
			Connector.KMNeedyModule.HandleStrike();
		}
		Connector.KMNeedyModule.HandlePass();
		DisarmNeedy();
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
		Connector.TwitchPress(button);
	}

	#endregion
}
