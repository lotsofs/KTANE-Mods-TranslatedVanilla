using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EMS Language Pool")]
public class EMSLanguagesPool : ScriptableObject {
	public List<string> pool;

	public enum Statuses {
		Uninitialized,
		FixedPool,
		RandomPool,
		ConfigFile,
	}
	public Statuses status;

	public List<string> FixedLanguages;
	public List<string> RandomLanguages;
	public bool ShuffleFixedLanguages;
	public bool AvoidDuplicates;
}
