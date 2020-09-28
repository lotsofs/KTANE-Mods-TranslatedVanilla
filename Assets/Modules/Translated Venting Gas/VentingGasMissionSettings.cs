using System.Collections.Generic;
using UnityEngine;

public class VentingGasMissionSettings : TranslatedModulesMissionSettings {

	/// <summary>
	/// These languages will be selected first. When they're depleted, random languages will be selected.
	/// </summary>
	public string[] VentGasTranslated_FixedLanguages;
	/// <summary>
	/// Overrides the settings file. The mission will randomly pick modules from this array.
	/// </summary>
	public string[] VentGasTranslated_RandomLanguages;
	/// <summary>
	/// If set to true, random language pool will be depleted before duplicate languages are picked.
	/// </summary>
	public bool VentGasTranslated_AvoidDuplicates;
	/// <summary>
	/// This will shuffle the fixed languages before selecting from it. Useful for when the amount of modules on the bomb isn't fixed.
	/// </summary>
	public bool VentGasTranslated_ShuffleFixedLanguages;

	public override string[] FixedLanguages { get { return VentGasTranslated_FixedLanguages; } set { VentGasTranslated_FixedLanguages = value; } }
	public override string[] RandomLanguages { get { return VentGasTranslated_RandomLanguages; } set { VentGasTranslated_RandomLanguages = value; } }
	public override bool ShuffleFixedLanguages { get { return VentGasTranslated_ShuffleFixedLanguages; } set { VentGasTranslated_ShuffleFixedLanguages = value; } }
	public override bool AvoidDuplicates { get { return VentGasTranslated_AvoidDuplicates; } set { VentGasTranslated_AvoidDuplicates = value; } }
}
