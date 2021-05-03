using UnityEngine;

public class TranslatedPassword : TranslatedModule<LanguagePassword> {
	[ContextMenu("Create Test Suite")]
	public void CreateTestSuite() {
		foreach (LanguagePassword lang in _languages) {
			var module = Instantiate(gameObject, transform.parent);
			module.name = "Password " + lang.Iso639.ToUpper();
			module.GetComponent<TranslatedPassword>()._languageOverride = lang;
		}
	}
}
