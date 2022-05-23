using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceConnector : MonoBehaviour {
	private static GameObject _serviceObject;

	public void GetService() {
		_serviceObject = GameObject.Find("TranslatedModulesService(Clone)");
		GameObject stickerObject = null;
		if (_serviceObject == null) {
			// translated modules service not installed
			return null;
		}
	}

	public static GameObject GetSticker() {

	}
}
