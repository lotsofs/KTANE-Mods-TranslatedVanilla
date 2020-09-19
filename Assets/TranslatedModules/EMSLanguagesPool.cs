using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EMS Language Pool")]
public class EMSLanguagesPool : ScriptableObject {
	public List<string> pool;

	public enum Statuses {
		FixedPool,
		RandomPool,
		ConfigFile,
		Uninitialized
	}
	public Statuses status;

	public string[] FixedLanguages;
	public string[] RandomLanguages;
	public bool ShuffleFixedLanguages;
	public bool AvoidDuplicates;
}
