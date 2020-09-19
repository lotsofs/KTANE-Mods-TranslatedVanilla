using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TranslatedModule<TLanguage, TExtendedMissionSettings> : MonoBehaviour
	where TLanguage : Language
	where TExtendedMissionSettings : TranslatedModulesMissionSettings {

	public TLanguage Language { get {
			if (_language == null) {
				SetLanguage(this.name);
			}
			return _language;
		} 
	}

	TLanguage _language;
	string _moduleLogName;
	TranslationSettings _settings;

	[SerializeField] TLanguage[] _languages;
	[SerializeField] TLanguage _fallbackLanguage;
	[SerializeField] Sticker _sticker;
	[SerializeField] EMSLanguagesPool _extendedMissionSettings;
	[Space]
	[SerializeField] string _settingsFileName;
	[SerializeField] string[] _oldSettingsFiles;

	[Header("Debug")]
	[SerializeField] TLanguage _languageOverride;
	[SerializeField] string _editorExtendedMissionSettings;


	void Awake() {
		// reset everything to blank at the start of the mission
		_extendedMissionSettings.pool = null;
		_extendedMissionSettings.status = EMSLanguagesPool.Statuses.Uninitialized;
		_moduleLogName = GetComponent<KMBombModule>().ModuleDisplayName;
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

		// check for an old config file
		foreach (string cfgOld in _oldSettingsFiles) {
			Configuration<TranslationSettings> configOld = new Configuration<TranslationSettings>(cfgOld, false);
			TranslationSettings oldSettings = configOld.OldSettings;
			if (oldSettings != null) {
				settings.UseAllLanguages = oldSettings.UseAllLanguages;
				settings.UseLanguagesWithManualOnly = oldSettings.UseLanguagesWithManualOnly;
				settings.LanguagePool = oldSettings.LanguagePool;
				configOld.ClearFile();
			}
		}
		config.Settings = settings;
		return settings;
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
		_language.Choose();

		// finalize selection
		LogFormat("Selected Language: {0}, {1} ({2})\n", _language.NativeName, _language.Name, _language.Iso639);
		_sticker.GenerateText(_language.Iso639, _language.Version);
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
		string includedSelection = "Languages available for selection: ";
		TLanguage transl;
		List<TLanguage> availableTranslations = new List<TLanguage>();
		for (int i = _languages.Length - 1; i >= 0; i--) {
			transl = _languages[i];
			if (transl.Disabled) {
				// language is disabled. Don't choose
				continue;
			}
			if (!_settings.UseAllLanguages && !_settings.LanguagePool.Contains(transl.Iso639)) {
				// if using the language pool and it does not contain this language
				excludedNotInPool += string.Format("{0}, ", transl.Iso639);
				continue;
			}
			if (_settings.UseLanguagesWithManualOnly && !transl.ManualAvailable) {
				// if a language has no manual but we want languages with a manual only, skip it
				excludedNoManual += string.Format("{0}, ", transl.Iso639);
				continue;
			}
			includedSelection += string.Format("{0}, ", transl.Iso639);
			availableTranslations.Add(transl);
		}

		if (!_settings.UseAllLanguages)
			Log(excludedNotInPool);
		else
			Log("Configuration file dictates using any available language.");
		if (_settings.UseLanguagesWithManualOnly)
			Log(excludedNoManual);
		else
			Log("Configuration file allows for the use of languages without a dedicated manual.");
		Log(includedSelection);

		if (availableTranslations.Count == 0) {
			Log("There were no languages available to be chosen for this module in accordance with the configuration file.");
			return null;
		}
		else {
			int index = UnityEngine.Random.Range(0, availableTranslations.Count);
			return availableTranslations[index];
		}
	}

	/// <summary>
	/// Select a language for this module.
	/// </summary>
	public void SetLanguage(string moduleName) {
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
}

