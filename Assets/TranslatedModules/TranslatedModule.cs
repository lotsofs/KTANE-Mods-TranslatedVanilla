﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class TranslatedModule<TLanguage, TExtendedMissionSettings> : MonoBehaviour
	where TLanguage : Language
	where TExtendedMissionSettings : TranslatedModulesMissionSettings {

	public TLanguage Language { get {
			if (_language == null) {
				GenerateLanguage(this.name);
			}
			return _language;
		} 
	}

	TLanguage _language;
	string _moduleLogName;
	TranslationSettings _settings;

	[SerializeField] protected TLanguage[] _languages;
	[SerializeField] TLanguage _fallbackLanguage;
	[SerializeField] Sticker _sticker;
	[SerializeField] EMSLanguagesPool _extendedMissionSettings;
	[Space]
	[SerializeField] string _settingsFileName;
	[SerializeField] string[] _oldSettingsFiles;

	[Header("Debug")]
	[SerializeField] protected TLanguage _languageOverride;
	[SerializeField] string _editorExtendedMissionSettings;


	void Awake() {
		// reset everything to blank at the start of the mission
		if (_extendedMissionSettings == null) {
			_extendedMissionSettings = new EMSLanguagesPool();
		}
		_extendedMissionSettings.pool = null;
		_extendedMissionSettings.status = EMSLanguagesPool.Statuses.Uninitialized;
		if (GetComponent<KMBombModule>() != null) {
			_moduleLogName = GetComponent<KMBombModule>().ModuleDisplayName;
		}
		else {
			_moduleLogName = GetComponent<KMNeedyModule>().ModuleDisplayName;
		}
	}

	void Start() {
		//SetLanguage();			// uncomment to call automatically. Commented here so ModuleLogName can be set first and then SetLanguage can be done automatically.
	}

	void Log(string message) {
		Debug.LogFormat("[{0}] {1}", _moduleLogName, message);
	}

	void LogFormat(string message, params string[] args) {
		message = string.Format(message, args);
		Log(message);
	}

	/// <summary>
	/// Reads the settings from the configuration file in the mod settings folder
	/// </summary>
	/// <returns></returns>
	TranslationSettings ReadConfig() {
		Configuration<TranslationSettings> config = new Configuration<TranslationSettings>(_settingsFileName);
		TranslationSettings settings = config.Settings;

		//// check for an old config file
		//foreach (string cfgOld in _oldSettingsFiles) {
		//	Configuration<TranslationSettings> configOld = new Configuration<TranslationSettings>(cfgOld, false);
		//	TranslationSettings oldSettings = configOld.OldSettings;
		//	if (oldSettings != null) {
		//		settings.UseAllLanguages = oldSettings.UseAllLanguages;
		//		settings.IgnoreWithoutManual = oldSettings.IgnoreWithoutManual;
		//		settings.LanguagePool = oldSettings.LanguagePool;
		//		configOld.ClearFile();
		//	}
		//}
		config.Settings = settings;

		if (settings.UseGlobalSettings) {
			Configuration<TranslationSettings> configG = new Configuration<TranslationSettings>("TranslatedModules-Settings");
			if (configG.Settings != null) {
				configG.Settings.UseGlobalSettings = true;
				Log("Config file dictates using the global translated modules settings.");
				return configG.Settings;
			}
			else {
				// could not find global config file. See if the service is installed, and whether there perhaps was an update that renamed the settings file.
				GameObject ts = GameObject.Find("TranslatedModulesService(Clone)");
				if (ts == null) {
					// translated modules service not installed.
					Log("Config file dictates using the global translated modules settings, but the translated modules service does not appear to be installed.");
					return null;
				}
				try {
					Component service = ts.GetComponent("TranslatedModulesService");
					Type type = service.GetType();
					FieldInfo fieldSettings = type.GetField("SettingsFileName");
					string settingsFileName = (string)fieldSettings.GetValue(service);
					Configuration<TranslationSettings> configG2 = new Configuration<TranslationSettings>("TranslatedModules-Settings");
					if (configG2.Settings != null) {
						configG2.Settings.UseGlobalSettings = true;
						Log("Config file dictates using the global translated modules settings. These settings were found, but under a different filename than expected.");
						return configG2.Settings;
					}
				}
				catch (Exception e) {
					Debug.Log(e.Message);
					Log("Config file dictates using the global translated modules settings, but an error occured trying to acquire them.");
					return null;
				}
			}
		}
		return config.Settings;
	}

	/// <summary>
	/// Looks for extended mission settings in the current mission and returns these if found, otherwise returns null.
	/// </summary>
	/// <returns></returns>
	TExtendedMissionSettings ReadExtendedMissionSettings() {
		TExtendedMissionSettings ems;
		EMSRResults result;
		if (Application.isEditor) {
			result = ExtendedMissionSettingsReader<TExtendedMissionSettings>.ReadMissionSettings(out ems, _editorExtendedMissionSettings);
		}
		else {
			result = ExtendedMissionSettingsReader<TExtendedMissionSettings>.ReadMissionSettings(out ems);
		}
		switch (result) {
			case EMSRResults.NotInstalled:
			case EMSRResults.Empty:
				break;
			case EMSRResults.Error:
				LogFormat("An exception occured when trying to read extended mission settings.");
				break;
			case EMSRResults.ReceivedNull:
				LogFormat("There was an issue with the extended mission settings service.");
				break;
			case EMSRResults.Success:
				LogFormat("Received extended mission settings. Checking it first for determining language.");
				return ems;
		}
		return null;
	}

	/// <summary>
	/// Finds a specific language in the language list for this module.
	/// </summary>
	/// <param name="iso">the iso code of the language</param>
	/// <returns>Whether it succesfully found something</returns>
	TLanguage FindLanguage(string iso) {
		for (int i = 0; i < _languages.Length; i++) {
			TLanguage t = _languages[i];
			if (!t.Disabled && t.Iso639 == iso) {
				LogFormat("Found language with ISO-639 code '{0}'.", t.Iso639);
				return t;
			}
		}
		LogFormat("Could not find language with ISO-639 code '{0}'.", iso);
		return null;
	}

	/// <summary>
	/// Marks the language language as the used language for this module
	/// </summary>
	/// <param name="language"></param>
	void UseLanguage(TLanguage language) {
		Log("--------------------------");
		_language = language;

		// finalize selection
		LogFormat("Selected Language: {0}, {1} ({2})\n", _language.NativeName, _language.Name, _language.Iso639);
		if (_sticker != null) {
			_sticker.GenerateText(_language.Iso639, _language.Version);
		}
		else {
			Debug.LogFormat("[Translated Modules Service] Module '{0}' has no sticker", _moduleLogName);
		}
	}

	/// <summary>
	/// Establish a global pool of languages from which all modules of this type will pick languages
	/// </summary>
	/// <param name="ems"></param>
	void EstablishPool(TExtendedMissionSettings ems) {
		_extendedMissionSettings.FixedLanguages = ems.FixedLanguages;
		_extendedMissionSettings.RandomLanguages = ems.RandomLanguages;
		_extendedMissionSettings.AvoidDuplicates = ems.AvoidDuplicates;
		_extendedMissionSettings.ShuffleFixedLanguages = ems.ShuffleFixedLanguages;
		_extendedMissionSettings.status = EMSLanguagesPool.Statuses.FixedPool;
		_extendedMissionSettings.pool = ems.FixedLanguages != null ? ems.FixedLanguages.ToList() : new List<string>();
	}

	/// <summary>
	/// Select a language for this module.
	/// </summary>
	public void GenerateLanguage(string moduleName) {
		_moduleLogName = moduleName;

		// debug
		if (Application.isEditor && _languageOverride != null) {
			LogFormat("DEBUG: Language overridden to {0}.", _languageOverride.Iso639);
			UseLanguage(_languageOverride);
			return;
		}

		TLanguage lang;


		// First check the EMS for languages to pick
		TExtendedMissionSettings ems = ReadExtendedMissionSettings();
		if (ems == null) {
			// No EMS used on this mission.
			lang = PickLanguageFromConfigFile();
		}
		else {
			switch (_extendedMissionSettings.status) {
				case EMSLanguagesPool.Statuses.Uninitialized:
					// Only one module per mission needs to record these settings. A null pool indicates no other module has processed it yet, otherwise it would be empty instead.
					// Status Uninitialized does the same. In either of these cases, establish a new one. Otherwise, a pool has already been established for this mission.
					EstablishPool(ems);
					goto case EMSLanguagesPool.Statuses.FixedPool;
				case EMSLanguagesPool.Statuses.FixedPool:
					Log("Checking fixed pool from extended mission settings.");
					lang = PickLanguageFromFixedPool();
					if (lang == null) {
						_extendedMissionSettings.status = EMSLanguagesPool.Statuses.RandomPool;
						goto case EMSLanguagesPool.Statuses.RandomPool;
					}
					else {
						break;
					}
				case EMSLanguagesPool.Statuses.RandomPool:
					Log("Checking random pool from extended mission settings.");
					lang = PickLanguageFromRandomPool();
					if (lang == null) {
						_extendedMissionSettings.status = EMSLanguagesPool.Statuses.ConfigFile;
						goto case EMSLanguagesPool.Statuses.ConfigFile;
					}
					else {
						break;
					}
				case EMSLanguagesPool.Statuses.ConfigFile:
					Log("Resorting to player's personal config file to determine the remaining modules' languages.");
					Log("--------------------------");
					lang = PickLanguageFromConfigFile();
					break;
				default:
					lang = null;
					break;
			}
		}
		if (lang != null) {
			UseLanguage(lang);
		}
		else {
			LogFormat("WARNING: Could not find a language to be used. Using a fallback language.");
			UseLanguage(_fallbackLanguage);
		}

	}
	
	/// <summary>
		/// Picks a language according to the EMS's fixed pool
		/// </summary>
		/// <returns></returns>
	TLanguage PickLanguageFromFixedPool() {
		while (true) {
			// check if pool even was provided
			if (_extendedMissionSettings.FixedLanguages == null || _extendedMissionSettings.FixedLanguages.Length == 0) {
				Log("No fixed pool provided.");
				return null;
			}

			// check if pool's depleted
			if (_extendedMissionSettings.pool.Count == 0) {
				Log("Fixed pool depleted.");
				return null;
			}

			// pick from pool
			int index = _extendedMissionSettings.ShuffleFixedLanguages ? UnityEngine.Random.Range(0, _extendedMissionSettings.pool.Count) : 0;
			string isoCode = _extendedMissionSettings.pool[index];
			TLanguage chosenLanguage = FindLanguage(isoCode);
			_extendedMissionSettings.pool.RemoveAt(index);
			if (chosenLanguage != null) {
				Log("Succesfully picked module from extended mission settings fixed pool.");
				return chosenLanguage;
			}
		}
	}

	/// <summary>
	/// Picks a language according to the random pool from EMS
	/// </summary>
	/// <returns></returns>
	TLanguage PickLanguageFromRandomPool() {
		List<string> invalidRandomPoolEntries = new List<string>();
		while (true) {
			// check if pool even was provided
			if (_extendedMissionSettings.RandomLanguages == null || _extendedMissionSettings.RandomLanguages.Length == 0) {
				Log("No random pool provided.");
				return null;
			}

			// check if the entire pool is invalid
			if (invalidRandomPoolEntries.Count >= _extendedMissionSettings.RandomLanguages.Length) {
				Log("There are no valid entries in the random pool.");
				return null;
			}

			// check if working pool has been depleted
			if (_extendedMissionSettings.pool.Count == 0) {
				Log("Random pool depleted. Refilling.");
				_extendedMissionSettings.pool = _extendedMissionSettings.RandomLanguages.ToList();
				invalidRandomPoolEntries.Clear();   // Have to clear this too to ensure it loops through the entire random pool array at least once.
				continue;
			}

			// pick from pool
			int index = UnityEngine.Random.Range(0, _extendedMissionSettings.pool.Count);
			string isoCode = _extendedMissionSettings.pool[index];
			TLanguage chosenLanguage = FindLanguage(isoCode);
			if (_extendedMissionSettings.AvoidDuplicates) {
				_extendedMissionSettings.pool.RemoveAt(index);
			}
			if (chosenLanguage != null) {
				Log("Succesfully picked module from extended mission settings random pool.");
				return chosenLanguage;
			}
			else {
				_extendedMissionSettings.pool.RemoveAt(index);
				invalidRandomPoolEntries.Add(isoCode);
				continue;
			}
		}
	}

	/// <summary>
	/// Picks a language according to the player's config file
	/// </summary>
	/// <returns></returns>
	TLanguage PickLanguageFromConfigFile() {
		_settings = ReadConfig();

		string excludedNotInPool = "Languages ignored because the configuration file does not include them: ";
		string excludedNoManual = "Languages ignored because the configuration file dictates modules with manuals only: ";
		string excludedMachine = "Languages ignored because the configuration file dictates ignoring machine translations: ";
		string excludedUntranslated = "Languages ignored because the configuration file dictates ignoring translations that didn't bother with translating the main content of the module: ";
		string excludedIgnored = "Languages ignored because they're actively disabled in the configuration file: ";
		string includedSelection = "{0} languages available for selection: ";

		TLanguage transl;
		List<TLanguage> availableTranslations = new List<TLanguage>();
		Dictionary<string, List<TLanguage>> languageGroups = new Dictionary<string, List<TLanguage>>();
		bool hasUnfinishedLanguages = false;

		for (int i = 0; i < _languages.Length; i++) {
			transl = _languages[i];
			if (transl.Disabled) {
				// language is disabled. Don't choose
				continue;
			}

			// check if we're using the enabled languages list and it does not contain this language
			if (!_settings.UseAllLanguages
			&& !_settings.EnabledLanguages.Contains(transl.Iso639, StringComparer.OrdinalIgnoreCase)
			&& !_settings.EnabledLanguages.Contains(transl.IetfBcp47, StringComparer.OrdinalIgnoreCase)) {
				excludedNotInPool += string.Format("{0}, ", transl.IetfBcp47);
				continue;
			}

			// check for disabled languages outright
			if (_settings.DisabledLanguages.Contains(transl.Iso639, StringComparer.OrdinalIgnoreCase)) {
				excludedIgnored += string.Format("{0}, ", transl.IetfBcp47);
				continue;
			}
			if (_settings.DisabledLanguages.Contains(transl.IetfBcp47, StringComparer.OrdinalIgnoreCase)) {
				excludedIgnored += string.Format("{0}, ", transl.IetfBcp47);
				continue;
			}
			if (_settings.DisabledLanguages.Contains(transl.IetfBcp47.Split(new string[] { "-x-", "-X-", "-t-", "-T-", "-u-", "-U-" }, StringSplitOptions.RemoveEmptyEntries)[0], StringComparer.OrdinalIgnoreCase)) {
				excludedIgnored += string.Format("{0}, ", transl.IetfBcp47);
				continue;
			}

			// check for certain language settings we dont want
			if (_settings.IgnoreWithoutManual && !transl.ManualAvailable) {
				excludedNoManual += string.Format("{0}, ", transl.IetfBcp47);
				continue;
			}
			if (_settings.IgnoreMachineTranslations && transl.MachineTranslation) {
				excludedMachine += string.Format("{0}, ", transl.IetfBcp47);
			}

			if (_settings.IgnoredPrivateSubtags != null && _settings.IgnoredPrivateSubtags.Length > 0) {
				bool cont = false;
				foreach (string subtag in transl.IetfBcp47.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries)) {
					if (subtag == "untc") {
						hasUnfinishedLanguages = true;
					}
					if (_settings.IgnoredPrivateSubtags.Contains(subtag, StringComparer.OrdinalIgnoreCase)) {
						LogFormat("Ignoring language {0} because its ietf bcp 47 tag contains illegal private use subtag '{1}'", transl.IetfBcp47, subtag);
						if (subtag == "untc") {
							excludedUntranslated += string.Format("{0}, ", transl.IetfBcp47);
						}
						cont = true; 
					}
				}
				if (cont) continue;
			}

			includedSelection += string.Format("{0}, ", transl.Iso639);
			availableTranslations.Add(transl);
			if (languageGroups.ContainsKey(transl.Iso639)) {
				languageGroups[transl.Iso639].Add(transl);
			}
			else {
				languageGroups.Add(transl.Iso639, new List<TLanguage> { transl } );
			}
		}

		if (!_settings.UseAllLanguages && excludedNotInPool.Trim().Last() != ':') Log(excludedNotInPool);
		else Log("Configuration file dictates using any available language.");

		if (_settings.DisabledLanguages != null && excludedIgnored.Trim().Last() != ':') Log(excludedIgnored);

		if (_settings.IgnoreWithoutManual && excludedNoManual.Trim().Last() != ':') Log(excludedNoManual);
		else Log("Configuration file allows for the use of translations without a dedicated manual.");

		if (_settings.IgnoreMachineTranslations && excludedMachine.Trim().Last() != ':') Log(excludedMachine);

		if (_settings.IgnoredPrivateSubtags.Contains("untc", StringComparer.OrdinalIgnoreCase) && excludedUntranslated.Trim().Last() != ':') Log(excludedUntranslated);
		else if (hasUnfinishedLanguages) Log("Configuration file allows for the use of translations that didn't bother to translate the main content of the module.");


		if (availableTranslations.Count == 0) {
			Log("There were no languages available to be chosen for this module in accordance with the configuration file.");
			return null;
		}
		LogFormat(includedSelection, availableTranslations.Count.ToString());

		if (_settings.TreatMultipleVariantsAsOne) {
			List<TLanguage>[] values = languageGroups.Values.ToArray();
			int index = UnityEngine.Random.Range(0, values.Length);
			int index2 = UnityEngine.Random.Range(0, values[index].Count);
			return values[index][index2];
		}
		else {
			int index = UnityEngine.Random.Range(0, availableTranslations.Count);
			return availableTranslations[index];
		}
	}
}

