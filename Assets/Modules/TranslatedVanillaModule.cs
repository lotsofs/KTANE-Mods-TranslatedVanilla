﻿using JetBrains.Annotations;
using TranslatedVanillaModulesLib;
using UnityEngine;

public abstract class TranslatedVanillaModule<TConnector> : MonoBehaviour where TConnector : TranslatedVanillaModuleConnector {
	protected TConnector Connector { get; private set; }
	public bool Solved { get; private set; }

	public virtual void Start() {
		this.Connector = this.GetComponent<TConnector>();
		var colorblindMode = this.GetComponent<KMColorblindMode>();
		if (colorblindMode != null) this.Connector.ColourblindMode = colorblindMode.ColorblindModeActive;
	}

	public virtual void Disarm() {
		this.Solved = true;
		this.Connector.KMBombModule.HandlePass();
	}

	protected bool TwitchColourblindModeCommand(string command) {
		command = command.Trim();
		if (command.EqualsIgnoreCase("colourblind") || command.EqualsIgnoreCase("colorblind") || command.EqualsIgnoreCase("cb") || command.EqualsIgnoreCase("color blind") || command.EqualsIgnoreCase("colour blind")) {
			this.Connector.ColourblindMode = true;
			return true;
		}
		return false;
	}

	public void Log(string message) { this.Connector.Log(message); }
	public void LogFormat(string message, params string[] args) { this.Connector.LogFormat(message, args); }
	[StringFormatMethod("format")]
	public void Log(string format, params object[] args) { this.Connector.Log(format, args); }
}
