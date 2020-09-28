using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Language for Vent Gas")]
public class LanguageVentingGas : Language {

	public enum DisplayMethods {
		Default,
		NonLatin,
		CustomTextMesh,
		Sprite
	}

	[Header("Display")]
	[Tooltip("Set to NonLatin if TMP_SubMesh throws a log stating a FallBackMaterial is being used. Set to CustomTextMesh if the text requires a custom font below. Set to sprite to use sprites.")]
	public DisplayMethods DisplayMethod = DisplayMethods.Default;

	[Header("Module Text")]
	public string VentGas = "VENT GAS?";
	public string Detonate = "DETONATE?";
	[TextArea]
	public string VentingComplete = "VENTING\nCOMPLETE";
	[TextArea]
	public string VentingPrevents = "VENTING\nPREVENTS\nEXPLOSIONS";
	
	[Space]
	public string YesNo = "Y/N";
	public string Yes = "YES";
	public string No = "NO";
	public string Y = "Y";
	public string N = "N";

	[Header("Log File Text")]
	public string LogPromptDetonate = "Prompt: Detonate?";
	public string LogPromptVentGas = "Prompt: Vent Gas?";

	[Space]

	public string LogTooLate = "Button wasn't pressed in time.";
	public string LogYesCorrect = "Pressed Yes. Disarmed!";
	public string LogYesIncorrect = "Pressed Yes. Strike!";
	public string LogNoCorrect = "Pressed No. Disarmed!";
	public string LogNoIncorrect = "Pressed No. Venting prevents explosions. Prompting again.";

	public override void Choose() {

	}
}
