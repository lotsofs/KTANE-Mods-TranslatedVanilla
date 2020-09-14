using System.Collections.Generic;

public class TheButtonMissionSettings : TranslatedVanillaMissionSettings {

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

	#region not actual settings
	public static List<string> pool;
	
	public enum Status {
		FixedPool,
		RandomPool,
		ConfigFile,
		SelectingFinished
	}
	public static Status status;

	#endregion
}
