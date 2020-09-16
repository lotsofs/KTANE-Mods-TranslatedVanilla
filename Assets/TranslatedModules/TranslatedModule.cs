using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NotVanillaModulesLib;

public class TranslatedModule<LanguageModule> : MonoBehaviour where LanguageModule : Language {

	[SerializeField] LanguageModule[] _languages;
	//[SerializeField] GameObject _languagesHolder;

	public NotVanillaModuleConnector KMModule;
	[SerializeField] Sticker _sticker;
	[SerializeField] string _settingsFileName;
	[SerializeField] string[] _oldSettingsFiles;

	[NonSerialized] public LanguageModule Language;
	[SerializeField] LanguageModule _fallback;
	[Header("Debug")]
	public LanguageModule Override;

	TranslationSettings _settings;

	//public void GenerateHtmlTable() {
	//	string table = "";
	//	for (int i = 0; i < _languagesHolder.transform.childCount; i++) {
	//		Translation trans = _languagesHolder.transform.GetChild(i).GetComponent<Translation>();
	//		table += string.Format("<li><strong>{0}</strong>: {1}</li>\n", trans.Iso639, trans.Name);
	//	}
	//	Debug.Log(table);
	//}

	void Awake() {
		SelectLanguage();
	}

	void Start() {
		TheButtonMissionSettings.pool = null;
		TheButtonMissionSettings.status = TheButtonMissionSettings.Status.SelectingFinished;
	}

