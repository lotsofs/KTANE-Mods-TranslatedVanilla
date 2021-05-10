using UnityEngine;

public class TranslatedWhosOnFirst : TranslatedModule<LanguageWhosOnFirst> {
	[ContextMenu("Create Test Suite")]
	public void CreateTestSuite() {
		foreach (LanguageWhosOnFirst lang in _languages) {
			var module = Instantiate(gameObject, transform.parent);
			module.name = "Who's on First " + lang.Iso639.ToUpper();
			module.GetComponent<TranslatedWhosOnFirst>()._languageOverride = lang;
		}
	}
}
