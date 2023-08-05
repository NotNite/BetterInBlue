using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace BetterInBlue.Windows;

public class ConfigWindow : Window, IDisposable {
    private Plugin plugin;

    public ConfigWindow(Plugin plugin) : base("Better in Blue Config") {
        this.plugin = plugin;
        this.Size = new Vector2(450, 400);
        this.SizeCondition = ImGuiCond.FirstUseEver;
    }

    public void Dispose() { }

    public override void Draw() {
        var applyToHotbars = Plugin.Configuration.ApplyToHotbars;
        if (ImGui.Checkbox("Apply to hotbars", ref applyToHotbars)) {
            Plugin.Configuration.ApplyToHotbars = applyToHotbars;
            Plugin.Configuration.Save();
        }

        if (!applyToHotbars) ImGui.BeginDisabled();
        this.HotbarSelector(true);
        this.HotbarSelector(false);
        if (!applyToHotbars) ImGui.EndDisabled();
    }

    private void HotbarSelector(bool firstHotbar) {
        var current = firstHotbar
                          ? Plugin.Configuration.HotbarOne
                          : Plugin.Configuration.HotbarTwo;

        if (ImGui.InputInt(firstHotbar ? "Hotbar 1" : "Hotbar 2", ref current)) {
            if (current > 10) current = 10;
            if (current < 1) current = 1;

            if (firstHotbar) {
                Plugin.Configuration.HotbarOne = current;
            } else {
                Plugin.Configuration.HotbarTwo = current;
            }

            Plugin.Configuration.Save();
        }
    }
}
