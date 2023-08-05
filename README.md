# Better in Blue

Better in Blue is a [Dalamud](https://github.com/goatcorp/Dalamud) plugin that adds infinite Blue Mage spell loadouts. Presets can be shared between players and accessed between characters.

Created because of a plugin idea in the [XIVLauncher & Dalamud](https://goat.place/) Discord server - extra thanks to the [Blue Academy](https://discord.gg/blueacademy) Discord server for helping me gauge interest.

## A note on safety

The Blue Mage spellbook uses networking to the server to set a loadout. This means that the plugin is making network requests. I have attempted to wrap it as safely as possible:

- Applying loadouts is one network request triggered via one button click, which means it meets the [official plugin repository rules](https://dalamud.dev/plugin-development/restrictions#what-am-i-allowed-to-do-in-my-plugin)
- Applying a loadout has several checks for the current job, action unlocks, being in combat, invalid data, and more
- The game function used to set the loadout has several clientside checks of its own before making the network request
- You can't create loadouts you can't create normally
