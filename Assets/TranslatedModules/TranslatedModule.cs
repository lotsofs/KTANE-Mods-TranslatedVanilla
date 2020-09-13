using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NotVanillaModulesLib;

public class TranslatedModule : MonoBehaviour {

	const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

	[SerializeField] GameObject _languagesHolder;

	public NotVanillaModuleConnector KMModule;
	[SerializeField] Sticker _sticker;
	[SerializeField] string _settingsFileName;
	[SerializeField] string[] _oldSettingsFiles;

	[NonSerialized] public Translation Language;
	[Header("Debug")]
	public Translation Override;

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

	public void SelectLanguage() {
		_settings = ReadConfig();

		string includedSelection = "Languages available for selection: ";
		string excludedNotInPool = "Languages ignored because the configuration file does not include them: ";
		string excludedNoManual = "Languages ignored because the configuration file dictates modules with manuals only: ";
		Translation transl;
		List<Translation> availableTranslations = new List<Translation>();
		for (int i = _languagesHolder.transform.childCount - 1; i >= 0; i--) {
			transl = _languagesHolder.transform.GetChild(i).GetComponent<Translation>();
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

		if (availableTranslations.Count == 0) {
			KMModule.Log("There were no languages available to be chosen for this module in accordance with the configuration file.");
			Language = _languagesHolder.transform.Find("Default").GetComponent<Translation>();
		}
		else {
			int index = UnityEngine.Random.Range(0, availableTranslations.Count);
			Language = availableTranslations[index];
		}

		// debug
		if (Override != null) {
			KMModule.LogFormat("DEBUG: Language overridden to {0}.", Override.Iso639);
			Language = Override;
		}

		// finalize selection
		Language.Choose();
		KMModule.LogFormat("Selected Language: {0}, {1} ({2})\n", Language.NativeName, Language.Name, Language.Iso639);

		_sticker.GenerateText(Language.Iso639, Language.Version);
	}
}
