using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;

namespace BetterInBlue; 

[Serializable]
public class Configuration : IPluginConfiguration {
    public int Version { get; set; } = 0;
    public List<Loadout> Loadouts { get; set; } = new();

    public void Save() {
        Services.PluginInterface.SavePluginConfig(this);
    }
}
