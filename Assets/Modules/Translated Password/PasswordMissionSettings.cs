public class PasswordMissionSettings : TranslatedModulesMissionSettings {

	/// <summary>
	/// These languages will be selected first. When they're depleted, random languages will be selected.
	/// </summary>
	public string[] PasswordsTranslated_FixedLanguages;
	/// <summary>
	/// Overrides the settings file. The mission will randomly pick modules from this array.
	/// </summary>
	public string[] PasswordsTranslated_RandomLanguages;
	/// <summary>
	/// If set to true, random language pool will be depleted before duplicate languages are picked.
	/// </summary>
	public bool PasswordsTranslated_AvoidDuplicates;
	/// <summary>
	/// This will shuffle the fixed languages before selecting from it. Useful for when the amount of modules on the bomb isn't fixed.
	/// </summary>
	public bool PasswordsTranslated_ShuffleFixedLanguages;

	public override string[] FixedLanguages { get { return PasswordsTranslated_FixedLanguages; } set { PasswordsTranslated_FixedLanguages = value; } }
	public override string[] RandomLanguages { get { return PasswordsTranslated_RandomLanguages; } set { PasswordsTranslated_RandomLanguages = value; } }
	public override bool ShuffleFixedLanguages { get { return PasswordsTranslated_ShuffleFixedLanguages; } set { PasswordsTranslated_ShuffleFixedLanguages = value; } }
	public override bool AvoidDuplicates { get { return PasswordsTranslated_AvoidDuplicates; } set { PasswordsTranslated_AvoidDuplicates = value; } }
}
