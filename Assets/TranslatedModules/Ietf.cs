using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Ietf {
	public class Language {
		public string Anglonym;
		public string Endonym;
		public string SuppressScript;
		public bool RightToLeft;
	}

	public enum Scripts {
		Tifinagh = 120,             // TFNG
		Hebrew = 125,               // HEBR
		Arabic = 160,               // ARAB
		Greek = 200,                // GREK
		Latin = 215,                // LATN
		Cyrillic = 221,             // CYRL
		Armenian = 230,             // ARMN
		Georgian = 240,             // GEOR
		Khutsuri = 241,             // GEOK
		Hangul = 286,               // HANG
		Korean = 287,               // KORE
		Devanagari = 315,           // DEVA
		Bengali = 325,              // BENG
		Tibetan = 330,              // TIBT
		Telugu = 340,               // TELU
		Malayalam = 347,            // MLYM
		Sinhala = 348,              // SINH
		Burmese = 350,              // MYMR
		Thai = 352,                 // THAI
		Lao = 356,                  // LAOO
		Javanese = 361,             // JAVA
		Hiragana = 410,             // HIRA
		Katakana = 411,             // KANA
		Kana = 412,                 // HRKT
		Japanese = 413,             // JPAN
		Geez = 430,                 // ETHI
		CanadianAboriginal = 440,   // CANS
		Cherokee = 445,             // CHER
		Yi = 460,                   // YIII
		HanSimplified = 501,        // HANS
		HanTraditional = 502,       // HANT
		Default = 900,              // QAAA
		Other = 901,                // QAAB
	};

	public enum ScriptCodes {
		Tfng = 120,
		Hebr = 125,
		Arab = 160,
		Grek = 200,
		Latn = 215,
		Cyrl = 221,
		Armn = 230,
		Geor = 240,
		Geok = 241,
		Hang = 286,
		Kore = 287,
		Deva = 315,
		Beng = 325,
		Tibt = 330,
		Telu = 340,
		Mlym = 347,
		Sinh = 348,
		Mymr = 350,
		Thai = 352,
		Laoo = 356,
		Java = 361,
		Hira = 410,
		Kana = 411,
		Hrkt = 412,
		Jpan = 413,
		Ethi = 430,
		Cans = 440,
		Cher = 445,
		Yiii = 460,
		Hans = 501,
		Hant = 502,
		Qaaa = 900,
		Qaab = 901,
	};

	// most of these regions will never be needed.
	public enum Regions {
		Default = 000,
		//World = 001,
		//Africa = 002,
		//NorthAmerica = 003,
		//SouthAmerica = 005,
		//Oceania = 009,
		//WesternAfrica = 011,
		//CentralAmerica = 013,
		//EasternAfrica = 014,
		//NorthernAfrica = 015,
		//MiddleAfrica = 017,
		//SouthernAfrica = 018,
		//Americas = 019,
		//NorthernAmerica = 021,
		//Caribbean = 029,
		//EasternAsia = 030,
		//SouthernAsia = 034,
		//SouthEasternAsia = 035,
		//SouthernEurope = 039,
		//AustraliaAndNewZealand = 053,
		//Melanesia = 054,
		//Micronesia = 057,
		//Polynesia = 061,
		//Asia = 142,
		//CentralAsia = 143,
		//WesternAsia = 145,
		//Europe = 150,
		//EasternEurope = 151,
		//NorthernEurope = 154,
		//WesternEurope = 155,
		//SubSaharanAfrica = 202,
		LatinAmericaAndTheCaribbean = 419,
		Other = 900,
	}

	public static Dictionary<string, Language> Languages = new Dictionary<string, Language> {
		{ "-", new Language { Anglonym = "Default",         Endonym = "English",            SuppressScript = "Latn" } },
		{ "af", new Language { Anglonym = "Afrikaans",		Endonym = "Afrikaans",			SuppressScript = "Latn" } },
		{ "ar", new Language { Anglonym = "Arabic",			Endonym = "العربية",			SuppressScript = "Arab", RightToLeft = true } },
		{ "az", new Language { Anglonym = "Azerbaijani",	Endonym = "Azərbaycanca",		SuppressScript = null } },
		{ "be", new Language { Anglonym = "Belarusian",		Endonym = "Беларуская",			SuppressScript = "Cyrl" } },
		{ "bg", new Language { Anglonym = "Bulgarian",		Endonym = "Български",			SuppressScript = "Cyrl" } },
		{ "bn", new Language { Anglonym = "Bengali",		Endonym = "বাংলা",				SuppressScript = "Beng" } },
		{ "br", new Language { Anglonym = "Breton",			Endonym = "Brezhoneg",			SuppressScript = null } },
		{ "ca", new Language { Anglonym = "Catalan",		Endonym = "Català",				SuppressScript = "Latn" } },
		{ "cs", new Language { Anglonym = "Czech",			Endonym = "Čeština",			SuppressScript = "Latn" } },
		{ "de", new Language { Anglonym = "German",			Endonym = "Deutsch",			SuppressScript = "Latn" } },
		{ "el", new Language { Anglonym = "Greek",			Endonym = "Ελληνικά",			SuppressScript = "Grek" } },
		{ "en", new Language { Anglonym = "English", 		Endonym = "English",			SuppressScript = "Latn" } },
		{ "eo", new Language { Anglonym = "Esperanto", 		Endonym = "Esperanto",			SuppressScript = "Latn" } },
		{ "es", new Language { Anglonym = "Spanish", 		Endonym = "Español",			SuppressScript = "Latn" } },
		{ "eu", new Language { Anglonym = "Basque", 		Endonym = "Euskara",			SuppressScript = "Latn" } },
		{ "fi", new Language { Anglonym = "Finnish", 		Endonym = "Suomi",				SuppressScript = "Latn" } },
		{ "fr", new Language { Anglonym = "French", 		Endonym = "Français",			SuppressScript = "Latn" } },
		{ "fy", new Language { Anglonym = "Frisian", 		Endonym = "Frysk",				SuppressScript = "Latn" } },
		{ "ga", new Language { Anglonym = "Irish", 			Endonym = "Gaeilge",			SuppressScript = "Latn" } },
		{ "gl", new Language { Anglonym = "Galician", 		Endonym = "Galego",				SuppressScript = "Latn" } },
		{ "he", new Language { Anglonym = "Hebrew", 		Endonym = "עברית",				SuppressScript = "Hebr", RightToLeft = true } },
		{ "hi", new Language { Anglonym = "Hindi", 			Endonym = "हिन्दी",				SuppressScript = "Deva" } },
		{ "hr", new Language { Anglonym = "Croatian", 		Endonym = "Hrvatski",			SuppressScript = "Latn" } },
		{ "hu", new Language { Anglonym = "Hungarian", 		Endonym = "Magyar",				SuppressScript = "Latn" } },
		{ "hy", new Language { Anglonym = "Armenian",		Endonym = "Հայերեն",			SuppressScript = "Armn" } },
		{ "it", new Language { Anglonym = "Italian", 		Endonym = "Italiano",			SuppressScript = "Latn" } },
		{ "ja", new Language { Anglonym = "Japanese", 		Endonym = "日本語",				SuppressScript = "Jpan" } },
		{ "jv", new Language { Anglonym = "Javanese", 		Endonym = "Jawa",				SuppressScript = null } },
		{ "ko", new Language { Anglonym = "Korean", 		Endonym = "한국어",				SuppressScript = "Kore" } },
		{ "la", new Language { Anglonym = "Latin", 			Endonym = "Latina",				SuppressScript = "Latn" } },
		{ "lb", new Language { Anglonym = "Luxembourgish",	Endonym = "Lëtzebuergesch",		SuppressScript = "Latn" } },
		{ "mk", new Language { Anglonym = "Macedonian", 	Endonym = "Македонски",			SuppressScript = "Cyrl" } },
		{ "ml", new Language { Anglonym = "Malayalam", 		Endonym = "മലയാളം",			SuppressScript = "Mlym" } },
		{ "nl", new Language { Anglonym = "Dutch", 			Endonym = "Nederlands",			SuppressScript = "Latn" } },
		{ "oc", new Language { Anglonym = "Occitan", 		Endonym = "Occitan",			SuppressScript = null } },
		{ "pl", new Language { Anglonym = "Polish", 		Endonym = "Polski",				SuppressScript = "Latn" } },
		{ "pt", new Language { Anglonym = "Portuguese", 	Endonym = "Português",			SuppressScript = "Latn" } },
		{ "ro", new Language { Anglonym = "Romanian", 		Endonym = "Română",				SuppressScript = "Latn" } },
		{ "ru", new Language { Anglonym = "Russian", 		Endonym = "Русский",			SuppressScript = "Cyrl" } },
		{ "so", new Language { Anglonym = "Somali", 		Endonym = "Soomaaliga",			SuppressScript = "Latn" } },
		{ "sv", new Language { Anglonym = "Swedish", 		Endonym = "Svenska",			SuppressScript = "Latn" } },
		{ "tl", new Language { Anglonym = "Tagalog", 		Endonym = "Tagalog",			SuppressScript = "Latn" } },
		{ "tr", new Language { Anglonym = "Turkish", 		Endonym = "Türkçe",				SuppressScript = "Latn" } },
		{ "tt", new Language { Anglonym = "Tatar", 			Endonym = "Татарча",			SuppressScript = null } },
		{ "uk", new Language { Anglonym = "Ukrainian", 		Endonym = "Українська",			SuppressScript = "Cyrl" } },
		{ "vi", new Language { Anglonym = "Vietnamese", 	Endonym = "Tiếng Việt",			SuppressScript = "Latn" } },
		
		
		{ "aa", new Language { Anglonym = "Afar", 			Endonym = "", SuppressScript = "" } },
		{ "ab", new Language { Anglonym = "Abkhazian", 		Endonym = "", SuppressScript = "" } },
		{ "ae", new Language { Anglonym = "Avestan", 		Endonym = "", SuppressScript = "" } },
		{ "ak", new Language { Anglonym = "Akan", 			Endonym = "", SuppressScript = "" } },
		{ "am", new Language { Anglonym = "Amharic", 		Endonym = "", SuppressScript = "" } },
		{ "an", new Language { Anglonym = "Aragonese", 		Endonym = "", SuppressScript = "" } },
		{ "as", new Language { Anglonym = "Assamese", 		Endonym = "", SuppressScript = "" } },
		{ "av", new Language { Anglonym = "Avaric", 		Endonym = "", SuppressScript = "" } },
		{ "ay", new Language { Anglonym = "Aymara", 		Endonym = "", SuppressScript = "" } },

		{ "ba", new Language { Anglonym = "Bashkir", 		Endonym = "", SuppressScript = "" } },
		{ "bh", new Language { Anglonym = "Bihari", 		Endonym = "", SuppressScript = "" } },
		{ "bi", new Language { Anglonym = "Bislama", 		Endonym = "", SuppressScript = "" } },
		{ "bm", new Language { Anglonym = "Bambara", 		Endonym = "", SuppressScript = "" } },
		{ "bo", new Language { Anglonym = "Tibetan", 		Endonym = "", SuppressScript = "" } },
		{ "bs", new Language { Anglonym = "Bosnian", 		Endonym = "", SuppressScript = "" } },

		{ "ce", new Language { Anglonym = "Chechen", 		Endonym = "", SuppressScript = "" } },
		{ "ch", new Language { Anglonym = "Chamorro", 		Endonym = "", SuppressScript = "" } },
		{ "co", new Language { Anglonym = "Corsican", 		Endonym = "", SuppressScript = "" } },
		{ "cr", new Language { Anglonym = "Cree", 			Endonym = "", SuppressScript = "" } },
		{ "cu", new Language { Anglonym = "Church Slavonic",Endonym = "", SuppressScript = "" } },
		{ "cv", new Language { Anglonym = "Chuvash", 		Endonym = "", SuppressScript = "" } },
		{ "cy", new Language { Anglonym = "Welsh", 			Endonym = "", SuppressScript = "" } },

		{ "da", new Language { Anglonym = "Danish", 		Endonym = "", SuppressScript = "" } },
		{ "dv", new Language { Anglonym = "Maldivian", 		Endonym = "", SuppressScript = "" } },
		{ "dz", new Language { Anglonym = "Dzongkha", 		Endonym = "", SuppressScript = "" } },

		{ "ee", new Language { Anglonym = "Ewe", 			Endonym = "", SuppressScript = "" } },
		{ "et", new Language { Anglonym = "Estonian", 		Endonym = "", SuppressScript = "" } },

		{ "fa", new Language { Anglonym = "Persian", 		Endonym = "", SuppressScript = "" } },
		{ "ff", new Language { Anglonym = "Fulah", 			Endonym = "", SuppressScript = "" } },
		{ "fj", new Language { Anglonym = "Fijian", 		Endonym = "", SuppressScript = "" } },
		{ "fo", new Language { Anglonym = "Faroese", 		Endonym = "", SuppressScript = "" } },

		{ "gd", new Language { Anglonym = "Gaelic", 		Endonym = "", SuppressScript = "" } },
		{ "gn", new Language { Anglonym = "Guarani", 		Endonym = "", SuppressScript = "" } },
		{ "gu", new Language { Anglonym = "Gujarati", 		Endonym = "", SuppressScript = "" } },
		{ "gv", new Language { Anglonym = "Manx", 			Endonym = "", SuppressScript = "" } },

		{ "ha", new Language { Anglonym = "Hausa", 			Endonym = "", SuppressScript = "" } },
		{ "ho", new Language { Anglonym = "Hiri Motu", 		Endonym = "", SuppressScript = "" } },
		{ "ht", new Language { Anglonym = "Haitian Creole", Endonym = "", SuppressScript = "" } },
		{ "hz", new Language { Anglonym = "Herero", 		Endonym = "", SuppressScript = "" } },

		{ "ia", new Language { Anglonym = "Interlingua",	Endonym = "", SuppressScript = "" } },
		{ "id", new Language { Anglonym = "Indonesian",		Endonym = "", SuppressScript = "" } },
		{ "ie", new Language { Anglonym = "Interlingue", 	Endonym = "", SuppressScript = "" } },
		{ "ig", new Language { Anglonym = "Igbo", 			Endonym = "", SuppressScript = "" } },
		{ "ii", new Language { Anglonym = "Nuosu", 			Endonym = "", SuppressScript = "" } },
		{ "ik", new Language { Anglonym = "Inupiaq", 		Endonym = "", SuppressScript = "" } },
		{ "io", new Language { Anglonym = "Ido", 			Endonym = "", SuppressScript = "" } },
		{ "is", new Language { Anglonym = "Icelandic", 		Endonym = "", SuppressScript = "" } },
		{ "iu", new Language { Anglonym = "Inuktitut", 		Endonym = "", SuppressScript = "" } },

		{ "ka", new Language { Anglonym = "Georgian", 		Endonym = "", SuppressScript = "" } },
		{ "kg", new Language { Anglonym = "Kongo", 			Endonym = "", SuppressScript = "" } },
		{ "ki", new Language { Anglonym = "Kikuyu", 		Endonym = "", SuppressScript = "" } },
		{ "kj", new Language { Anglonym = "Kwanyama", 		Endonym = "", SuppressScript = "" } },
		{ "kk", new Language { Anglonym = "Kazakh", 		Endonym = "", SuppressScript = "" } },
		{ "kl", new Language { Anglonym = "Greenlandic", 	Endonym = "", SuppressScript = "" } },
		{ "km", new Language { Anglonym = "Khmer", 			Endonym = "", SuppressScript = "" } },
		{ "kn", new Language { Anglonym = "Kannada", 		Endonym = "", SuppressScript = "" } },
		{ "kr", new Language { Anglonym = "Kanuri", 		Endonym = "", SuppressScript = "" } },
		{ "ks", new Language { Anglonym = "Kashmiri", 		Endonym = "", SuppressScript = "" } },
		{ "ku", new Language { Anglonym = "Kurdish", 		Endonym = "", SuppressScript = "" } },
		{ "kv", new Language { Anglonym = "Komi", 			Endonym = "", SuppressScript = "" } },
		{ "kw", new Language { Anglonym = "Cornish", 		Endonym = "", SuppressScript = "" } },
		{ "ky", new Language { Anglonym = "Kyrgyz", 		Endonym = "", SuppressScript = "" } },

		{ "lg", new Language { Anglonym = "Ganda", 			Endonym = "", SuppressScript = "" } },
		{ "li", new Language { Anglonym = "Limburgish", 	Endonym = "", SuppressScript = "" } },
		{ "ln", new Language { Anglonym = "Lingala", 		Endonym = "", SuppressScript = "" } },
		{ "lo", new Language { Anglonym = "Lao", 			Endonym = "", SuppressScript = "" } },
		{ "lt", new Language { Anglonym = "Lithuanian", 	Endonym = "", SuppressScript = "" } },
		{ "lu", new Language { Anglonym = "Luba-Katanga", 	Endonym = "", SuppressScript = "" } },
		{ "lv", new Language { Anglonym = "Latvian", 		Endonym = "", SuppressScript = "" } },

		{ "mg", new Language { Anglonym = "Malagasy", 		Endonym = "", SuppressScript = "" } },
		{ "mh", new Language { Anglonym = "Marshallese", 	Endonym = "", SuppressScript = "" } },
		{ "mi", new Language { Anglonym = "Maori", 			Endonym = "", SuppressScript = "" } },
		{ "mn", new Language { Anglonym = "Mongolian", 		Endonym = "", SuppressScript = "" } },
		{ "mr", new Language { Anglonym = "Marathi", 		Endonym = "", SuppressScript = "" } },
		{ "ms", new Language { Anglonym = "Malay", 			Endonym = "", SuppressScript = "" } },
		{ "mt", new Language { Anglonym = "Maltese", 		Endonym = "", SuppressScript = "" } },
		{ "my", new Language { Anglonym = "Burmese", 		Endonym = "", SuppressScript = "" } },

		{ "na", new Language { Anglonym = "Nauru", 			Endonym = "", SuppressScript = "" } },
		{ "nb", new Language { Anglonym = "Bokmål", 		Endonym = "", SuppressScript = "" } },
		{ "nd", new Language { Anglonym = "Northern Ndebele",Endonym = "", SuppressScript = "" } },
		{ "ne", new Language { Anglonym = "Nepali", 		Endonym = "", SuppressScript = "" } },
		{ "ng", new Language { Anglonym = "Ndonga", 		Endonym = "", SuppressScript = "" } },
		{ "nn", new Language { Anglonym = "Nynorsk", 		Endonym = "", SuppressScript = "" } },
		{ "no", new Language { Anglonym = "Norwegian", 		Endonym = "", SuppressScript = "" } },
		{ "nr", new Language { Anglonym = "Southern Ndebele",Endonym = "", SuppressScript = "" } },
		{ "nv", new Language { Anglonym = "Navajo", 		Endonym = "", SuppressScript = "" } },
		{ "ny", new Language { Anglonym = "Chewa", 			Endonym = "", SuppressScript = "" } },

		{ "oj", new Language { Anglonym = "Ojibwa", 		Endonym = "", SuppressScript = "" } },
		{ "om", new Language { Anglonym = "Oromo", 			Endonym = "", SuppressScript = "" } },
		{ "or", new Language { Anglonym = "Oriya", 			Endonym = "", SuppressScript = "" } },
		{ "os", new Language { Anglonym = "Ossetic", 		Endonym = "", SuppressScript = "" } },

		{ "pa", new Language { Anglonym = "Punjabi", 		Endonym = "", SuppressScript = "" } },
		{ "pi", new Language { Anglonym = "Pali", 			Endonym = "", SuppressScript = "" } },
		{ "ps", new Language { Anglonym = "Pashto", 		Endonym = "", SuppressScript = "" } },

		{ "qu", new Language { Anglonym = "Quechua", 		Endonym = "", SuppressScript = "" } },

		{ "rm", new Language { Anglonym = "Romansh", 		Endonym = "", SuppressScript = "" } },
		{ "rn", new Language { Anglonym = "Rundi", 			Endonym = "", SuppressScript = "" } },
		{ "rw", new Language { Anglonym = "Kinyarwanda", 	Endonym = "", SuppressScript = "" } },

		{ "sa", new Language { Anglonym = "Sanskrit", 		Endonym = "", SuppressScript = "" } },
		{ "sc", new Language { Anglonym = "Sardinian", 		Endonym = "", SuppressScript = "" } },
		{ "sd", new Language { Anglonym = "Sindhi", 		Endonym = "", SuppressScript = "" } },
		{ "se", new Language { Anglonym = "Sami", 			Endonym = "", SuppressScript = "" } },
		{ "sg", new Language { Anglonym = "Sango", 			Endonym = "", SuppressScript = "" } },
		{ "si", new Language { Anglonym = "Sinhala", 		Endonym = "", SuppressScript = "" } },
		{ "sk", new Language { Anglonym = "Slovak", 		Endonym = "", SuppressScript = "" } },
		{ "sl", new Language { Anglonym = "Slovenian", 		Endonym = "", SuppressScript = "" } },
		{ "sm", new Language { Anglonym = "Samoan", 		Endonym = "", SuppressScript = "" } },
		{ "sn", new Language { Anglonym = "Shona", 			Endonym = "", SuppressScript = "" } },
		{ "sq", new Language { Anglonym = "Albanian", 		Endonym = "", SuppressScript = "" } },
		{ "sr", new Language { Anglonym = "Serbian", 		Endonym = "", SuppressScript = "" } },
		{ "ss", new Language { Anglonym = "Swati", 			Endonym = "", SuppressScript = "" } },
		{ "st", new Language { Anglonym = "Sotho", 			Endonym = "", SuppressScript = "" } },
		{ "su", new Language { Anglonym = "Sundanese", 		Endonym = "", SuppressScript = "" } },
		{ "sw", new Language { Anglonym = "Swahili", 		Endonym = "", SuppressScript = "" } },

		{ "ta", new Language { Anglonym = "Tamil", 			Endonym = "", SuppressScript = "" } },
		{ "te", new Language { Anglonym = "Telugu", 		Endonym = "", SuppressScript = "" } },
		{ "tg", new Language { Anglonym = "Tajik", 			Endonym = "", SuppressScript = "" } },
		{ "th", new Language { Anglonym = "Thai", 			Endonym = "", SuppressScript = "" } },
		{ "ti", new Language { Anglonym = "Tigrinya", 		Endonym = "", SuppressScript = "" } },
		{ "tk", new Language { Anglonym = "Turkmen", 		Endonym = "", SuppressScript = "" } },
		{ "tn", new Language { Anglonym = "Tswana", 		Endonym = "", SuppressScript = "" } },
		{ "to", new Language { Anglonym = "Tonga", 			Endonym = "", SuppressScript = "" } },
		{ "ts", new Language { Anglonym = "Tsonga", 		Endonym = "", SuppressScript = "" } },
		{ "tw", new Language { Anglonym = "Twi", 			Endonym = "", SuppressScript = "" } },
		{ "ty", new Language { Anglonym = "Tahitian", 		Endonym = "", SuppressScript = "" } },

		{ "ug", new Language { Anglonym = "Uyghur", 		Endonym = "", SuppressScript = "" } },
		{ "ur", new Language { Anglonym = "Urdu", 			Endonym = "", SuppressScript = "" } },
		{ "uz", new Language { Anglonym = "Uzbek", 			Endonym = "", SuppressScript = "" } },

		{ "ve", new Language { Anglonym = "Venda", 			Endonym = "", SuppressScript = "" } },
		{ "vo", new Language { Anglonym = "Volapük", 		Endonym = "", SuppressScript = "" } },

		{ "wa", new Language { Anglonym = "Walloon", 		Endonym = "", SuppressScript = "" } },
		{ "wo", new Language { Anglonym = "Wolof", 			Endonym = "", SuppressScript = "" } },

		{ "xh", new Language { Anglonym = "Xhosa", 			Endonym = "", SuppressScript = "" } },

		{ "yi", new Language { Anglonym = "Yiddish", 		Endonym = "", SuppressScript = "" } },
		{ "yo", new Language { Anglonym = "Yoruba", 		Endonym = "", SuppressScript = "" } },

		{ "za", new Language { Anglonym = "Zhuang; Chuang", Endonym = "", SuppressScript = "" } },
		{ "zh", new Language { Anglonym = "Chinese", 		Endonym = "", SuppressScript = "" } },
		{ "zu", new Language { Anglonym = "Zulu", 			Endonym = "", SuppressScript = "" } },

	};
}
