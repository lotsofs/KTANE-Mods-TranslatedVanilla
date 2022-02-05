# Translated Vanilla Modules

This is a mod for the game [_Keep Talking and Nobody Explodes_](https://keeptalkinggame.com/) which adds translations to the vanilla modules.

The code for this project heavily borrows from Andrio Celos' hard work on [Not Vanilla Modules](https://github.com/AndrioCelos/KtaneNotVanillaModules) ([Steam Workshop](https://steamcommunity.com/sharedfiles/filedetails/?id=2003251353)) which add a twist to the vanilla modules.

<!-- A build is available on the [Steam Workshop](https://steamcommunity.com/sharedfiles/filedetails/?id=2003251353). -->

Manuals are available on the [Repository of Manual Pages](https://ktane.timwi.de/). 
<!-- Todo: Link to my own website as well. -->

Based on the [_Keep Talking and Nobody Explodes_ modkit](https://github.com/keeptalkinggame/ktanemodkit/).

## How to build

Building this mod is a little more involved than with most mods, but works the same as with the Not Modules.

1. Open [the helper plugin Visual Studio project](NotVanillaModulesLib/NotVanillaModulesLib.csproj) with a text editor and ensure the variables `UnityInstallPath` and `GameInstallPath` are set to the correct paths for your installations. Then delete the `PreBuildEvent` directive.
2. Set the build configuration to Debug and build the helper plugin.
3. Open this repository using Unity 2017 LTS.
4. In Unity, select `Keep Talking ModKit` → `Configure Mod` and update the build path to match your installation.
5. Select `Keep Talking ModKit` → `Build AssetBundle`.
6. Set the build configuration to Release and rebuild the helper plugin. The library will automatically be copied to the installed mod directory.

See [the helper plugin readme file](NotVanillaModulesLib/readme.md) for more information.


## Documentation
language tags are based on https://tools.ietf.org/rfc/bcp/bcp47.txt

### Machine Translation platforms ###
und - *und*etermined
wkp - *w*i*k*i*p*edia - Simple terms such as color names etc. were taken from the titles of their wikipedia articles in their respective languages (or similar websites), but not checked against declensions etc. 
gtr - *g*oogle *tr*anslate - Simply Google Translation (or some other robot translation service). Likely very wrong.

### Private Language Subtags ###
Private versions: 4 alpha characters. Currently in-use:
orig - *Orig*inal translations from Tharagon's mod in use and unchanged since 2018.
nvml - Translations provided in this remake using AndrioCelos's *N*ot *V*anilla *M*odules *l*ibrary.
offc - *Off*i*c*ial translations as provided by Steel Crate Games in their 2020 localization update.

Other information: 5 alpha characters. Currently in-use:
untrc - *Unt*ranslated *c*ontent - Use when some provided translation did not actually translate the content, eg. various of the Asian morse codes which use latin words like shell, halls, etc. anyway.

v# - *V*ersion *#* (where # is a number)