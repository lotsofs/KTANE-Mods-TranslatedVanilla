using System;
using System.Collections;
using TranslatedVanillaModulesLib;
using UnityEngine;
using Random = UnityEngine.Random;

public class VentingGas : TranslatedVanillaModule<TranslatedVentingGasConnector> {

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
		string name = string.Format("{0} #{1}", Connector.KMNeedyModule.ModuleDisplayName, Connector.ModuleID);
		_translation.GenerateLanguage(name);

		if (_translation.Language.DisplayMethod == LanguageVentingGas.DisplayMethods.CustomTextMesh) {
			Connector.UseCustomDisplay();
		}

		if (_translation.Language.VentGas.Contains("\n")) {
			_multilineVent = true;
			Connector.SetDisplayText(TranslatedVentingGasConnector.Texts.VentGas, _translation.Language.VentGas.Split(new[] { "\n" }, StringSplitOptions.None)[0]);
			Connector.SetDisplayText(TranslatedVentingGasConnector.Texts.VentYN, _translation.Language.VentGas.Split(new[] { "\n" }, StringSplitOptions.None)[1]);
		}
		else {
			Connector.SetDisplayText(TranslatedVentingGasConnector.Texts.VentGas, _translation.Language.VentGas);
			Connector.SetDisplayText(TranslatedVentingGasConnector.Texts.VentYN, _translation.Language.YesNo);
		}
		if (_translation.Language.Detonate.Contains("\n")) {
			_multilineDetonate = true;
			Connector.SetDisplayText(TranslatedVentingGasConnector.Texts.Detonate, _translation.Language.Detonate.Split(new[] { "\n" }, StringSplitOptions.None)[0]);
			Connector.SetDisplayText(TranslatedVentingGasConnector.Texts.DetonateYN, _translation.Language.Detonate.Split(new[] { "\n" }, StringSplitOptions.None)[1]);
		}
		else {
			Connector.SetDisplayText(TranslatedVentingGasConnector.Texts.Detonate, _translation.Language.Detonate);
			Connector.SetDisplayText(TranslatedVentingGasConnector.Texts.DetonateYN, _translation.Language.YesNo);
		}
		Connector.SetDisplayText(TranslatedVentingGasConnector.Texts.VentingComplete, _translation.Language.VentingComplete);
		Connector.SetDisplayText(TranslatedVentingGasConnector.Texts.VentingPreventsExplosions, _translation.Language.VentingPrevents);
		Connector.InputText = string.Empty;

		if (_translation.Language.RightToLeft) Connector.SetButtonTexts(_translation.Language.N, _translation.Language.Y);
		else Connector.SetButtonTexts(_translation.Language.Y, _translation.Language.N);

		TwitchHelpMessage = string.Format("{1}, {2} - !{0} {3} | !{0} {4}", "{0}", _translation.Language.NativeName, _translation.Language.Name, _translation.Language.Y, _translation.Language.N);
	}

	public void DisarmNeedy(bool ventingComplete = false) {
		Connector.InputText = string.Empty;
		Connector.DisableDisplay();
		Connector.SetDisplayActive(TranslatedVentingGasConnector.Texts.VentingComplete, ventingComplete);
		_active = false;
		if (_coroutine != null) {
			StopCoroutine(_coroutine);
			_coroutine = null;
		}
	}

	private void KMNeedyModule_OnNeedyActivation() {
		_active = true;
		Connector.SetDisplayActive(TranslatedVentingGasConnector.Texts.VentingComplete, false);
		if (Random.Range(0f, 1f) < 0.1f) {
			_correctButton = VentingGasButton.N;
			Connector.SetDisplayActive(TranslatedVentingGasConnector.Texts.Detonate, true);
			Connector.SetDisplayActive(TranslatedVentingGasConnector.Texts.DetonateYN, true);
			Log(_translation.Language.LogPromptDetonate);
			if (_multilineDetonate) {
				Connector.SetDisplayText(TranslatedVentingGasConnector.Texts.InputText, _translation.Language.YesNo);
			}
		}
		else {
			_correctButton = VentingGasButton.Y;
			Connector.SetDisplayActive(TranslatedVentingGasConnector.Texts.VentGas, true);
			Connector.SetDisplayActive(TranslatedVentingGasConnector.Texts.VentYN, true);
			Log(_translation.Language.LogPromptVentGas);
			if (_multilineVent) {
				Connector.SetDisplayText(TranslatedVentingGasConnector.Texts.InputText, _translation.Language.YesNo);
			}
		}
		Connector.SetDisplayActive(TranslatedVentingGasConnector.Texts.InputText, true);
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
			Connector.SetDisplayActive(TranslatedVentingGasConnector.Texts.VentingPreventsExplosions, true);
			yield return new WaitForSeconds(0.75f);
			Connector.SetDisplayActive(TranslatedVentingGasConnector.Texts.VentingPreventsExplosions, false);
			yield return new WaitForSeconds(0.4f);
			Connector.SetDisplayActive(TranslatedVentingGasConnector.Texts.VentingPreventsExplosions, true);
			yield return new WaitForSeconds(0.75f);
			Connector.SetDisplayActive(TranslatedVentingGasConnector.Texts.VentingPreventsExplosions, false);
			yield return new WaitForSeconds(0.4f);
			Connector.SetDisplayActive(TranslatedVentingGasConnector.Texts.VentingPreventsExplosions, true);
			yield return new WaitForSeconds(1f);
			Connector.SetDisplayActive(TranslatedVentingGasConnector.Texts.VentingPreventsExplosions, false);
			Connector.SetDisplayActive(TranslatedVentingGasConnector.Texts.VentGas, true);
			Connector.SetDisplayActive(TranslatedVentingGasConnector.Texts.VentYN, true);
			Connector.SetDisplayActive(TranslatedVentingGasConnector.Texts.InputText, true);
			if (_multilineVent) {
				Connector.SetDisplayText(TranslatedVentingGasConnector.Texts.InputText, _translation.Language.YesNo);
			}
			_coroutine = null;
		}
	}

	#region TP

	// Twitch Plays support
	public static string TwitchHelpMessage = "!{0} left [press the left button] | !{0} right [press the right button]";

	public IEnumerator ProcessTwitchCommand(string command) {
		command = command.Trim().ToUpperInvariant();
		// todo: right to left languages get messed up with this?
		VentingGasButton button;
		if (command == _translation.Language.Y || command == _translation.Language.Yes) {
			button = _translation.Language.RightToLeft ? VentingGasButton.N : VentingGasButton.Y;
		}
		else if (command == _translation.Language.N || command == _translation.Language.No) {
			button = _translation.Language.RightToLeft ? VentingGasButton.Y : VentingGasButton.N;
		}
		else if (command == "LEFT") {
			button = VentingGasButton.Y;
		}
		else if (command == "RIGHT") {
			button = VentingGasButton.N;
		}
		else {
			yield break;
		}
		yield return null;
		Connector.TwitchPress(button);
	}

	#endregion
}
