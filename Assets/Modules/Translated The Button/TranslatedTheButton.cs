using UnityEngine;

public class TranslatedTheButton : TranslatedModule<LanguageTheButton> {
	[ContextMenu("Create Test Suite")]
	public void CreateTestSuite() {
		foreach (LanguageTheButton lang in _languages) {
			var module = Instantiate(gameObject, transform.parent);
			module.name = "The Button " + lang.Iso639.ToUpper();
			module.GetComponent<TranslatedTheButton>()._languageOverride = lang;
		}
	}
}
