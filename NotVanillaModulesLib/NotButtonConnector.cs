using System;
using System.Collections;
using System.Linq;

#if (!DEBUG)
using TMPro;
#endif

using UnityEngine;

namespace NotVanillaModulesLib {
	public class NotButtonConnector : NotVanillaModuleConnector {
		public Transform TestModelCover;
		public MeshRenderer TestModelCap;
		public TextMesh TestModelText;
		public MeshRenderer TestLightRenderer;
		public Light[] TestLights;
		public TextMesh TestModelColourblindLightText;

		public TextMesh CustomTextMesh;

		public Material[] Materials;
		public Material[] ColourblindMaterials;
		public Material[] LightMaterials;

		public KMAudio KMAudio { get; private set; }
		
		private KMSelectable testModelButton;
#if (!DEBUG)
		private PressableButton button;
		private Animator lidAnimator;
		private bool buttonBeingPushed;
		public TextMeshPro ColourblindLightText;
#endif
		private GameObject UsedLabelMesh;

		private Coroutine animationCoroutine;

		public event EventHandler Held;
		public event EventHandler Released;

		private static readonly Color DarkTextColor = new Color(0, 0, 0, 0.8f);
		private static readonly Color LightTextColor = new Color(1, 1, 1, 0.9f);

		/// <summary>Returns a value indicating whether the vanilla Button module will open the cover on selection or focus. The value is not valid during Awake.</summary>
		public bool ShouldOpenCoverOnSelection { get; private set; }

		private ButtonColour colour;
		private ButtonLightColour lightColour;

		protected override void AwakeLive() {
#if (!DEBUG)
			var buttonComponentPrefab = GetComponentPrefab<ButtonComponent>();
			this.ShouldOpenCoverOnSelection = buttonComponentPrefab.LidBehaviour == 0 || KTInputManager.Instance.IsMotionControlMode();
			var buttonPrefab = buttonComponentPrefab.transform.Find("Button").GetComponent<PressableButton>();
			this.button = Instantiate(buttonPrefab, this.transform);
			this.lidAnimator = this.button.transform.Find("Opening_LID").GetComponent<Animator>();

			var buttonEventConnector = new ButtonEventConnector();
			buttonEventConnector.Held += this.ButtonEventConnector_Held;
			buttonEventConnector.Released += this.ButtonEventConnector_Released;
			buttonEventConnector.Attach(this.button);

			var text = Instantiate(GetComponentPrefab<PasswordComponent>().transform.Find("Layout_DEFAULT").GetComponent<PasswordLayout>().Spinners[0].Display, this.button.transform.Find("parts/LED_Off"), false);
			this.ColourblindLightText = text;
			text.enableAutoSizing = false;
			text.transform.localRotation = Quaternion.Euler(90, 180, 0);
			text.transform.localPosition = new Vector3(0.015f, 0.001f, 0.05f);
			text.transform.localScale = new Vector3(0.005f, 0.005f, 1);
			text.alignment = TextAlignmentOptions.Center;
			text.color = new Color(0, 0, 0, 0.8f);
			text.lineSpacing = -12;
			text.transform.SetParent(text.transform.parent.parent, true);
#endif
		}

		protected override void AwakeTest() { }

		public override void Start() {
			base.Start();
			this.KMAudio = this.GetComponent<KMAudio>();

			if (this.TestMode) {
				var lightScale = this.transform.lossyScale.x;
				foreach (var light in this.TestLights) light.range *= lightScale;
			}
		}

		protected override void StartLive() {
#if (!DEBUG)
			var selectable = this.GetComponent<ModSelectable>();
			selectable.Children[0] = this.button.GetComponent<Selectable>();
			selectable.Children[0].Parent = selectable;
			var testModelButton = this.GetComponent<KMSelectable>().Children[0];
			selectable.Children[0].OnHighlight = () => testModelButton.OnHighlight?.Invoke();
			selectable.Children[0].OnHighlightEnded = () => testModelButton.OnHighlightEnded?.Invoke();
#endif
		}

		protected override void StartTest() {
			this.testModelButton = this.GetComponent<KMSelectable>().Children[0];
			this.testModelButton.OnInteract = this.TestModelButton_Interact;
			this.testModelButton.OnInteractEnded = this.TestModelButton_InteractEnded;
			this.TestModelColourblindLightText.gameObject.SetActive(false);
		}

		public override bool ColourblindMode {
			get => base.ColourblindMode;
			set {
				base.ColourblindMode = value;
				this.SetColour(this.colour);
				this.SetLightColour(this.lightColour);
			}
		}

		private void ButtonEventConnector_Held(object sender, EventArgs e) {
#if (!DEBUG)
			this.buttonBeingPushed = true;
#endif
			this.Held?.Invoke(this, e);
		}

		private void ButtonEventConnector_Released(object sender, EventArgs e) => this.Released?.Invoke(this, e);

