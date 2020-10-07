using UnityEngine;

public class TranslatedVentingGas : TranslatedModule<LanguageVentingGas, VentingGasMissionSettings> {
	[ContextMenu("Create Test Suite")]
	public void CreateTestSuite() {
		foreach (LanguageVentingGas lang in _languages) {
			var module = Instantiate(gameObject, transform.parent);
			module.name = "Venting Gas " + lang.Iso639.ToUpper();
			module.GetComponent<TranslatedVentingGas>()._languageOverride = lang;
		}
	}
}
