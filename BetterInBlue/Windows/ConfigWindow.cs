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

        var applyToCrossHotbars = Plugin.Configuration.ApplyToCrossHotbars;
        if (ImGui.Checkbox("Apply to crosshotbars", ref applyToCrossHotbars)) {
            Plugin.Configuration.ApplyToCrossHotbars = applyToCrossHotbars;
            Plugin.Configuration.Save();
        }

        if (!applyToCrossHotbars) ImGui.BeginDisabled();
        this.CrossHotbarSelector(true);
        this.CrossHotbarSelector(false);
        if (!applyToCrossHotbars) ImGui.EndDisabled();

        if (ImGui.IsItemHovered()) {
            ImGui.SetTooltip(
                "If checked, applying a loadout will write each action to your hotbars.\n"
                + "Hotbar contents are not saved to your character config until an action is moved."
            );
        }
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

    private void CrossHotbarSelector(bool firstHotbar) {
        var current = firstHotbar
                          ? Plugin.Configuration.CrossHotbarOne
                          : Plugin.Configuration.CrossHotbarTwo;

        if (ImGui.InputInt(firstHotbar ? "CrossHotbar 1" : "CrossHotbar 2", ref current)) {
            if (current > 8) current = 8;
            if (current < 1) current = 1;
            if (firstHotbar) {
                Plugin.Configuration.CrossHotbarOne = current;
            } else {
                Plugin.Configuration.CrossHotbarTwo = current;
            }

            Plugin.Configuration.Save();
        }
    }
}
