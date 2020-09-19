using System.Collections.Generic;
using UnityEngine;

public class TheButtonMissionSettings : TranslatedModulesMissionSettings {

	/// <summary>
	/// These languages will be selected first. When they're depleted, random languages will be selected.
	/// </summary>
	public string[] BigButtonTranslated_FixedLanguages;
	/// <summary>
	/// Overrides the settings file. The mission will randomly pick modules from this array.
	/// </summary>
	public string[] BigButtonTranslated_RandomLanguages;
	/// <summary>
	/// If set to true, random language pool will be depleted before duplicate languages are picked.
	/// </summary>
	public bool BigButtonTranslated_AvoidDuplicates;
	/// <summary>
	/// This will shuffle the fixed languages before selecting from it. Useful for when the amount of modules on the bomb isn't fixed.
	/// </summary>
	public bool BigButtonTranslated_ShuffleFixedLanguages;

	public override string[] FixedLanguages { get { return BigButtonTranslated_FixedLanguages; } set { BigButtonTranslated_FixedLanguages = value; } }
	public override string[] RandomLanguages { get { return BigButtonTranslated_RandomLanguages; } set { BigButtonTranslated_RandomLanguages = value; } }
	public override bool ShuffleFixedLanguages { get { return BigButtonTranslated_ShuffleFixedLanguages; } set { BigButtonTranslated_ShuffleFixedLanguages = value; } }
	public override bool AvoidDuplicates { get { return BigButtonTranslated_AvoidDuplicates; } set { BigButtonTranslated_AvoidDuplicates = value; } }
}
