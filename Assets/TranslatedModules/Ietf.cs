using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public static class Ietf {
	public class Language {
		public string Anglonym;
		public string Autonym;
		public string SuppressScript;
		public bool RightToLeft;
		public bool IsMacroLanguage;
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


	// from https://en.wikipedia.org/wiki/List_of_ISO_639-1_codes ad 2022-01-29
	public static Dictionary<string, Language> Languages = new Dictionary<string, Language> {
		{ "-", new Language { Anglonym = "Default",         Autonym = "English",            SuppressScript = "Latn" } },

		{ "aa", new Language { Anglonym = "Afar", 			Autonym = "Afaraf",				SuppressScript = null } },
		{ "ab", new Language { Anglonym = "Abkhazian", 		Autonym = "аҧсуа бызшәа",		SuppressScript = "Cyrl" } },
		{ "ae", new Language { Anglonym = "Avestan", 		Autonym = "avesta",				SuppressScript = null } },
		{ "af", new Language { Anglonym = "Afrikaans",		Autonym = "Afrikaans",			SuppressScript = "Latn" } },
		//{ "ak", new Language { Anglonym = "Akan", 			Autonym = "Akan",				SuppressScript = null} }, // Macro Language, use more specific primary language subtag instead
		{ "am", new Language { Anglonym = "Amharic", 		Autonym = "አማርኛ",				SuppressScript = "Ethi" } },
		{ "an", new Language { Anglonym = "Aragonese", 		Autonym = "aragonés",			SuppressScript = null } },
		{ "ar", new Language { Anglonym = "Arabic",			Autonym = "العربية",			SuppressScript = "Arab", RightToLeft = true} }, // Macro Language, but needed for legacy reasons
		{ "as", new Language { Anglonym = "Assamese", 		Autonym = "অসমীয়া",				SuppressScript = "Beng" } },
		{ "av", new Language { Anglonym = "Avaric", 		Autonym = "авар мацӀ",			SuppressScript = null } },
		//{ "ay", new Language { Anglonym = "Aymara", 		Autonym = "aymar aru",			SuppressScript = "Latn"} }, // Macro Language, use more specific primary language subtag instead
		{ "az", new Language { Anglonym = "Azerbaijani",	Autonym = "azərbaycan dili",	SuppressScript = null} }, // Macro Language, but needed for legacy reasons

		{ "ba", new Language { Anglonym = "Bashkir", 		Autonym = "башҡорт теле",		SuppressScript = null } },
		{ "be", new Language { Anglonym = "Belarusian",		Autonym = "беларуская мова",	SuppressScript = "Cyrl" } },
		{ "bg", new Language { Anglonym = "Bulgarian",		Autonym = "български език",		SuppressScript = "Cyrl" } },
		{ "bi", new Language { Anglonym = "Bislama", 		Autonym = "Bislama",			SuppressScript = null } },
		{ "bm", new Language { Anglonym = "Bambara", 		Autonym = "bamanankan",			SuppressScript = null } },
		{ "bn", new Language { Anglonym = "Bengali",		Autonym = "বাংলা",				SuppressScript = "Beng" } },
		{ "bo", new Language { Anglonym = "Tibetan", 		Autonym = "བོད་ཡིག",				SuppressScript = null } },
		{ "br", new Language { Anglonym = "Breton",			Autonym = "brezhoneg",			SuppressScript = null } },
		{ "bs", new Language { Anglonym = "Bosnian", 		Autonym = "bosanski jezik",		SuppressScript = null } },

		{ "ca", new Language { Anglonym = "Catalan",		Autonym = "català",				SuppressScript = "Latn" } },
		{ "ce", new Language { Anglonym = "Chechen", 		Autonym = "нохчийн мотт",		SuppressScript = null } },
		{ "ch", new Language { Anglonym = "Chamorro", 		Autonym = "Chamoru",			SuppressScript = "Latn" } },
		{ "co", new Language { Anglonym = "Corsican", 		Autonym = "corsu",				SuppressScript = null } },
		//{ "cr", new Language { Anglonym = "Cree", 			Autonym = "ᓀᐦᐃᔭᐍᐏᐣ",			SuppressScript = null} }, // Macro Language, use more specific primary language subtag instead
		{ "cs", new Language { Anglonym = "Czech",			Autonym = "čeština",			SuppressScript = "Latn" } },
		{ "cu", new Language { Anglonym = "Church Slavic",	Autonym = "ѩзыкъ словѣньскъ",	SuppressScript = null } },
		{ "cv", new Language { Anglonym = "Chuvash", 		Autonym = "чӑваш чӗлхи",		SuppressScript = null } },
		{ "cy", new Language { Anglonym = "Welsh", 			Autonym = "Cymraeg",			SuppressScript = "Latn" } },

		{ "da", new Language { Anglonym = "Danish", 		Autonym = "dansk",				SuppressScript = "Latn" } },
		{ "de", new Language { Anglonym = "German",			Autonym = "Deutsch",			SuppressScript = "Latn" } },
		{ "dv", new Language { Anglonym = "Divehi", 		Autonym = "ދިވެހި",					SuppressScript = "Thaa", RightToLeft = true } },
		{ "dz", new Language { Anglonym = "Dzongkha", 		Autonym = "རྫོང་ཁ",					SuppressScript = "Tibt" } },

		{ "ee", new Language { Anglonym = "Ewe", 			Autonym = "Eʋegbe",				SuppressScript = null } },
		{ "el", new Language { Anglonym = "Greek",			Autonym = "Ελληνικά",			SuppressScript = "Grek" } },
		{ "en", new Language { Anglonym = "English", 		Autonym = "English",			SuppressScript = "Latn" } },
		{ "eo", new Language { Anglonym = "Esperanto", 		Autonym = "Esperanto",			SuppressScript = "Latn" } },
		{ "es", new Language { Anglonym = "Spanish", 		Autonym = "Español",			SuppressScript = "Latn" } },
		{ "et", new Language { Anglonym = "Estonian", 		Autonym = "eesti",				SuppressScript = "Latn"} }, // Macro Language, but needed for legacy reasons
		{ "eu", new Language { Anglonym = "Basque", 		Autonym = "euskara",			SuppressScript = "Latn" } },

		//{ "fa", new Language { Anglonym = "Persian", 		Autonym = "فارسی",				SuppressScript = "Arab", RightToLeft = true} }, // Macro Language, use more specific primary language subtag instead
		//{ "ff", new Language { Anglonym = "Fulah", 			Autonym = "Fulfulde",			SuppressScript = null} }, // Macro Language, use more specific primary language subtag instead
		{ "fi", new Language { Anglonym = "Finnish", 		Autonym = "suomi",				SuppressScript = "Latn" } },
		{ "fj", new Language { Anglonym = "Fijian", 		Autonym = "vosa Vakaviti",		SuppressScript = "Latn" } },
		{ "fo", new Language { Anglonym = "Faroese", 		Autonym = "føroyskt",			SuppressScript = "Latn" } },
		{ "fr", new Language { Anglonym = "French", 		Autonym = "français",			SuppressScript = "Latn" } },
		{ "fy", new Language { Anglonym = "Western Frisian",Autonym = "Frysk",				SuppressScript = "Latn" } },

		{ "ga", new Language { Anglonym = "Irish", 			Autonym = "Gaeilge",			SuppressScript = "Latn" } },
		{ "gd", new Language { Anglonym = "Gaelic", 		Autonym = "Gàidhlig",			SuppressScript = null } },
		{ "gl", new Language { Anglonym = "Galician", 		Autonym = "Galego",				SuppressScript = "Latn" } },
		//{ "gn", new Language { Anglonym = "Guarani", 		Autonym = "Avañe'ẽ",			SuppressScript = "Latn"} }, // Macro Language, use more specific primary language subtag instead
		{ "gu", new Language { Anglonym = "Gujarati", 		Autonym = "ગુજરાતી",				SuppressScript = "Gujr" } },
		{ "gv", new Language { Anglonym = "Manx", 			Autonym = "Gaelg",				SuppressScript = "Latn" } },

		{ "ha", new Language { Anglonym = "Hausa", 			Autonym = "هَوُسَ",				SuppressScript = null, RightToLeft = true } },
		{ "he", new Language { Anglonym = "Hebrew", 		Autonym = "עברית",				SuppressScript = "Hebr", RightToLeft = true } },
		{ "hi", new Language { Anglonym = "Hindi", 			Autonym = "हिन्दी",				SuppressScript = "Deva" } },
		{ "ho", new Language { Anglonym = "Hiri Motu", 		Autonym = "Hiri Motu",			SuppressScript = null } },
		{ "hr", new Language { Anglonym = "Croatian", 		Autonym = "hrvatski jezik",		SuppressScript = "Latn" } },
		{ "ht", new Language { Anglonym = "Haitian Creole", Autonym = "Kreyòl ayisyen",		SuppressScript = "Latn" } },
		{ "hu", new Language { Anglonym = "Hungarian", 		Autonym = "magyar",				SuppressScript = "Latn" } },
		{ "hy", new Language { Anglonym = "Armenian",		Autonym = "Հայերեն",			SuppressScript = "Armn" } },
		{ "hz", new Language { Anglonym = "Herero", 		Autonym = "Otjiherero",			SuppressScript = null } },

		{ "ia", new Language { Anglonym = "Interlingua",	Autonym = "Interlingia",		SuppressScript = null } },
		{ "id", new Language { Anglonym = "Indonesian",		Autonym = "Bahasa Indonesia",	SuppressScript = "Latn" } },
		{ "ie", new Language { Anglonym = "Interlingue", 	Autonym = "Interlingue",		SuppressScript = null } },
		{ "ig", new Language { Anglonym = "Igbo", 			Autonym = "Asụsụ Igbo",			SuppressScript = null } },
		{ "ii", new Language { Anglonym = "Sichuan Yi", 	Autonym = "ꆈꌠ꒿",				SuppressScript = null } },
		//{ "ik", new Language { Anglonym = "Inupiaq", 		Autonym = "Iñupiaq",			SuppressScript = null} }, // Macro Language, use more specific primary language subtag instead
		{ "io", new Language { Anglonym = "Ido", 			Autonym = "Ido",				SuppressScript = null } },
		{ "is", new Language { Anglonym = "Icelandic", 		Autonym = "Íslenska",			SuppressScript = "Latn" } },
		{ "it", new Language { Anglonym = "Italian", 		Autonym = "Italiano",			SuppressScript = "Latn" } },
		//{ "iu", new Language { Anglonym = "Inuktitut", 		Autonym = "ᐃᓄᒃᑎᑐᑦ",				SuppressScript = null} }, // Macro Language, use more specific primary language subtag instead

		{ "ja", new Language { Anglonym = "Japanese", 		Autonym = "日本語",				SuppressScript = "Jpan" } },
		{ "jv", new Language { Anglonym = "Javanese", 		Autonym = "ꦧꦱꦗꦮ",			SuppressScript = null } },

		{ "ka", new Language { Anglonym = "Georgian", 		Autonym = "ქართული",			SuppressScript = "Geor" } },
		//{ "kg", new Language { Anglonym = "Kongo", 			Autonym = "Kikongo",			SuppressScript = null} }, // Macro Language, use more specific primary language subtag instead
		{ "ki", new Language { Anglonym = "Kikuyu", 		Autonym = "Gĩkũyũ",				SuppressScript = null } },
		{ "kj", new Language { Anglonym = "Kuanyama", 		Autonym = "Kuanyama",			SuppressScript = null } },
		{ "kk", new Language { Anglonym = "Kazakh", 		Autonym = "қазақ тілі",			SuppressScript = "Cyrl" } },
		{ "kl", new Language { Anglonym = "Kalaallisut", 	Autonym = "kalaallisut",		SuppressScript = "Latn" } },
		{ "km", new Language { Anglonym = "Central Khmer", 	Autonym = "ខ្មែរ",					SuppressScript = "Khmr" } },
		{ "kn", new Language { Anglonym = "Kannada", 		Autonym = "ಕನ್ನಡ",				SuppressScript = "Knda" } },
		{ "ko", new Language { Anglonym = "Korean", 		Autonym = "한국어",				SuppressScript = "Kore" } },
		//{ "kr", new Language { Anglonym = "Kanuri", 		Autonym = "Kanuri",				SuppressScript = null} }, // Macro Language, use more specific primary language subtag instead
		{ "ks", new Language { Anglonym = "Kashmiri", 		Autonym = "कॉशुर",				SuppressScript = null } },
		//{ "ku", new Language { Anglonym = "Kurdish", 		Autonym = "Kurdî",				SuppressScript = null} }, // Macro Language, use more specific primary language subtag instead
		//{ "kv", new Language { Anglonym = "Komi", 			Autonym = "коми кыв",			SuppressScript = null} }, // Macro Language, use more specific primary language subtag instead
		{ "kw", new Language { Anglonym = "Cornish", 		Autonym = "Kernewek",			SuppressScript = null } },
		{ "ky", new Language { Anglonym = "Kirghiz", 		Autonym = "Кыргызча",			SuppressScript = null } },

		{ "la", new Language { Anglonym = "Latin", 			Autonym = "latine",				SuppressScript = "Latn" } },
		{ "lb", new Language { Anglonym = "Luxembourgish",	Autonym = "Lëtzebuergesch",		SuppressScript = "Latn" } },
		{ "lg", new Language { Anglonym = "Ganda", 			Autonym = "Luganda",			SuppressScript = null } },
		{ "li", new Language { Anglonym = "Limburgan", 		Autonym = "Limburgs",			SuppressScript = null } },
		{ "ln", new Language { Anglonym = "Lingala", 		Autonym = "Lingála",			SuppressScript = "Latn" } },
		{ "lo", new Language { Anglonym = "Lao", 			Autonym = "ພາສາລາວ",			SuppressScript = "Laoo" } },
		{ "lt", new Language { Anglonym = "Lithuanian", 	Autonym = "lietuvių kalba",		SuppressScript = "Latn" } },
		{ "lu", new Language { Anglonym = "Luba-Katanga", 	Autonym = "Kiluba",				SuppressScript = null } },
		//{ "lv", new Language { Anglonym = "Latvian", 		Autonym = "latviešu valoda",	SuppressScript = "Latn"} }, // Macro Language, use more specific primary language subtag instead


		//{ "mg", new Language { Anglonym = "Malagasy", 		Autonym = "fiteny malagasy",	SuppressScript = "Latn"} }, // Macro Language, use more specific primary language subtag instead
		{ "mh", new Language { Anglonym = "Marshallese", 	Autonym = "Kajin M̧ajeļ",		SuppressScript = "Latn" } },
		{ "mi", new Language { Anglonym = "Maori", 			Autonym = "te reo Māori",		SuppressScript = null } },
		{ "mk", new Language { Anglonym = "Macedonian", 	Autonym = "Македонски",			SuppressScript = "Cyrl" } },
		{ "ml", new Language { Anglonym = "Malayalam", 		Autonym = "മലയാളം",			SuppressScript = "Mlym" } },
		//{ "mn", new Language { Anglonym = "Mongolian", 		Autonym = "Монгол хэл",			SuppressScript = null} }, // Macro Language, use more specific primary language subtag instead
		{ "mr", new Language { Anglonym = "Marathi", 		Autonym = "मराठी",				SuppressScript = "Deva" } },
		//{ "ms", new Language { Anglonym = "Malay", 			Autonym = "Bahasa Melayu",		SuppressScript = "Latn"} }, // Macro Language, use more specific primary language subtag instead
		{ "mt", new Language { Anglonym = "Maltese", 		Autonym = "Malti",				SuppressScript = "Latn" } },
		{ "my", new Language { Anglonym = "Burmese", 		Autonym = "ဗမာစာ",				SuppressScript = "Mymr" } },

		{ "na", new Language { Anglonym = "Nauru", 			Autonym = "Dorerin Naoero",		SuppressScript = "Latn" } },
		{ "nb", new Language { Anglonym = "Norwegian Bokmål",Autonym = "Norsk Bokmål",		SuppressScript = "Latn" } },
		{ "nd", new Language { Anglonym = "North Ndebele",	Autonym = "isiNdebele",			SuppressScript = "Latn" } },
		//{ "ne", new Language { Anglonym = "Nepali", 		Autonym = "नेपाली",				SuppressScript = "Deva"} }, // Macro Language, use more specific primary language subtag instead
		{ "ng", new Language { Anglonym = "Ndonga", 		Autonym = "Owambo",				SuppressScript = null } },
		{ "nl", new Language { Anglonym = "Dutch", 			Autonym = "Nederlands",			SuppressScript = "Latn" } },
		{ "nn", new Language { Anglonym = "Norwegian Nynorsk",Autonym = "Norsk Nynorsk",	SuppressScript = "Latn" } },
		{ "no", new Language { Anglonym = "Norwegian", 		Autonym = "Norsk",				SuppressScript = "Latn"} }, // Macro Language, but needed for legacy reasons
		{ "nr", new Language { Anglonym = "South Ndebele",	Autonym = "isiNdebele",			SuppressScript = "Latn" } },
		{ "nv", new Language { Anglonym = "Navajo", 		Autonym = "Diné bizaad",		SuppressScript = null } },
		{ "ny", new Language { Anglonym = "Chichewa", 		Autonym = "chiCheŵa",			SuppressScript = "Latn" } },

		{ "oc", new Language { Anglonym = "Occitan", 		Autonym = "Occitan",			SuppressScript = null } },
		//{ "oj", new Language { Anglonym = "Ojibwa", 		Autonym = "ᐊᓂᔑᓈᐯᒧᐎᓐ",			SuppressScript = null} }, // Macro Language, use more specific primary language subtag instead
		//{ "om", new Language { Anglonym = "Oromo", 			Autonym = "Afaan Oromoo",		SuppressScript = "Latn"} }, // Macro Language, use more specific primary language subtag instead
		//{ "or", new Language { Anglonym = "Oriya", 			Autonym = "ଓଡ଼ିଆ",				SuppressScript = "Orya"} }, // Macro Language, use more specific primary language subtag instead
		{ "os", new Language { Anglonym = "Ossetian", 		Autonym = "ирон ӕвзаг",			SuppressScript = null } },

		{ "pa", new Language { Anglonym = "Punjabi", 		Autonym = "ਪੰਜਾਬੀ",				SuppressScript = "Guru" } },
		{ "pi", new Language { Anglonym = "Pali", 			Autonym = "पालि",				SuppressScript = null } },
		{ "pl", new Language { Anglonym = "Polish", 		Autonym = "język polski",		SuppressScript = "Latn" } },
		{ "ps", new Language { Anglonym = "Pashto", 		Autonym = "پښتو",				SuppressScript = "Arab", RightToLeft = true} }, // Macro Language, use more specific primary language subtag instead
		{ "pt", new Language { Anglonym = "Portuguese", 	Autonym = "Português",			SuppressScript = "Latn" } },

		//{ "qu", new Language { Anglonym = "Quechua",        Autonym = "Runa Simi",			SuppressScript = "Latn"} }, // Macro Language, use more specific primary language subtag instead

		{ "rm", new Language { Anglonym = "Romansh", 		Autonym = "Rumantsch Grischun", SuppressScript = "Latn" } },
		{ "rn", new Language { Anglonym = "Rundi", 			Autonym = "Ikirundi",			SuppressScript = "Latn" } },
		{ "ro", new Language { Anglonym = "Romanian", 		Autonym = "Română",				SuppressScript = "Latn" } },
		{ "ru", new Language { Anglonym = "Russian", 		Autonym = "русский",			SuppressScript = "Cyrl" } },
		{ "rw", new Language { Anglonym = "Kinyarwanda", 	Autonym = "Ikinyarwanda",		SuppressScript = "Latn" } },

		{ "sa", new Language { Anglonym = "Sanskrit", 		Autonym = "संस्कृतम्",				SuppressScript = null } },
		//{ "sc", new Language { Anglonym = "Sardinian", 		Autonym = "sardu",				SuppressScript = null} }, // Macro Language, use more specific primary language subtag instead
		{ "sd", new Language { Anglonym = "Sindhi", 		Autonym = "सिंधी",				SuppressScript = null } },
		{ "se", new Language { Anglonym = "Northern Sami", 	Autonym = "Davvisámegiella",	SuppressScript = null } },
		{ "sg", new Language { Anglonym = "Sango", 			Autonym = "yângâ tî sängö",		SuppressScript = "Latn" } },
		{ "si", new Language { Anglonym = "Sinhala", 		Autonym = "සිංහල",				SuppressScript = "Sinh" } },
		{ "sk", new Language { Anglonym = "Slovak", 		Autonym = "slovenčina",			SuppressScript = "Latn" } },
		{ "sl", new Language { Anglonym = "Slovenian", 		Autonym = "Slovenski jezik",	SuppressScript = "Latn" } },
		{ "sm", new Language { Anglonym = "Samoan", 		Autonym = "gagana fa'a Samoa",	SuppressScript = "Latn" } },
		{ "sn", new Language { Anglonym = "Shona", 			Autonym = "chiShona",			SuppressScript = null } },
		{ "so", new Language { Anglonym = "Somali", 		Autonym = "Soomaaliga",			SuppressScript = "Latn" } },
		//{ "sq", new Language { Anglonym = "Albanian", 		Autonym = "Shqip",				SuppressScript = "Latn"} }, // Macro Language, use more specific primary language subtag instead
		{ "sr", new Language { Anglonym = "Serbian", 		Autonym = "српски језик",		SuppressScript = null } },
		{ "ss", new Language { Anglonym = "Swati", 			Autonym = "SiSwati",			SuppressScript = "Latn" } },
		{ "st", new Language { Anglonym = "Southern Sotho", Autonym = "Sesotho",			SuppressScript = "Latn" } },
		{ "su", new Language { Anglonym = "Sundanese", 		Autonym = "Basa Sunda",			SuppressScript = null } },
		{ "sv", new Language { Anglonym = "Swedish", 		Autonym = "Svenska",			SuppressScript = "Latn" } },
		//{ "sw", new Language { Anglonym = "Swahili", 		Autonym = "Kiswahili",			SuppressScript = "Latn"} }, // Macro Language, use more specific primary language subtag instead

		{ "ta", new Language { Anglonym = "Tamil", 			Autonym = "தமிழ்",				SuppressScript = "Taml" } },
		{ "te", new Language { Anglonym = "Telugu", 		Autonym = "తెలుగు",				SuppressScript = "Telu" } },
		{ "tg", new Language { Anglonym = "Tajik", 			Autonym = "тоҷикӣ",				SuppressScript = null } },
		{ "th", new Language { Anglonym = "Thai", 			Autonym = "ไทย",					SuppressScript = "Thai" } },
		{ "ti", new Language { Anglonym = "Tigrinya", 		Autonym = "ትግርኛ",				SuppressScript = "Ethi" } },
		{ "tk", new Language { Anglonym = "Turkmen", 		Autonym = "Türkmençe",			SuppressScript = null } },
		{ "tl", new Language { Anglonym = "Tagalog", 		Autonym = "Wikang Tagalog",		SuppressScript = "Latn" } },
		{ "tn", new Language { Anglonym = "Tswana", 		Autonym = "Setswana",			SuppressScript = "Latn" } },
		{ "to", new Language { Anglonym = "Tonga", 			Autonym = "Faka Tonga",			SuppressScript = "Latn" } },
		{ "tr", new Language { Anglonym = "Turkish", 		Autonym = "Türkçe",				SuppressScript = "Latn" } },
		{ "ts", new Language { Anglonym = "Tsonga", 		Autonym = "Xitsonga",			SuppressScript = null } },
		{ "tt", new Language { Anglonym = "Tatar", 			Autonym = "татар теле",			SuppressScript = null } },
		{ "tw", new Language { Anglonym = "Twi", 			Autonym = "Twi",				SuppressScript = null } },
		{ "ty", new Language { Anglonym = "Tahitian", 		Autonym = "Reo Tahiti",			SuppressScript = null } },

		{ "ug", new Language { Anglonym = "Uighur", 		Autonym = "ئۇيغۇرچە",			SuppressScript = null, RightToLeft = true } },
		{ "uk", new Language { Anglonym = "Ukrainian", 		Autonym = "Українська",			SuppressScript = "Cyrl" } },
		{ "ur", new Language { Anglonym = "Urdu", 			Autonym = "اردو",				SuppressScript = "Arab", RightToLeft = true } },
		//{ "uz", new Language { Anglonym = "Uzbek", 			Autonym = "Oʻzbek",				SuppressScript = null} }, // Macro Language, use more specific primary language subtag instead


		{ "ve", new Language { Anglonym = "Venda",          Autonym = "Tshivenḓa",			SuppressScript = "Latn" } },
		{ "vi", new Language { Anglonym = "Vietnamese", 	Autonym = "Tiếng Việt",			SuppressScript = "Latn" } },
		{ "vo", new Language { Anglonym = "Volapük",        Autonym = "Volapük",			SuppressScript = null } },
		
		{ "wa", new Language { Anglonym = "Walloon", 		Autonym = "Walon",				SuppressScript = null } },
		{ "wo", new Language { Anglonym = "Wolof", 			Autonym = "Wollof",				SuppressScript = null } },

		{ "xh", new Language { Anglonym = "Xhosa", 			Autonym = "isiXhosa",			SuppressScript = "Latn" } },

		//{ "yi", new Language { Anglonym = "Yiddish", 		Autonym = "ייִדיש",				SuppressScript = "Hebr", RightToLeft = true} }, // Macro Language, use more specific primary language subtag instead
		{ "yo", new Language { Anglonym = "Yoruba", 		Autonym = "Yorùbá",				SuppressScript = null } },

		//{ "za", new Language { Anglonym = "Zhuang",			Autonym = "Saɯ cueŋƅ",			SuppressScript = null} }, // Macro Language, use more specific primary language subtag instead
		{ "zh", new Language { Anglonym = "Chinese", 		Autonym = "中文",				SuppressScript = null} }, // Macro Language, but needed for legacy reasons
		{ "zu", new Language { Anglonym = "Zulu", 			Autonym = "isiZulu",			SuppressScript = "Latn" } },

	};
}
