﻿using UnityEngine;

public class TranslatedWhosOnFirst : TranslatedModule<LanguageWhosOnFirst, WhosOnFirstMissionSettings> {
	[ContextMenu("Create Test Suite")]
	public void CreateTestSuite() {
		foreach (LanguageWhosOnFirst lang in _languages) {
			var module = Instantiate(gameObject, transform.parent);
			module.name = "Password " + lang.Iso639.ToUpper();
			module.GetComponent<TranslatedWhosOnFirst>()._languageOverride = lang;
		}
	}
}
