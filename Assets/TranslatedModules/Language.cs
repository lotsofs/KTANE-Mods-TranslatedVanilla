using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Language : ScriptableObject {

	[Tooltip("Disabled languages are never chosen, eg. because they're still a work in progress.")]
	public bool Disabled = false;
	[Header("Language Information")]
	public string Name;
	public string NativeName;
	public string Iso639;
	public int Version = 1;
	public bool ManualAvailable = false;
	[Space]
	[Tooltip("The unity editor does not properly show right to left languages, showing them as left to right instead. If this happens on the actual text mesh too, the letters must be manually switched around 'siht ekil'. Tick this to make the mod do that for you. This will not change anything in the editor and it will maintain the incorrect left-to-right look. It is recommended to copy paste the texts into a text editor and edit them there. This does not do anything in the editor.")]
	public bool RightToLeft = false;

	public abstract void Choose();

	internal bool Flipped = false;

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
		Flipped = true;
		return string.Join(Environment.NewLine, splits);
	}
}
