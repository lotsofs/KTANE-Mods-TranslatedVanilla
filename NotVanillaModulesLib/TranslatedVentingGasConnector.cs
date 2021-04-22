using System;
using TranslatedVanillaModulesLib.TestModel;
using UnityEngine;
#if (!DEBUG)
using TMPro;
#endif

namespace TranslatedVanillaModulesLib {
	public class TranslatedVentingGasConnector : TranslatedVanillaModuleConnector {
		public enum Texts {
			VentGas,
			VentYN,
			Detonate,
			DetonateYN,
			VentingPreventsExplosions,
			VentingComplete,
			InputText,
		}

		public GameObject TestModelDisplayBase;
		public TextMesh[] TestModelDisplayTexts;
		public TestModelButton[] TestModelButtons;
		public TextMesh[] TestModelButtonLabels;

#if (!DEBUG)
		private TextMeshPro[] displayTexts;
		private KeypadButton[] buttons;
#endif
		private bool _useCustomDisplay = false;

		public string InputText {
			get {
#if (!DEBUG)
				if (!this.TestMode && !_useCustomDisplay) return this.displayTexts[(int)Texts.InputText].text;
#endif
				return this.TestModelDisplayTexts[(int)Texts.InputText].text;
			}
			set {
				if (!this.TestMode && !_useCustomDisplay) {
#if (!DEBUG)
					this.displayTexts[(int)Texts.InputText].text = value;
#endif
				}
				else this.TestModelDisplayTexts[(int)Texts.InputText].text = value;
			}
		}

		public void SetDisplayActive(Texts display, bool active) {
			if (!this.TestMode && !_useCustomDisplay) {
#if (!DEBUG)
				displayTexts[(int)display].gameObject.SetActive(active);
#endif
			}
			else {
				this.TestModelDisplayTexts[(int)display].gameObject.SetActive(active);
			}
		}

		public void SetDisplayText(Texts display, string text) {
			if (!this.TestMode && !_useCustomDisplay) {
#if (!DEBUG)
				displayTexts[(int)display].text = text;
#endif
			}
			else {
				this.TestModelDisplayTexts[(int)display].text = text;
			}
		}

		public void DisableDisplay() {
			if (!this.TestMode && !_useCustomDisplay) {
#if (!DEBUG)
				foreach (var text in this.displayTexts) {
					text.gameObject.SetActive(false);
				}
#endif
			}
			else {
				foreach (var text in this.TestModelDisplayTexts) {
					text.gameObject.SetActive(false);
				}
			}
		}

		public event EventHandler<VentingGasButtonEventArgs> ButtonPressed;

		public void SetButtonTexts(string yes, string no) {
			if (!this.TestMode && !_useCustomDisplay) {
#if (!DEBUG)
				buttons[0].GetComponentInChildren<TextMeshPro>().text = no;
				buttons[1].GetComponentInChildren<TextMeshPro>().text = yes;
#endif
			}
			TestModelButtonLabels[0].text = no;
			TestModelButtonLabels[1].text = yes;
		}
		
		public void UseCustomDisplay() {
			_useCustomDisplay = true;
#if (!DEBUG)
			foreach (TextMeshPro tmp in displayTexts) {
				tmp.gameObject.SetActive(false);
			}
			for (int i = 0; i < TestModelDisplayTexts.Length; i++) {
				TestModelDisplayTexts[i].transform.SetParent(this.transform, true);
				TestModelDisplayTexts[i].gameObject.SetActive(false);
			}
			for (int i = 0; i < buttons.Length; i++) {
				TestModelButtonLabels[i].transform.SetParent(buttons[i].transform, true);
				buttons[i].GetComponentInChildren<TextMeshPro>().gameObject.SetActive(false);
			}
#endif
		}

		protected override void AwakeLive() {
#if (!DEBUG)
			using var wrapper = this.InstantiateComponent<NeedyVentComponent>();
			wrapper.Component.transform.Find("Component_Needy_VentGas").SetParent(this.transform, false);

			this.displayTexts = new TextMeshPro[7];
			displayTexts[(int)Texts.VentGas] = wrapper.Component.VentText.transform.Find("VentGas").GetComponent<TextMeshPro>();
			displayTexts[(int)Texts.VentYN] = wrapper.Component.VentText.transform.Find("VentYN").GetComponent<TextMeshPro>();
			displayTexts[(int)Texts.Detonate] = wrapper.Component.DetonateText.transform.Find("Detonate").GetComponent<TextMeshPro>();
			displayTexts[(int)Texts.DetonateYN] = wrapper.Component.DetonateText.transform.Find("DetonateYN").GetComponent<TextMeshPro>();
			displayTexts[(int)Texts.VentingPreventsExplosions] = wrapper.Component.PreventsText.transform.Find("VentingPreventsExplosions").GetComponent<TextMeshPro>();
			displayTexts[(int)Texts.VentingComplete] = wrapper.Component.VentingCompleteText.transform.Find("VentingComplete").GetComponent<TextMeshPro>();
			displayTexts[(int)Texts.InputText] = wrapper.Component.InputText;

			foreach (var text in this.displayTexts) {
				DestroyImmediate(text.GetComponent<I2.Loc.Localize>());
			}

			var displayBases = new GameObject[5];
			displayBases[0] = wrapper.Component.VentText;
			displayBases[1] = wrapper.Component.DetonateText;
			displayBases[2] = wrapper.Component.PreventsText;
			displayBases[3] = wrapper.Component.VentingCompleteText;
			displayBases[4] = wrapper.Component.InputText.gameObject;

			foreach (var displayBase in displayBases) {
				//displayBase.gameObject.SetActive(false);
				displayBase.transform.SetParent(this.transform, false);
			}
			DisableDisplay();

			var keypadEventConnector = new KeypadEventConnector();
			keypadEventConnector.ButtonPressed += (sender, e) => this.ButtonPressed?.Invoke(this, new VentingGasButtonEventArgs((VentingGasButton) e.ButtonIndex));

			this.buttons = new[] { wrapper.Component.NoButton, wrapper.Component.YesButton };
			keypadEventConnector.Attach(this.buttons);

			FixKeypadButtons(this.buttons);
#endif
		}
		protected override void AwakeTest() { }
		protected override void StartLive() {
#if (!DEBUG)
			var selectable = this.GetComponent<ModSelectable>();
			for (int i = 0; i < this.buttons.Length; ++i) {
				selectable.Children[i] = this.buttons[i].GetComponent<Selectable>();
				selectable.Children[i].Parent = selectable;
			}
#endif
		}
		protected override void StartTest() {
			foreach (var button in this.TestModelButtons)
				button.Pressed += (sender, e) => this.ButtonPressed?.Invoke(this, new VentingGasButtonEventArgs((VentingGasButton) e.ButtonIndex));
			DisableDisplay();
		}

		public void TwitchPress(VentingGasButton button) {
			if (this.TestMode) TwitchExtensions.Click(this.TestModelButtons[(int) button]);
#if (!DEBUG)
			else TwitchExtensions.Click(this.buttons[(int) button]);
#endif
		}
	}
}