		private bool TestModelButton_Interact() {
			this.Held?.Invoke(this, EventArgs.Empty);
			this.KMAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonPress, this.testModelButton.transform);
			return false;
		}
		private void TestModelButton_InteractEnded() {
			this.Released?.Invoke(this, EventArgs.Empty);
			this.KMAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonRelease, this.testModelButton.transform);
		}

		public void OpenCover() {
			if (this.TestMode) {
				this.StopAllCoroutines();
				this.StartCoroutine(this.OpenCoverTestCoroutine());
			} else {
#if (!DEBUG)
				this.lidAnimator.SetBool("IsLidOpen", true);
#endif
			}
		}
		private IEnumerator OpenCoverTestCoroutine() {
			while (this.TestModelCover.localEulerAngles.x < 80) {
				this.TestModelCover.localEulerAngles += new Vector3(4, 0, 0);
				yield return null;
			}
		}

		public void CloseCover() {
			if (this.TestMode) {
				this.StopAllCoroutines();
				this.StartCoroutine(this.CloseCoverTestCoroutine());
			} else {
#if (!DEBUG)
				if (this.buttonBeingPushed) {
					// Interacting with the button causes the module's OnDefocus event to be called.
					// We don't want to close the cover in this particular case.
					this.buttonBeingPushed = false;
				} else {
					this.lidAnimator.SetBool("IsLidOpen", false);
				}
#endif
			}
		}
		private IEnumerator CloseCoverTestCoroutine() {
			while (this.TestModelCover.localEulerAngles.x > 1) {
				this.TestModelCover.localEulerAngles -= new Vector3(4, 0, 0);
				yield return null;
			}
			this.TestModelCover.localEulerAngles = Vector3.zero;
		}

		public void SetColour(ButtonColour colour) {
			this.colour = colour;
			var material = (this.ColourblindMode ? this.ColourblindMaterials : this.Materials)[(int) colour];
			var lightText = colour switch {
				ButtonColour.Yellow => false,
				ButtonColour.White => false,
				_ => true
			};

			if (this.TestMode) {
				this.TestModelCap.material = material;
				this.TestModelText.color = lightText ? LightTextColor : DarkTextColor;
				// Colourblind materials use a high texture scale so that they will appear correctly on the vanilla button model.
				// This needs to be changed in the test harness.
				material.mainTextureScale = Vector2.one;
				material.mainTextureOffset = Vector2.zero;
			}
			else {
#if (!DEBUG)
				var buttonComponent = this.button.GetComponent<PressableButton>();
				if (!lightText) buttonComponent.text.GetComponent<Renderer>().enabled = true;
				if (!this.ColourblindMode) {
					switch (colour) {
						case ButtonColour.Red: buttonComponent.SetColor(BombGame.ButtonColor.red); return;
						case ButtonColour.Yellow: buttonComponent.SetColor(BombGame.ButtonColor.yellow); return;
						case ButtonColour.Blue: buttonComponent.SetColor(BombGame.ButtonColor.blue); return;
						case ButtonColour.White: buttonComponent.SetColor(BombGame.ButtonColor.white); return;
					}
				}
				else {
					// centers the colorblind textures
					material.mainTextureScale = Vector2.one * 25;
					material.mainTextureOffset = new Vector2(0.41f, 0.47f);
				}
				buttonComponent.SetColor(BombGame.ButtonColor.white);
				var cap = this.button.transform.Find("ButtonTop").Find("Button_Top_White").GetComponent<MeshRenderer>();
				material = Instantiate(material);
				InstanceDestroyer.AddObjectToDestroy(this.gameObject, material);
				if (!this.ColourblindMode) material.mainTexture = cap.material.mainTexture;
				cap.material = material;
				buttonComponent.text.color = lightText ? LightTextColor : DarkTextColor;
					//With light text, hide the text while the lights are off.
					//If we don't do this, the text will appear lit even without the lights.
					//The vanilla PressableButton does the same thing...
				buttonComponent.text.GetComponent<HideOnLightsChange>().enabled = lightText;
#endif
			}
		}

		public void SetLabel(string label, Font font, Material fontMaterial, int fontSize) {
			label = label.Replace(@"\n", Environment.NewLine);
			if (this.TestMode) {
				this.TestModelText.text = label;
				if (font != null && fontMaterial != null) {
					TestModelText.font = font;
					TestModelText.GetComponent<MeshRenderer>().material = fontMaterial;
				}
				if (fontSize > 0) {
					TestModelText.fontSize = fontSize / 5;
				}
			}
#if (!DEBUG)
			else {
				Vector3 position = this.button.GetComponent<PressableButton>().text.transform.localPosition;
				CustomTextMesh.transform.SetParent(this.button.GetComponent<PressableButton>().text.transform, true);
				CustomTextMesh.gameObject.SetActive(true);
				CustomTextMesh.text = label;
				CustomTextMesh.transform.localPosition = position;
				CustomTextMesh.color = this.button.GetComponent<PressableButton>().text.color;

				if (font != null && fontMaterial != null) {
					CustomTextMesh.font = font;
					CustomTextMesh.GetComponent<MeshRenderer>().material = fontMaterial;
				}
				if (fontSize > 0) {
					CustomTextMesh.fontSize = fontSize;
				}
				UsedLabelMesh = CustomTextMesh.gameObject;
				this.button.GetComponent<PressableButton>().text.enabled = false;
			}
#endif
		}

		public void SetLabel(string label) {
				label = label.Replace(@"\n", Environment.NewLine);
				if (this.TestMode) {
					this.TestModelText.text = label;
				}
#if (!DEBUG)
				else {
					this.button.GetComponent<PressableButton>().text.text = label;
				}
#endif
			}

		public void SetLightColour(ButtonLightColour colour) {
			this.lightColour = colour;
			if (colour == ButtonLightColour.Off) {
				if (this.TestMode) {
					this.TestModelColourblindLightText.gameObject.SetActive(false);
					this.TestLightRenderer.material = this.LightMaterials[(int) colour];
					foreach (var light in this.TestLights)
						light.gameObject.SetActive(false);

					if (this.animationCoroutine != null) {
						this.StopCoroutine(this.animationCoroutine);
						this.animationCoroutine = null;
					}
				}
#if (!DEBUG)
				else {
					this.ColourblindLightText.gameObject.SetActive(false);
					this.button.transform.Find("parts/LED_Blue").gameObject.SetActive(false);
					this.button.transform.Find("parts/LED_Red").gameObject.SetActive(false);
					this.button.transform.Find("parts/LED_Yellow").gameObject.SetActive(false);
					this.button.transform.Find("parts/LED_White").gameObject.SetActive(false);
					this.button.transform.Find("parts/LED_Off").gameObject.SetActive(true);
				}
#endif
			} 
			else {
				var text = colour switch {
					ButtonLightColour.Red => "R", 
					ButtonLightColour.Yellow => "Y",
					ButtonLightColour.Blue => "B",
					ButtonLightColour.White => "W", 
					_ => ""
				};
				if (this.TestMode) {
					this.TestLightRenderer.material = this.LightMaterials[(int)colour];
					foreach (var light in this.TestLights) {
						light.gameObject.SetActive(true);
						switch (colour) {
							case ButtonLightColour.Red: light.color = Color.red; break;
							case ButtonLightColour.Yellow: light.color = Color.yellow; break;
							case ButtonLightColour.Blue: light.color = Color.blue; break;
							case ButtonLightColour.White: light.color = Color.white; break;
						}
					}
					this.animationCoroutine = this.StartCoroutine(this.LightAnimationCoroutine());
					this.TestModelColourblindLightText.text = text;
					this.TestModelColourblindLightText.gameObject.SetActive(this.ColourblindMode);
				}
#if (!DEBUG)
				else {
					this.ColourblindLightText.text = text;
					this.ColourblindLightText.gameObject.SetActive(this.ColourblindMode);
					var ledName = colour switch
					{
						ButtonLightColour.Red => "parts/LED_Red",
						ButtonLightColour.Yellow => "parts/LED_Yellow",
						ButtonLightColour.Blue => "parts/LED_Blue",
						ButtonLightColour.White => "parts/LED_White",
						_ => "parts/LED_Off"
					};
					this.button.transform.Find("parts/LED_Off").gameObject.SetActive(false);
					this.button.transform.Find(ledName).gameObject.SetActive(true);
				}
#endif
			}
		}
		public void SetLightBrightness(float brightness) {
			this.TestLightRenderer.material.SetFloat("_Blend", brightness);
			foreach (var light in this.TestLights) light.intensity = brightness * 2;
		}

		public void ToggleLabel(bool on) {
			if (!this.TestMode) {
#if (!DEBUG)
				if (UsedLabelMesh != null) {
					UsedLabelMesh.SetActive(on);
				}
				else {
					var buttonComponent = this.button.GetComponent<PressableButton>();
					buttonComponent.text.enabled = on;
				}
#endif
			}
		}

		/// <summary>
		/// Animate the strip brightness
		/// </summary>
		/// <returns></returns>
		private IEnumerator LightAnimationCoroutine() {
			float time = 0;
			while (true) {
				var r = time % 1.4f;
				this.SetLightBrightness(0.79f + 0.3f * Math.Abs(r - 0.7f));
				yield return null;
				time += Time.deltaTime;
			}
		}

		public void TwitchPress() {
			if (this.TestMode) TwitchExtensions.Press(this.testModelButton);
#if (!DEBUG)
			else TwitchExtensions.Press(this.button);
#endif
		}

		public void TwitchRelease() {
			if (this.TestMode) TwitchExtensions.Release(this.testModelButton);
#if (!DEBUG)
			else TwitchExtensions.Release(this.button);
#endif
		}
	}
}
