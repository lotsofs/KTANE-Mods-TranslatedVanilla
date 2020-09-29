using System;
using System.Collections;
using NotVanillaModulesLib;
using UnityEngine;
using Random = UnityEngine.Random;

public class NotVentingGas : NotVanillaModule<NotVentingGasConnector> {

	private Coroutine _coroutine;
	private bool _active = false;
	private bool _multilineVent = false;
	private bool _multilineDetonate = false;

	private VentingGasButton _correctButton;
	private TranslatedVentingGas _translation; 

	public override void Start() {
		base.Start();
		Connector.KMNeedyModule.OnNeedyActivation = KMNeedyModule_OnNeedyActivation;
		Connector.KMNeedyModule.OnNeedyDeactivation = () => DisarmNeedy();
		Connector.KMNeedyModule.OnTimerExpired = KMNeedyModule_OnTimerExpired;
		GetComponent<KMBombInfo>().OnBombSolved = Connector.DisableDisplay;
		Connector.ButtonPressed += Connector_ButtonPressed;

		_translation = GetComponent<TranslatedVentingGas>();

		if (_translation.Language.VentGas.Contains("\n")) {
			_multilineVent = true;
			Connector.SetDisplayText(NotVentingGasConnector.Texts.VentGas, _translation.Language.VentGas.Split(new[] { "\n" }, StringSplitOptions.None)[0]);
			Connector.SetDisplayText(NotVentingGasConnector.Texts.VentYN, _translation.Language.VentGas.Split(new[] { "\n" }, StringSplitOptions.None)[1]);
		}
		else {
			Connector.SetDisplayText(NotVentingGasConnector.Texts.VentGas, _translation.Language.VentGas);
			Connector.SetDisplayText(NotVentingGasConnector.Texts.VentYN, _translation.Language.YesNo);
		}
		if (_translation.Language.Detonate.Contains("\n")) {
			_multilineDetonate = true;
			Connector.SetDisplayText(NotVentingGasConnector.Texts.Detonate, _translation.Language.Detonate.Split(new[] { "\n" }, StringSplitOptions.None)[0]);
			Connector.SetDisplayText(NotVentingGasConnector.Texts.DetonateYN, _translation.Language.Detonate.Split(new[] { "\n" }, StringSplitOptions.None)[1]);
		}
		else {
			Connector.SetDisplayText(NotVentingGasConnector.Texts.Detonate, _translation.Language.Detonate);
			Connector.SetDisplayText(NotVentingGasConnector.Texts.DetonateYN, _translation.Language.YesNo);
		}
		Connector.SetDisplayText(NotVentingGasConnector.Texts.VentingComplete, _translation.Language.VentingComplete);
		Connector.SetDisplayText(NotVentingGasConnector.Texts.VentingPreventsExplosions, _translation.Language.VentingPrevents);
		Connector.InputText = string.Empty;
	}

	public void DisarmNeedy(bool ventingComplete = false) {
		Connector.InputText = string.Empty;
		Connector.DisableDisplay();
		Connector.SetDisplayActive(NotVentingGasConnector.Texts.VentingComplete, ventingComplete);
		_active = false;
		if (_coroutine != null) {
			StopCoroutine(_coroutine);
			_coroutine = null;
		}
	}

	private void KMNeedyModule_OnNeedyActivation() {
		_active = true;
		Connector.SetDisplayActive(NotVentingGasConnector.Texts.VentingComplete, false);
		if (Random.Range(0f, 1f) < 0.1f) {
			_correctButton = VentingGasButton.N;
			Connector.SetDisplayActive(NotVentingGasConnector.Texts.Detonate, true);
			Connector.SetDisplayActive(NotVentingGasConnector.Texts.DetonateYN, true);
			Log(_translation.Language.LogPromptDetonate);
			if (_multilineDetonate) {
				Connector.SetDisplayText(NotVentingGasConnector.Texts.InputText, _translation.Language.YesNo);
			}
		}
		else {
			_correctButton = VentingGasButton.Y;
			Connector.SetDisplayActive(NotVentingGasConnector.Texts.VentGas, true);
			Connector.SetDisplayActive(NotVentingGasConnector.Texts.VentYN, true);
			Log(_translation.Language.LogPromptVentGas);
			if (_multilineVent) {
				Connector.SetDisplayText(NotVentingGasConnector.Texts.InputText, _translation.Language.YesNo);
			}
		}
		Connector.SetDisplayActive(NotVentingGasConnector.Texts.InputText, true);
	}

	private void KMNeedyModule_OnTimerExpired() {
		Log(_translation.Language.LogTooLate);
		Connector.KMNeedyModule.HandleStrike();
		DisarmNeedy();
	}

	private void Connector_ButtonPressed(object sender, VentingGasButtonEventArgs e) {
		if (_coroutine == null && _active) _coroutine = StartCoroutine(ButtonPressedCoroutine(e.Button));
	}

	private IEnumerator ButtonPressedCoroutine(VentingGasButton button) {
		button = button == VentingGasButton.Y ^ _translation.Language.RightToLeft ? VentingGasButton.Y : VentingGasButton.N;   

		var label = button == VentingGasButton.Y ? _translation.Language.Yes : _translation.Language.No;
		float inputTime = button == VentingGasButton.Y ? 1.5f : 1.0f;
		float charInputDelay = inputTime / label.Length;
		Connector.InputText = "";
		for (int i = 0; i < label.Length; i++) {
			if (_translation.Language.RightToLeft) {
				Connector.InputText = label[label.Length - 1 - i] + Connector.InputText;
			}
			else { 
				Connector.InputText += label[i];
			}
			yield return new WaitForSeconds(charInputDelay);
		}
		yield return new WaitForSeconds(0.5f);

		bool correctPress = button == _correctButton;

		if (_correctButton == VentingGasButton.N) {
			if (button == _correctButton) {
				Log(_translation.Language.LogNoCorrect);
			}
			else {
				Log(_translation.Language.LogYesIncorrect);
				Connector.KMNeedyModule.HandleStrike();
			}
			Connector.KMNeedyModule.HandlePass();
			DisarmNeedy();
			yield break;
		}
		else if (button == _correctButton) {
			Log(_translation.Language.LogYesCorrect);
			Connector.KMNeedyModule.HandlePass();
			DisarmNeedy(true);
			yield break;
		}
		else {
			Log(_translation.Language.LogNoIncorrect);
			Connector.DisableDisplay();
			Connector.InputText = string.Empty;
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
			if (_multilineVent) {
				Connector.SetDisplayText(NotVentingGasConnector.Texts.InputText, _translation.Language.YesNo);
			}
			_coroutine = null;
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
