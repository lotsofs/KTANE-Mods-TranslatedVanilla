using System;
using System.Collections.Generic;
using System.Linq;
using NotVanillaModulesLib.TestModel;
#if (!DEBUG)
using TMPro;
#endif
using UnityEngine;

namespace NotVanillaModulesLib {
	public class NotPasswordConnector : NotVanillaModuleConnector {
		public TestModelSpinner[] TestModelCharSpinners;
		public TestModelButton TestModelSubmitButton;
		public TextMesh TestModelButtonLabel;

		public event EventHandler SubmitPressed;

#if (!DEBUG)
		private IList<CharSpinner> spinners;
		private KeypadButton submitButton;
		private GameObject displayGlow;
		private PasswordLayout layout;
#endif

		bool _useCustomDisplay = false;
		bool _useCustomButton = false;

		protected override void AwakeLive() {
#if (!DEBUG)
			using var wrapper = this.InstantiateComponent<PasswordComponent>();
			this.layout = wrapper.Component.CurrentLayout = wrapper.Component.transform.Find("Layout_DEFAULT").GetComponent<PasswordLayout>();
			this.layout.transform.SetParent(this.transform, false);
			this.spinners = wrapper.Component.Spinners;
			this.submitButton = wrapper.Component.SubmitButton;
			this.submitButton.transform.SetParent(this.transform, false);
			//this.submitButton.transform.Rotate(new Vector3(0, 180, 0));
			this.displayGlow = wrapper.Component.DisplayGlow;
			this.displayGlow.transform.SetParent(this.transform, false);
			this.displayGlow.SetActive(false);

			var keypadEventConnector = new KeypadEventConnector();
			keypadEventConnector.ButtonPressed += this.KeypadEventConnector_ButtonPressed;
			keypadEventConnector.Attach(this.submitButton);

			FixKeypadButtons(this.GetComponentsInChildren<KeypadButton>());
#endif
		}

#if (!DEBUG)
		private void KeypadEventConnector_ButtonPressed(object sender, KeypadButtonEventArgs e) {
			this.SubmitPressed?.Invoke(this, EventArgs.Empty);
		}

#endif

		protected override void AwakeTest() {
		}
		protected override void StartLive() {
#if (!DEBUG)
			var selectable = this.GetComponent<ModSelectable>();
			for (int i = 0; i < 5; ++i) {
				selectable.Children[i] = this.spinners[i].UpButton.GetComponent<Selectable>();
				selectable.Children[i].Parent = selectable;
				selectable.Children[5 + i] = this.spinners[i].DownButton.GetComponent<Selectable>();
				selectable.Children[5 + i].Parent = selectable;
			}
			selectable.Children[12] = this.submitButton.GetComponent<Selectable>();
			selectable.Children[12].Parent = selectable;
#endif
		}
		protected override void StartTest() {
			this.TestModelSubmitButton.Pressed += (sender, e) => this.SubmitPressed?.Invoke(this, EventArgs.Empty);
		}

		public void Activate() {
			if (this.TestMode) foreach (var spinner in this.TestModelCharSpinners) spinner.Activate();
			else {
#if (!DEBUG)
				this.layout.Activate();
				this.displayGlow.SetActive(true);
#endif
				if (_useCustomDisplay) {
					foreach (TestModelSpinner spinner in TestModelCharSpinners) {
						spinner.Text.gameObject.SetActive(true);
					}
				}		
			}
		}

		public void UseCustomButtonLabel() {
			_useCustomButton = true;
#if (!DEBUG)
			submitButton.GetComponentInChildren<TextMeshPro>().gameObject.SetActive(false);
			TestModelButtonLabel.transform.SetParent(submitButton.transform, true);
#endif
		}

		public void SetButtonLabel(string label) {
			if (!this.TestMode && !_useCustomButton) {
#if (!DEBUG)
				submitButton.GetComponentInChildren<TextMeshPro>().text = label;
#endif
			}
			TestModelButtonLabel.text = label;
		}

		#region spinners

		public void UseCustomSpinners(int fontSize, Vector3 offset, Font font = null, Material fontMaterial = null) {
			_useCustomDisplay = true;
#if (!DEBUG)
			foreach (var spinner in spinners) {
				spinner.gameObject.SetActive(false);
			}
#endif
			for (int i = 0; i < TestModelCharSpinners.Length; i++) {
				TextMesh textMesh = TestModelCharSpinners[i].Text;
				if (font != null) {
					textMesh.font = font;
					textMesh.GetComponent<MeshRenderer>().material = fontMaterial;
				}
				textMesh.transform.localPosition = offset;
				textMesh.fontSize = fontSize;
				textMesh.gameObject.SetActive(false);
#if (!DEBUG)
				textMesh.transform.SetParent(this.transform, true);
				spinners[i].UpButton.OnPush += TestModelCharSpinners[i].Up;
				spinners[i].DownButton.OnPush += TestModelCharSpinners[i].Down;
#endif
			}
			// todo: button
			//for (int i = 0; i < buttons.Length; i++) {
			//	TestModelButtonLabels[i].transform.SetParent(buttons[i].transform, true);
			//	buttons[i].GetComponentInChildren<TextMeshPro>().gameObject.SetActive(false);
			//}
		}

		public string GetWord(bool rtl = false) {
			string word = "";
			for (int i = 0; i < 5; i++) {
				int j = rtl ? 5 - 1 - i : i;
				if (this.TestMode || _useCustomDisplay) {
					word += TestModelCharSpinners[j].SelectedChar;
				}
#if (!DEBUG)
				else {
					word += spinners[j].GetCurrentChar();
				}
#endif
			}
			return word;
		}

		public IEnumerable<char> GetSpinnerChoices(int index) {
			if (this.TestMode || _useCustomDisplay)	return TestModelCharSpinners[index].Choices;
#if (!DEBUG)
			else {
				var charSpinner = this.spinners[index];
				return charSpinner.Options;
			}
#endif
			return null;
		}

		public void SetSpinnerChoices(int index, IEnumerable<char> choices) {
			if (this.TestMode || _useCustomDisplay) this.TestModelCharSpinners[index].SetChoices(choices);
#if (!DEBUG)
			else {
				var charSpinner = this.spinners[index];
				charSpinner.Options = choices.ToList();
				charSpinner.UpdateDisplay();
			}
#endif
		}

		#endregion

		#region TP

		public void TwitchMoveUp(int index) {
			if (this.TestMode) TwitchExtensions.Click(this.TestModelCharSpinners[index].UpButton);
#if (!DEBUG)
			else TwitchExtensions.Click(this.spinners[index].UpButton);
#endif
		}

		public void TwitchMoveDown(int index) {
			if (this.TestMode) TwitchExtensions.Click(this.TestModelCharSpinners[index].DownButton);
#if (!DEBUG)
			else TwitchExtensions.Click(this.spinners[index].DownButton);
#endif
		}

		public void TwitchPressSubmit() {
			if (this.TestMode) TwitchExtensions.Press(this.TestModelSubmitButton);
#if (!DEBUG)
			else TwitchExtensions.Click(this.submitButton);
#endif
		}

		#endregion
	}
}
