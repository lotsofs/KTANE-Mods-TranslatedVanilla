using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Collections;

public class Language : ScriptableObject {

	[Tooltip("Disabled languages are never chosen, eg. because they're still a work in progress.")]
	public bool Disabled = false;

	[Header("Language Information")]
	public string Iso639;
	public string Name;         // todo: this should be obsolete
	public string NativeName;   // todo: this should be obsolete
	[Tooltip("The unity editor does not properly show right to left languages, showing them as left to right instead. If this happens on the actual text mesh too, the letters must be manually switched around 'siht ekil'. Tick this to make the mod do that for you. This will not change anything in the editor and it will maintain the incorrect left-to-right look. It is recommended to copy paste the texts into a text editor and edit them there. This does not do anything in the editor.")]
	public bool RightToLeft = false;
	[Space]
	public int Version = 1;
	public bool ManualAvailable = false;
	public string[] ManualLinks;
	[Space]
	public string TwitchHelpMessage = "!{0} twitch help message";

	[Header("Optional IETF BCP 47 Language Tag Information")]
	public string ExtLang = "";
	public Ietf.Scripts Script = Ietf.Scripts.Default;
	public Ietf.Regions LanguageRegion = Ietf.Regions.Default;
	[Tooltip("Two letter country code. If set, overrides the Language Region set above.")]
	public string Iso3166 = "";
	public string Variant = "";
	public bool MachineTranslation = false;
	public string MachineUsed = "";
	public string[] AdditionalExtendedSubtags;
	public bool ShowVersionSubtag = false;
	public string[] AdditionalPrivateSubtags;
	[Space]
	public string IetfBcp47 = "";

	/// <summary>
	/// a bool that denotes whether right to left text has been flipped in this game session. We don't want to flip it a second time for this would put it back in the wrong order.
	/// </summary>
	internal bool flipped = false;

	[ContextMenu("Auto Complete")]
	public void AutoComplete() {
		if (Ietf.Languages.ContainsKey(Iso639)) {
			if (string.IsNullOrEmpty(Name)) Name = Ietf.Languages[Iso639].Anglonym;
			if (string.IsNullOrEmpty(NativeName)) NativeName = Ietf.Languages[Iso639].Endonym;
			if (Ietf.Languages[Iso639].RightToLeft) RightToLeft = true; // only set to true if definitely true since false could be 'varies' as well
		}

		IetfBcp47 = Iso639;
		if (IetfBcp47.Length < 2) {
			return;
		}
		if (!string.IsNullOrEmpty(ExtLang)) IetfBcp47 += "-" + ExtLang.ToLower();
		if (Script != Ietf.Scripts.Default && Script.ToString() != Ietf.Languages[Iso639].SuppressScript ) IetfBcp47 += "-" + (Ietf.ScriptCodes)Script;
		if (!string.IsNullOrEmpty(Iso3166)) IetfBcp47 += "-" + Iso3166.ToUpper();
		else if (LanguageRegion != Ietf.Regions.Default) IetfBcp47 += "-" + LanguageRegion;
		if (!string.IsNullOrEmpty(Variant)) IetfBcp47 += "-" + Variant.ToLower();
		if (MachineTranslation) IetfBcp47 += "-t-t0-" + (string.IsNullOrEmpty(MachineUsed) ? MachineUsed : "und");
		foreach (string subtag in AdditionalExtendedSubtags) IetfBcp47 += "-" + subtag;
		if (AdditionalPrivateSubtags.Length > 0 || ShowVersionSubtag) IetfBcp47 += "-x";
		foreach (string subtag in AdditionalPrivateSubtags) IetfBcp47 += "-" + subtag;
		if (ShowVersionSubtag) IetfBcp47 += "-v" + Version;
	}

	//public abstract string GetLabelFromEnglishName(string str);

	//public abstract string GetLogFromEnglishName(string str);

	//public abstract Sprite GetSpriteFromEnglishName(string str);

	//public abstract int GetSizeFromEnglishName(string str);

	internal string ReverseReadingDirection(string str) {
		string[] splits = str.Split(new[] { Environment.NewLine, "\n" }, StringSplitOptions.None);
		for (int i = 0; i < splits.Length; i++) {
			IEnumerable<char> chars = splits[i].Reverse();
			splits[i] = new string(chars.ToArray());
		}
		return string.Join(Environment.NewLine, splits);
	}
}
