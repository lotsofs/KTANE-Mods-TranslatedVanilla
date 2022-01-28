public class MorseCodeMissionSettings : TranslatedModulesMissionSettings {

	/// <summary>
	/// These languages will be selected first. When they're depleted, random languages will be selected.
	/// </summary>
	public string[] MorseCodeTranslated_FixedLanguages;
	/// <summary>
	/// Overrides the settings file. The mission will randomly pick modules from this array.
	/// </summary>
	public string[] MorseCodeTranslated_RandomLanguages;
	/// <summary>
	/// If set to true, random language pool will be depleted before duplicate languages are picked.
	/// </summary>
	public bool MorseCodeTranslated_AvoidDuplicates;
	/// <summary>
	/// This will shuffle the fixed languages before selecting from it. Useful for when the amount of modules on the bomb isn't fixed.
	/// </summary>
	public bool MorseCodeTranslated_ShuffleFixedLanguages;

	public override string[] FixedLanguages { get { return MorseCodeTranslated_FixedLanguages; } set { MorseCodeTranslated_FixedLanguages = value; } }
	public override string[] RandomLanguages { get { return MorseCodeTranslated_RandomLanguages; } set { MorseCodeTranslated_RandomLanguages = value; } }
	public override bool ShuffleFixedLanguages { get { return MorseCodeTranslated_ShuffleFixedLanguages; } set { MorseCodeTranslated_ShuffleFixedLanguages = value; } }
	public override bool AvoidDuplicates { get { return MorseCodeTranslated_AvoidDuplicates; } set { MorseCodeTranslated_AvoidDuplicates = value; } }
}
