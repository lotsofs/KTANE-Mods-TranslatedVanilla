using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EMS Language Pool")]
public class EMSLanguagesPool : ScriptableObject {
	public List<string> Pool;
	public string BombSerial;

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

	public void Purge() {
		Pool = new List<string>();
		FixedLanguages = new List<string>();
		RandomLanguages = new List<string>();
		ShuffleFixedLanguages = false;
		AvoidDuplicates = false;
		status = Statuses.Uninitialized;
	}
}
