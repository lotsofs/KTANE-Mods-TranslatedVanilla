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

	//public abstract string GetLabelFromEnglishName(string str);

	//public abstract string GetLogFromEnglishName(string str);

	//public abstract Sprite GetSpriteFromEnglishName(string str);

	//public abstract int GetSizeFromEnglishName(string str);
}
