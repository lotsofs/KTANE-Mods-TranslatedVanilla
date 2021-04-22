using System;

namespace TranslatedVanillaModulesLib {
	public class VentingGasButtonEventArgs : EventArgs {
		public VentingGasButton Button { get; }
		public VentingGasButtonEventArgs(VentingGasButton button) => this.Button = button;
	}
}