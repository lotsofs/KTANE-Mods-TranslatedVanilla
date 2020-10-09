using System.Collections.Generic;
using UnityEngine;

public class PasswordMissionSettings : TranslatedModulesMissionSettings {

	/// <summary>
	/// These languages will be selected first. When they're depleted, random languages will be selected.
	/// </summary>
	public string[] PasswordTranslated_FixedLanguages;
	/// <summary>
	/// Overrides the settings file. The mission will randomly pick modules from this array.
	/// </summary>
	public string[] PasswordTranslated_RandomLanguages;
	/// <summary>
	/// If set to true, random language pool will be depleted before duplicate languages are picked.
	/// </summary>
	public bool PasswordTranslated_AvoidDuplicates;
	/// <summary>
	/// This will shuffle the fixed languages before selecting from it. Useful for when the amount of modules on the bomb isn't fixed.
	/// </summary>
	public bool PasswordTranslated_ShuffleFixedLanguages;

	public override string[] FixedLanguages { get { return PasswordTranslated_FixedLanguages; } set { PasswordTranslated_FixedLanguages = value; } }
	public override string[] RandomLanguages { get { return PasswordTranslated_RandomLanguages; } set { PasswordTranslated_RandomLanguages = value; } }
	public override bool ShuffleFixedLanguages { get { return PasswordTranslated_ShuffleFixedLanguages; } set { PasswordTranslated_ShuffleFixedLanguages = value; } }
	public override bool AvoidDuplicates { get { return PasswordTranslated_AvoidDuplicates; } set { PasswordTranslated_AvoidDuplicates = value; } }
}
