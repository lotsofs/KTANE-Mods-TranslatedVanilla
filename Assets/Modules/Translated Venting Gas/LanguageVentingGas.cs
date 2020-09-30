using UnityEngine;

[CreateAssetMenu(fileName = "Language for Vent Gas")]
public class LanguageVentingGas : Language {

	public enum DisplayMethods {
		Default,
		//FallbackFont,
		CustomTextMesh,
		Sprite
	}

	[Header("Display")]
	[Tooltip("Set to NonLatin if TMP_SubMesh throws a log stating a FallBackMaterial is being used. Set to CustomTextMesh if the text requires a custom font below. Set to sprite to use sprites.")]
	public DisplayMethods DisplayMethod = DisplayMethods.Default;

	[Tooltip("The unity editor does not properly show right to left languages, showing them as left to right instead. If this happens on the actual text mesh too, the letters must be manually switched around 'siht ekil'. Tick this to make the mod do that for you. This will not change anything in the editor and it will maintain the incorrect left-to-right look. It is recommended to copy paste the texts into a text editor and edit them there. This does not do anything in the editor.")]
	public bool RightToLeft = false;

	[Header("Module Text")]
	[TextArea] public string VentGas = "VENT GAS?";
	[TextArea] public string Detonate = "DETONATE?";
	[TextArea] public string VentingComplete = "VENTING\nCOMPLETE";
	[TextArea] public string VentingPrevents = "VENTING\nPREVENTS\nEXPLOSIONS";

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
		if (RightToLeft && !_flipped && !Application.isEditor) {
			VentGas = ReverseReadingDirection(VentGas);
			Detonate = ReverseReadingDirection(Detonate);
			VentingComplete = ReverseReadingDirection(VentingComplete);
			VentingPrevents = ReverseReadingDirection(VentingPrevents);
			YesNo = ReverseReadingDirection(YesNo);
			Yes = ReverseReadingDirection(Yes);
			No = ReverseReadingDirection(No);
			Y = ReverseReadingDirection(Y);
			N = ReverseReadingDirection(N);
		}
	}
}
