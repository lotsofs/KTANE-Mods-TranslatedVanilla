using System;
using System.Reflection;
using UnityEngine;

public class Sticker : MonoBehaviour {

	[SerializeField] TextMesh _bigLabel;
	[SerializeField] TextMesh _blackBar;
	[SerializeField] string _PeriodicTableCode;

	const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

	public GameObject FindGlobalSticker(string iso, int langVersion, string periodicTableSymbol) {
		GameObject ts = GameObject.Find("TranslatedModulesService(Clone)");
		GameObject stickerObject = null;
		if (ts == null) {
			// translated modules service not installed
			return null;
		}
		try {
			Component service = ts.GetComponent("TranslatedModulesService");
			Type type = service.GetType();
			FieldInfo fieldSticker = type.GetField("Sticker");
			GameObject sticker = (GameObject)fieldSticker.GetValue(service);

			stickerObject = GameObject.Instantiate(sticker);

			Component stickerComponent = stickerObject.GetComponent("StickerGlobal");
			Type typeSticker = stickerComponent.GetType();
			MethodInfo stickerMethod = typeSticker.GetMethod("GenerateText", new Type[] { typeof(string), typeof(int), typeof(string) });
			stickerMethod.Invoke(stickerComponent, new object[] { iso, langVersion, periodicTableSymbol });
			return stickerObject;
		}
		catch (Exception e) {
			Debug.Log(e.Message);
			if (stickerObject != null) {
				GameObject.Destroy(stickerObject);
			}
			return null;
		}
	}

	/// <summary>
	/// Adds some aesthetics to the sticker
	/// </summary>
	public void GenerateText(string iso, int langVersion) {
		GameObject sticker = FindGlobalSticker(iso, langVersion, _PeriodicTableCode);
		if (sticker != null) {
			this.transform.GetChild(0).gameObject.SetActive(false);
			sticker.transform.SetParent(this.transform.parent);
			sticker.transform.localPosition = this.transform.localPosition;
			sticker.transform.localRotation = this.transform.localRotation;
			sticker.transform.localScale = this.transform.localScale;
			return;
		}

		_bigLabel.text = iso.ToUpperInvariant();

		string hexPTC = "";
		foreach (char c in _PeriodicTableCode) {
			hexPTC += Convert.ToInt32(c).ToString("X");
		}

		string randomChars = "";
		for (int i = 0; i < 4; i++) {
			randomChars += chars[UnityEngine.Random.Range(0, chars.Length)];
		}

		_blackBar.text = string.Format("TLM{0}.{1}.{2}{3}{4}",
			hexPTC,
			randomChars,
			iso.ToUpperInvariant(),
			UnityEngine.Random.Range(0, 1000).ToString("000"),
			langVersion.ToString()
		);
	}
}
