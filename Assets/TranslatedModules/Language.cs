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

	public abstract void Choose();

	internal bool _flipped = false;

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
		_flipped = true;
		return string.Join(Environment.NewLine, splits);
	}
}