	/// <summary>
	/// Reads the settings from the configuration file in the mod settings folder
	/// </summary>
	/// <returns></returns>
	public TranslationSettings ReadConfig() {
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
	public TheButtonMissionSettings ReadExtendedMissionSettings() {
		TheButtonMissionSettings settings;
		EMSRResults result = ExtendedMissionSettingsReader<TheButtonMissionSettings>.ReadMissionSettings(out settings);
		switch (result) {
			case EMSRResults.NotInstalled:
			case EMSRResults.Empty:
				break;
			case EMSRResults.Error:
				KMModule.LogFormat("An exception occured when trying to read extended mission settings.");
				break;
			case EMSRResults.ReceivedNull:
				KMModule.LogFormat("There was an issue with the extended mission settings service.");
				break;
			case EMSRResults.Success:
				KMModule.LogFormat("Received extended mission settings. Checking it first for determining language.");
				return settings;
		}
		return null;
	}

	/// <summary>
	/// Finds a specific language and marks it as used for this module.
	/// </summary>
	/// <param name="iso">the iso code of the language</param>
	/// <returns>Whether it succesfully found something</returns>
	public bool ChooseFixedLanguage(string iso) {
		for (int i = 0; i < _languages.Length; i++) {
			LanguageModule t = _languages[i];
			if (!t.Disabled && t.Iso639 == iso) {
				Language = t;
				KMModule.LogFormat("Selecting language with ISO-639 code '{0}' from extended mission settings.", t.Iso639);
				t.Choose();
				return true;
			}
		}
		KMModule.LogFormat("Could not find language with ISO-639 code '{0}' from extended mission settings.", iso);
		return false;
	}

	/// <summary>
	/// Select a language for this module.
	/// </summary>
	public void SelectLanguage() {
		// debug
		if (Override != null) {
			KMModule.LogFormat("DEBUG: Language overridden to {0}.", Override.Iso639);
			Language = Override;
			Override.Choose();
			goto Finalize;
		}
		
		// First check the EMS for languages to pick
		TheButtonMissionSettings ems = ReadExtendedMissionSettings();
		if (ems == null) goto ConfigFile;	// No EMS used on this mission.

		List<string> invalidRandomPoolEntries = new List<string>();
		// Only one module per mission needs to record these settings. A null pool indicates no other module has processed it yet, otherwise it would be empty instead.
		// Status = Finished could be a leftover from previous bomb.
		if (TheButtonMissionSettings.pool == null || TheButtonMissionSettings.status == TheButtonMissionSettings.Status.SelectingFinished) {
			TheButtonMissionSettings.status = TheButtonMissionSettings.Status.FixedPool;
			TheButtonMissionSettings.pool = ems.BigButtonTranslated_FixedLanguages != null ? ems.BigButtonTranslated_FixedLanguages.ToList() : new List<string>();
		}

		if (TheButtonMissionSettings.status == TheButtonMissionSettings.Status.RandomPool) goto PreRandomPool;
		if (TheButtonMissionSettings.status == TheButtonMissionSettings.Status.ConfigFile) goto PreConfigFile;



		// --------------------------------------------------------------------------------
		// Check the extended mission settings fixed pool for languages to use. Keeps looping until it is depleted, or if none is provided, then it will move on.
		// --------------------------------------------------------------------------------
		KMModule.Log("Checking fixed pool from extended mission settings.");
	FixedPool:
		// check if pool even was provided
		if (ems.BigButtonTranslated_FixedLanguages == null || ems.BigButtonTranslated_FixedLanguages.Length == 0) {
			KMModule.Log("No fixed pool provided.");
			goto PreRandomPool;
		}

		// check if pool's depleted
		if (TheButtonMissionSettings.pool.Count == 0) {
			KMModule.Log("Fixed pool depleted.");
			goto PreRandomPool;
		}

		// pick from pool
		int indexF = ems.BigButtonTranslated_ShuffleFixedLanguages ? UnityEngine.Random.Range(0, TheButtonMissionSettings.pool.Count) : 0;
		string isoF = TheButtonMissionSettings.pool[indexF];
		bool chosenF = ChooseFixedLanguage(isoF);
		TheButtonMissionSettings.pool.RemoveAt(indexF);
		if (chosenF) {
			KMModule.Log("Succesfully picked module from extended mission settings fixed pool.");
			goto Finalize;
		}
		else goto FixedPool;


		// --------------------------------------------------------------------------------
		// Checked the extended mission settings random pool. Keeps looping through it if it is provided and valid, otherwise it moves on.
		// --------------------------------------------------------------------------------
	PreRandomPool:
		TheButtonMissionSettings.status = TheButtonMissionSettings.Status.RandomPool;
		KMModule.Log("Checking random pool from extended mission settings.");
	RandomPool:
		// check if pool even was provided
		if (ems.BigButtonTranslated_RandomLanguages == null || ems.BigButtonTranslated_RandomLanguages.Length == 0) {
			KMModule.Log("No random pool provided.");
			goto PreConfigFile;
		}

		// check if the entire pool is invalid
		if (invalidRandomPoolEntries.Count >= ems.BigButtonTranslated_RandomLanguages.Length) {
			KMModule.Log("There are no valid entries in the random pool.");
			goto PreConfigFile;
		}

		// check if working pool has been depleted
		if (TheButtonMissionSettings.pool.Count == 0) {
			KMModule.Log("Random pool depleted. Refilling.");
			TheButtonMissionSettings.pool = ems.BigButtonTranslated_RandomLanguages.ToList();
			invalidRandomPoolEntries.Clear();	// Have to clear this too to ensure it loops through the entire random pool array at least once.
			goto RandomPool;
		}

		// pick from pool
		int indexR = UnityEngine.Random.Range(0, TheButtonMissionSettings.pool.Count);
		string isoR = TheButtonMissionSettings.pool[indexR];
		bool chosenR = ChooseFixedLanguage(isoR);
		if (ems.BigButtonTranslated_AvoidDuplicates) {
			TheButtonMissionSettings.pool.RemoveAt(indexR);
		}
		if (chosenR) {
			KMModule.Log("Succesfully picked module from extended mission settings random pool.");
			goto Finalize;
		}
		else {
			TheButtonMissionSettings.pool.RemoveAt(indexR);
			invalidRandomPoolEntries.Add(isoR);
			goto RandomPool;
		}

	// --------------------------------------------------------------------------------
	// If no extended mission settings are provided, or the fixed pool is depleted and there's no random pool, then use the player's own config
	// --------------------------------------------------------------------------------
	PreConfigFile:
		TheButtonMissionSettings.status = TheButtonMissionSettings.Status.ConfigFile;
		KMModule.Log("Resorting to player's personal config file to determine the remaining modules' languages.");
		KMModule.Log("--------------------------");
	ConfigFile:

		_settings = ReadConfig();

		string excludedNotInPool = "Languages ignored because the configuration file does not include them: ";
		string excludedNoManual = "Languages ignored because the configuration file dictates modules with manuals only: ";
		string includedSelection = "Languages available for selection: ";
		LanguageModule transl;
		List<LanguageModule> availableTranslations = new List<LanguageModule>();
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
			KMModule.Log(excludedNotInPool);
		else
			KMModule.Log("Configuration file dictates using any available language.");
		if (_settings.UseLanguagesWithManualOnly)
			KMModule.Log(excludedNoManual);
		else
			KMModule.Log("Configuration file allows for the use of languages without a dedicated manual.");
		KMModule.Log(includedSelection);

		LanguageModule selected;
		if (availableTranslations.Count == 0) {
			KMModule.Log("There were no languages available to be chosen for this module in accordance with the configuration file.");
			selected = _fallback;
		}
		else {
			int index = UnityEngine.Random.Range(0, availableTranslations.Count);
			selected = availableTranslations[index];
		}
		selected.Choose();
		Language = selected;

		// --------------------------------------------------------------------------------
		// Final steps
		// --------------------------------------------------------------------------------
	Finalize:
		KMModule.Log("--------------------------");

		// finalize selection
		KMModule.LogFormat("Selected Language: {0}, {1} ({2})\n", Language.NativeName, Language.Name, Language.Iso639);

		_sticker.GenerateText(Language.Iso639, Language.Version);
	}
}
