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
		Connector.KMNeedyModule.OnNeedyDeactivation = () => DisarmNeedy();
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

	public void DisarmNeedy(bool ventingComplete = false) {
		Connector.InputText = "";
		Connector.DisableDisplay();
		Connector.SetDisplayActive(NotVentingGasConnector.Texts.VentingComplete, ventingComplete);
		if (coroutine != null) {
			StopCoroutine(coroutine);
			coroutine = null;
		}
	}

	private void KMNeedyModule_OnNeedyActivation() {
		Connector.SetDisplayActive(NotVentingGasConnector.Texts.VentingComplete, false);
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
		// if RTL, we actually pressed the other button
		bool rightToLeft = false; // todo: implement RTL :p
		button = button == VentingGasButton.Y ^ rightToLeft ? VentingGasButton.Y : VentingGasButton.N;   

		var label = button == VentingGasButton.Y ? "YES" : "NO";
		float inputTime = button == VentingGasButton.Y ? 1.5f : 1.0f;
		float charInputDelay = inputTime / label.Length;
		foreach (var c in label) {
			Connector.InputText += c;
			yield return new WaitForSeconds(charInputDelay);
		}
		yield return new WaitForSeconds(0.5f);

		bool correctPress = button == correctButton;

		if (correctButton == VentingGasButton.N) {
			if (button == correctButton) {
				Log("You pressed N. That was correct.");
		Connector.KMNeedyModule.HandlePass();
			}
			else {
				Log("You pressed Y. That was incorrect.");
				Connector.KMNeedyModule.HandleStrike();
			}
			DisarmNeedy();
			yield break;
		}
		else if (button == correctButton) {
			Log("You pressed Y. That was correct.");
			Connector.KMNeedyModule.HandlePass();
			DisarmNeedy(true);
			yield break;
		}
		else {
			Connector.DisableDisplay();
			Connector.InputText = "";
			Connector.SetDisplayActive(NotVentingGasConnector.Texts.VentingPreventsExplosions, true);
			yield return new WaitForSeconds(0.75f);
			Connector.SetDisplayActive(NotVentingGasConnector.Texts.VentingPreventsExplosions, false);
			yield return new WaitForSeconds(0.4f);
			Connector.SetDisplayActive(NotVentingGasConnector.Texts.VentingPreventsExplosions, true);
			yield return new WaitForSeconds(0.75f);
			Connector.SetDisplayActive(NotVentingGasConnector.Texts.VentingPreventsExplosions, false);
			yield return new WaitForSeconds(0.4f);
			Connector.SetDisplayActive(NotVentingGasConnector.Texts.VentingPreventsExplosions, true);
			yield return new WaitForSeconds(1f);
			Connector.SetDisplayActive(NotVentingGasConnector.Texts.VentingPreventsExplosions, false);
			Connector.SetDisplayActive(NotVentingGasConnector.Texts.VentGas, true);
			Connector.SetDisplayActive(NotVentingGasConnector.Texts.VentYN, true);
			Connector.SetDisplayActive(NotVentingGasConnector.Texts.InputText, true);
			coroutine = null;
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
		Connector.TwitchPress(button);
	}

	#endregion
}
