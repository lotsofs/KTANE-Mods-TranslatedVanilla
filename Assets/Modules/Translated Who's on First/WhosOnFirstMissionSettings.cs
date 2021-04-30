public class WhosOnFirstMissionSettings : TranslatedModulesMissionSettings {

	/// <summary>
	/// These languages will be selected first. When they're depleted, random languages will be selected.
	/// </summary>
	public string[] WhosOnFirstTranslated_FixedLanguages;
	/// <summary>
	/// Overrides the settings file. The mission will randomly pick modules from this array.
	/// </summary>
	public string[] WhosOnFirstTranslated_RandomLanguages;
	/// <summary>
	/// If set to true, random language pool will be depleted before duplicate languages are picked.
	/// </summary>
	public bool WhosOnFirstTranslated_AvoidDuplicates;
	/// <summary>
	/// This will shuffle the fixed languages before selecting from it. Useful for when the amount of modules on the bomb isn't fixed.
	/// </summary>
	public bool WhosOnFirstTranslated_ShuffleFixedLanguages;

	public override string[] FixedLanguages { get { return WhosOnFirstTranslated_FixedLanguages; } set { WhosOnFirstTranslated_FixedLanguages = value; } }
	public override string[] RandomLanguages { get { return WhosOnFirstTranslated_RandomLanguages; } set { WhosOnFirstTranslated_RandomLanguages = value; } }
	public override bool ShuffleFixedLanguages { get { return WhosOnFirstTranslated_ShuffleFixedLanguages; } set { WhosOnFirstTranslated_ShuffleFixedLanguages = value; } }
	public override bool AvoidDuplicates { get { return WhosOnFirstTranslated_AvoidDuplicates; } set { WhosOnFirstTranslated_AvoidDuplicates = value; } }
}
