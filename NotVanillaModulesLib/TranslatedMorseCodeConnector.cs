using System;
using System.Linq;
using TranslatedVanillaModulesLib.TestModel;
using UnityEngine;
#if (!DEBUG)
using TMPro;
#endif

namespace TranslatedVanillaModulesLib {
	public class TranslatedMorseCodeConnector : TranslatedVanillaModuleConnector {
		public GameObject TestModelTunerSlider;
		public TextMesh TestModelTunerDisplay;
		public GameObject LightOff;
		public GameObject LightOn;
		public TestModelButton TestModelDownButton;
		public TestModelButton TestModelUpButton;
		public TestModelButton TestModelSubmitButton;

		public event EventHandler DownPressed;
		public event EventHandler UpPressed;
		public event EventHandler SubmitPressed;
		public event EventHandler SubmitReleased;

		private float _freqMarkerCurrent;
#pragma warning disable IDE0044 // Add readonly modifier
		private float _freqMarkerSpeed = 0.25f;
		private int _freqMarkerStartFreq = 500;
		private int _freqMarkerEndFreq = 600;
#pragma warning restore IDE0044 // Add readonly modifier
		private bool _useCustomButton;

#if (!DEBUG)
		private TextMeshPro _displayText;
		private KeypadButton[] _buttons;

		private Transform freqMarker;
		private Vector3 freqMarkerStart;
		private Vector3 freqMarkerEnd;
#endif

		private float targetSliderPosition;
		private string mhz = "MHz";

		public void Update() {
			var d = this.targetSliderPosition - this._freqMarkerCurrent;
			if (d != 0) {
				var dx = this._freqMarkerSpeed * Time.deltaTime;
				if (d > 0) {
					if (d < dx) this._freqMarkerCurrent = this.targetSliderPosition;
					else this._freqMarkerCurrent += dx;
				} else {
					if (d > -dx) this._freqMarkerCurrent = this.targetSliderPosition;
					else this._freqMarkerCurrent -= dx;
				}
				if (this.TestMode) {
					var position = this.TestModelTunerSlider.transform.localPosition;
					position.x = (this._freqMarkerCurrent - 0.5f) * 0.11f;
					this.TestModelTunerSlider.transform.localPosition = position;
				}
#if (!DEBUG)
				else this.freqMarker.localPosition = Vector3.Lerp(this.freqMarkerStart, this.freqMarkerEnd, this._freqMarkerCurrent);
#endif
			}
		}

		protected override void AwakeLive() {
#if (!DEBUG)
			using var wrapper = this.InstantiateComponent<MorseCodeComponent>();
			this._displayText = wrapper.Component.DisplayText;
			this._displayText.transform.SetParent(this.transform, false);

			var layout = wrapper.Component.transform.Find("Component_Morse");
			layout.SetParent(this.transform, false);
			//wrapper.Component.TransmitButton.Text.text = "XT";

			this.LightOff = wrapper.Component.LEDUnlit;
			this.LightOn = wrapper.Component.LEDLit;

			this._buttons = new[] { wrapper.Component.DownButton, wrapper.Component.UpButton, wrapper.Component.TransmitButton };

			var keypadEventConnector = new KeypadEventConnector();
			keypadEventConnector.ButtonPressed += this.KeypadEventConnector_ButtonPressed;
			keypadEventConnector.ButtonReleased += this.KeypadEventConnector_ButtonReleased;
			keypadEventConnector.Attach(this._buttons);

			var marker = layout.Find("Freq_Marker").GetComponent<FreqMarker>();
			this.freqMarker = marker.transform;
			this.freqMarkerStart = marker.StartPoint.localPosition;
			this.freqMarkerEnd = marker.EndPoint.localPosition;
			this._freqMarkerStartFreq = marker.StartFreq;
			this._freqMarkerEndFreq = marker.EndFreq;
			this._freqMarkerSpeed = marker.Speed;
			Destroy(marker);  // FreqMarker doesn't do what we need it to and may throw exceptions in Update.
#endif
		}

