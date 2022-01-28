using UnityEngine;

public class TranslatedMorseCode : TranslatedModule<LanguageMorseCode> {
	[ContextMenu("Create Test Suite")]
	public void CreateTestSuite() {
		foreach (LanguageMorseCode lang in _languages) {
			var module = Instantiate(gameObject, transform.parent);
			module.name = "Morse Code " + lang.Iso639.ToUpper();
			module.GetComponent<TranslatedMorseCode>()._languageOverride = lang;
		}
	}
}
