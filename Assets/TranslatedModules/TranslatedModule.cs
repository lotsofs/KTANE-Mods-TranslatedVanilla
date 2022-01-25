using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using KModkit;

public class TranslatedModule<TLanguage> : MonoBehaviour
	where TLanguage : Language
	//where TExtendedMissionSettings : TranslatedModulesMissionSettings 
	{

	public TLanguage Language { get {
			if (_language == null) {
				GenerateLanguage(this.name);
			}
			return _language;
		} 
	}

	TLanguage _language;
	string _moduleLogName;
	string _moduleType;
	TranslationSettings _settings;

	[SerializeField] protected TLanguage[] _languages;
	[SerializeField] TLanguage _fallbackLanguage;
	[SerializeField] Sticker _sticker;
	[SerializeField] KMExtendedMissionSettings _extendedMissionSettings;
	[SerializeField] EMSLanguagesPool _languagePool;
	[Space]
	[SerializeField] string _settingsFileName;

	[Header("Debug")]
	[SerializeField] protected TLanguage _languageOverride;


	void Awake() {
		if (GetComponent<KMBombModule>() != null) {
			_moduleLogName = GetComponent<KMBombModule>().ModuleDisplayName;
			_moduleType = GetComponent<KMBombModule>().ModuleType;
		}
		else {
			_moduleLogName = GetComponent<KMNeedyModule>().ModuleDisplayName;
			_moduleType = GetComponent<KMNeedyModule>().ModuleType;
		}
	}

	void Start() {
		//SetLanguage();			// uncomment to call automatically. Commented here so ModuleLogName can be set first and then SetLanguage can be done automatically.
	}

	void OnDestroy() {
		_languagePool.Purge();
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
	bool ReadExtendedMissionSettings() {
		_languagePool.FixedLanguages = _extendedMissionSettings.GetStringListSetting(_moduleType + "_FixedLanguages");
		_languagePool.RandomLanguages = _extendedMissionSettings.GetStringListSetting(_moduleType + "_RandomLanguages");
		_languagePool.ShuffleFixedLanguages = _extendedMissionSettings.GetBoolSetting(_moduleType + "_ShuffleFixedLanguages");
		_languagePool.AvoidDuplicates = _extendedMissionSettings.GetBoolSetting(_moduleType + "_AvoidDuplicates");
		_languagePool.Pool = _languagePool.FixedLanguages != null ? _languagePool.FixedLanguages.ToList() : new List<string>();
		return ((_languagePool.FixedLanguages != null && _languagePool.FixedLanguages.Count > 0) || (_languagePool.RandomLanguages != null && _languagePool.RandomLanguages.Count > 0));
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

		TLanguage lang = CheckLanguagePool();

		if (lang != null) {
			UseLanguage(lang);
			return;
		}
		//lang = CheckLanguagePool(_languagePoolGlobal);
		//if (lang != null) {
		//	UseLanguage(lang);
		//	return;
		//}
		LogFormat("WARNING: Could not find a language to be used. Using a fallback language.");
		UseLanguage(_fallbackLanguage);
	}
	
	TLanguage CheckLanguagePool() {
		TLanguage lang;
		// First check from the extended mission settings

		// first we will check if a language pool has been generated for this bomb, by comparing the serial number of the current bomb with
		// the stored serial number in the pool. If they don't match, we need to get the pool. If they match, it's already been done by another module.
		string serial = GetComponent<KMBombInfo>().GetSerialNumber();
		if (serial != _languagePool.BombSerial) {
			// not read yet. Do it now.
			_languagePool.Purge();
			_languagePool.BombSerial = serial;
			if (ReadExtendedMissionSettings()) {
				_languagePool.status = EMSLanguagesPool.Statuses.FixedPool;
			}
			else {
				// nothing read. Use config file.
				_languagePool.status = EMSLanguagesPool.Statuses.ConfigFile;
			}
		}

		switch (_languagePool.status) {
			case EMSLanguagesPool.Statuses.FixedPool:
				lang = PickLanguageFromFixedPool();
				if (lang == null) {
					_languagePool.status = EMSLanguagesPool.Statuses.RandomPool;
					goto case EMSLanguagesPool.Statuses.RandomPool;
				}
				else {
					break;
				}
			case EMSLanguagesPool.Statuses.RandomPool:
				lang = PickLanguageFromRandomPool();
				if (lang == null) {
					_languagePool.status = EMSLanguagesPool.Statuses.ConfigFile;
					goto case EMSLanguagesPool.Statuses.ConfigFile;
				}
				else {
					break;
				}
			case EMSLanguagesPool.Statuses.ConfigFile:
				lang = PickLanguageFromConfigFile();
				break;
			default:
				lang = null;
				break;
		}
		return lang;
	}

	/// <summary>
		/// Picks a language according to the EMS's fixed pool
		/// </summary>
		/// <returns></returns>
	TLanguage PickLanguageFromFixedPool() {
		Log("Checking fixed pool from extended mission settings.");
		int retry = 0;
		while (retry < 100) {
			retry++;
			// check if pool even was provided
			if (_languagePool.FixedLanguages == null || _languagePool.FixedLanguages.Count == 0) {
				Log("No fixed pool provided.");
				return null;
			}

			// check if pool's depleted
			if (_languagePool.Pool.Count == 0) {
				Log("Fixed pool depleted.");
				return null;
			}

			// pick from pool
			int index = _languagePool.ShuffleFixedLanguages ? UnityEngine.Random.Range(0, _languagePool.Pool.Count) : 0;
			string isoCode = _languagePool.Pool[index];
			TLanguage chosenLanguage = FindLanguage(isoCode);
			_languagePool.Pool.RemoveAt(index);
			if (chosenLanguage != null) {
				Log("Succesfully picked language from extended mission settings fixed pool.");
				return chosenLanguage;
			}
		}
		Log("Picking language from fixed pool failed (tried " + retry + " times).");
		return null;
	}

	/// <summary>
	/// Picks a language according to the random pool from EMS
	/// </summary>
	/// <returns></returns>
	TLanguage PickLanguageFromRandomPool() {
		Log("Checking random pool from extended mission settings.");
		List<string> invalidRandomPoolEntries = new List<string>();
		int retry = 0;
		while (retry < 100) {
			retry++;
			// check if pool even was provided
			if (_languagePool.RandomLanguages == null || _languagePool.RandomLanguages.Count == 0) {
				Log("No random pool provided.");
				return null;
			}

			// check if the entire pool is invalid
			if (invalidRandomPoolEntries.Count >= _languagePool.RandomLanguages.Count) {
				Log("There are no valid entries in the random pool.");
				return null;
			}

			// check if working pool has been depleted
			if (_languagePool.Pool == null || _languagePool.Pool.Count == 0) {
				Log("Random pool depleted. Refilling.");
				_languagePool.Pool = _languagePool.RandomLanguages.ToList();
				invalidRandomPoolEntries.Clear();
				continue;
			}

			// pick from pool
			int index = UnityEngine.Random.Range(0, _languagePool.Pool.Count);
			string isoCode = _languagePool.Pool[index];
			TLanguage chosenLanguage = FindLanguage(isoCode);
			if (_languagePool.AvoidDuplicates) {
				_languagePool.Pool.RemoveAt(index);
			}
			if (chosenLanguage != null) {
				Log("Succesfully picked module from extended mission settings random pool.");
				return chosenLanguage;
			}
			else {
				_languagePool.Pool.RemoveAt(index);
				invalidRandomPoolEntries.Add(isoCode);
			}
		}
		Log("Picking language from random pool failed (tried " + retry + " times).");
		return null;
	}

	/// <summary>
	/// Picks a language according to the player's config file
	/// </summary>
	/// <returns></returns>
	TLanguage PickLanguageFromConfigFile() {
		Log("Using player's personal config file to determine the module' language.");
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
						excludedUntranslated += string.Format("{0}, ", transl.IetfBcp47);
					}
					if (_settings.IgnoredPrivateSubtags.Contains(subtag, StringComparer.OrdinalIgnoreCase)) {
						LogFormat("Ignoring language {0} because its ietf bcp 47 tag contains illegal private use subtag '{1}'", transl.IetfBcp47, subtag);
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

		if (!_settings.UseAllLanguages && excludedNotInPool.Trim().Last() != ':') 
			Log(excludedNotInPool);
		else 
			Log("Configuration file dictates using any available language.");

		if (_settings.DisabledLanguages != null && excludedIgnored.Trim().Last() != ':') 
			Log(excludedIgnored);

		if (_settings.IgnoreWithoutManual && excludedNoManual.Trim().Last() != ':') 
			Log(excludedNoManual);
		else 
			Log("Configuration file allows for the use of translations without a dedicated manual.");

		if (_settings.IgnoreMachineTranslations && excludedMachine.Trim().Last() != ':') 
			Log(excludedMachine);

		if (_settings.IgnoredPrivateSubtags.Contains("untc", StringComparer.OrdinalIgnoreCase) && excludedUntranslated.Trim().Last() != ':') 
			Log(excludedUntranslated);
		else if (hasUnfinishedLanguages) 
			Log("Configuration file allows for the use of translations that didn't bother to translate the main content of the module.");


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

