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
