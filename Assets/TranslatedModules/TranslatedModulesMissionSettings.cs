public abstract class TranslatedModulesMissionSettings {
	public abstract string[] FixedLanguages { get; set; }
	public abstract string[] RandomLanguages { get; set; }
	public abstract bool ShuffleFixedLanguages { get; set; }
	public abstract bool AvoidDuplicates { get; set; }
}
