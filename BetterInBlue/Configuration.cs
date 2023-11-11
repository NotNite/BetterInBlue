using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using Lumina.Excel.GeneratedSheets;

namespace BetterInBlue;

[Serializable]
public class Configuration : IPluginConfiguration {
    public int Version { get; set; } = 1;
    public List<Loadout> Loadouts { get; set; } = new();

    public bool ApplyToHotbars { get; set; } = false;
    public bool ApplyToCrossHotbars { get; set; } = false;
    public int HotbarOne { get; set; } = 1;
    public int HotbarTwo { get; set; } = 2;
    public int CrossHotbarOne { get; set; } = 1;
    public int CrossHotbarTwo { get; set; } = 2;

    public void Save() {
        Services.PluginInterface.SavePluginConfig(this);
    }
}