		public void UseCustomButtonLabel() {
			_useCustomButton = true;
#if (!DEBUG)
			_buttons[2].GetComponentInChildren<TextMeshPro>().gameObject.SetActive(false);
			TestModelSubmitButton.TextMesh.transform.SetParent(_buttons[2].transform, true);
#endif
		}

#if (!DEBUG)
		private void KeypadEventConnector_ButtonPressed(object sender, KeypadButtonEventArgs e) {
			switch (e.ButtonIndex) {
				case 0: this.DownPressed?.Invoke(sender, EventArgs.Empty); break;
				case 1: this.UpPressed?.Invoke(sender, EventArgs.Empty); break;
				case 2: this.SubmitPressed?.Invoke(sender, EventArgs.Empty); break;
			}
		}
		private void KeypadEventConnector_ButtonReleased(object sender, KeypadButtonEventArgs e) {
			if (e.ButtonIndex == 2) this.SubmitReleased?.Invoke(sender, EventArgs.Empty);
		}
#endif

		protected override void AwakeTest() { }
		protected override void StartLive() {
#if (!DEBUG)
			var selectable = this.GetComponent<ModSelectable>();
			for (int i = 0; i < selectable.Children.Length; ++i) {
				selectable.Children[i] = this._buttons[i].GetComponent<Selectable>();
				selectable.Children[i].Parent = selectable;
			}
			this._displayText.GetComponent<Renderer>().enabled = false;
#endif
		}
		protected override void StartTest() {
			this.TestModelDownButton.Pressed += (sender, e) => this.DownPressed?.Invoke(this, EventArgs.Empty);
			this.TestModelUpButton.Pressed += (sender, e) => this.UpPressed?.Invoke(this, EventArgs.Empty);
			this.TestModelSubmitButton.Pressed += (sender, e) => this.SubmitPressed?.Invoke(this, EventArgs.Empty);
			this.TestModelTunerDisplay.GetComponent<Renderer>().enabled = false;
		}

		public void Activate() {
			if (this.TestMode) this.TestModelTunerDisplay.GetComponent<Renderer>().enabled = true;
#if (!DEBUG)
			else this._displayText.GetComponent<Renderer>().enabled = true;
#endif
		}

		public void SetSlider(float position) => this.targetSliderPosition = position;
		public void SetSlider(int frequencyFractionalPartKHz) =>
			this.SetSlider((float) (frequencyFractionalPartKHz - this._freqMarkerStartFreq) / (this._freqMarkerEndFreq - this._freqMarkerStartFreq));
		public void SetSliderImmediate(float position) {
			this.SetSliderImmediate(position);
			this._freqMarkerCurrent = this.targetSliderPosition + 0.001f;
		}
		public void SetSliderImmediate(int frequencyFractionalPartKHz) {
			this.SetSliderImmediate(frequencyFractionalPartKHz);
			this._freqMarkerCurrent = this.targetSliderPosition + 0.001f;
		}

		public void SetLabels(string tx, string freq = "MHz") {
			mhz = freq;
			if (this.TestMode || _useCustomButton) this.TestModelSubmitButton.TextMesh.text = tx;
#if (!DEBUG)
			else this._buttons[2].Text.text = tx;
#endif
		}

		public void SetDisplay(string frequencyFractionalPart) {
			if (this.TestMode) this.TestModelTunerDisplay.text = $"3.{frequencyFractionalPart}\u00A0{mhz}";
#if (!DEBUG)
			else this._displayText.text = $"3.{frequencyFractionalPart} {mhz}";
#endif
		}

		public void SetLight(bool on) {
			this.LightOff.SetActive(!on);
			this.LightOn.SetActive(on);
		}

		public void TwitchMoveDown() {
			if (this.TestMode) TwitchExtensions.Click(this.TestModelDownButton);
#if (!DEBUG)
			else TwitchExtensions.Click(this._buttons[0]);
#endif
		}
		public void TwitchMoveUp() {
			if (this.TestMode) TwitchExtensions.Click(this.TestModelUpButton);
#if (!DEBUG)
			else TwitchExtensions.Click(this._buttons[1]);
#endif
		}
		public void TwitchSubmit() {
			if (this.TestMode) TwitchExtensions.Click(this.TestModelSubmitButton);
#if (!DEBUG)
			else TwitchExtensions.Click(this._buttons[2]);
#endif
		}
		public void TwitchPressSubmit() {
			if (this.TestMode) TwitchExtensions.Press(this.TestModelSubmitButton);
#if (!DEBUG)
			else TwitchExtensions.Press(this._buttons[2]);
#endif
		}
		public void TwitchReleaseSubmit() {
			if (this.TestMode) TwitchExtensions.Release(this.TestModelSubmitButton);
#if (!DEBUG)
			else TwitchExtensions.Release(this._buttons[2]);
#endif
		}
	}
}
