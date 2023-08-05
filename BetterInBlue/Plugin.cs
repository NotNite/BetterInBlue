using System;
using System.Collections.Generic;
using Dalamud.Game.Command;
using Dalamud.Plugin;
using BetterInBlue.Windows;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using ImGuiScene;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace BetterInBlue;

public sealed class Plugin : IDalamudPlugin {
    public string Name => "Better in Blue";
    private const string CommandName = "/pblue";

    public WindowSystem WindowSystem = new("BetterInBlue");
    public static Configuration Configuration;
    public MainWindow MainWindow;
    public ConfigWindow ConfigWindow;

    public static ExcelSheet<Action> Action = null!;
    public static ExcelSheet<AozAction> AozAction = null!;
    public static ExcelSheet<AozActionTransient> AozActionTransient = null!;
    public Dictionary<uint, TextureWrap> Icons = new();

    public Plugin(DalamudPluginInterface pluginInterface) {
        pluginInterface.Create<Services>();

        Configuration = Services.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        this.MainWindow = new MainWindow(this);
        this.ConfigWindow = new ConfigWindow(this);

        WindowSystem.AddWindow(MainWindow);
        WindowSystem.AddWindow(ConfigWindow);

        Services.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand) {
            HelpMessage = "Opens the main menu."
        });

        Services.PluginInterface.UiBuilder.Draw += this.DrawUi;
        Services.PluginInterface.UiBuilder.OpenConfigUi += this.OpenConfigUi;

        Action = Services.DataManager.GetExcelSheet<Action>()!;
        AozAction = Services.DataManager.GetExcelSheet<AozAction>()!;
        AozActionTransient = Services.DataManager.GetExcelSheet<AozActionTransient>()!;

        var emptySlot = "ui/uld/DragTargetA_hr1.tex";
        var emptySlotIcon = Services.DataManager.GetImGuiTexture(emptySlot)!;
        this.Icons.Add(0, emptySlotIcon);

        foreach (var aozAction in AozAction) {
            if (aozAction.RowId == 0) continue;
            var transient = AozActionTransient.GetRow(aozAction.RowId)!;
            var icon = Services.DataManager.GetImGuiTextureIcon(transient.Icon, true)!;
            this.Icons.Add(aozAction.RowId, icon);
        }
    }

    public void Dispose() {
        this.WindowSystem.RemoveAllWindows();
        MainWindow.Dispose();
        Services.CommandManager.RemoveHandler(CommandName);
        foreach (var icon in this.Icons.Values) icon.Dispose();
        this.Icons.Clear();
    }

    private void OnCommand(string command, string args) {
        switch (args) {
            case "config":
            case "settings":
                this.OpenConfigUi();
                break;

            default:
                this.MainWindow.IsOpen = true;
                break;
        }
    }

    private void DrawUi() {
        this.WindowSystem.Draw();
    }

    public void OpenConfigUi() {
        this.ConfigWindow.IsOpen = true;
    }

    public static uint AozToNormal(uint id) {
        if (id == 0) return 0;
        return AozAction.GetRow(id)!.Action.Row;
    }

    public static uint NormalToAoz(uint id) {
        foreach (var aozAction in AozAction) {
            if (aozAction.Action.Row == id) return aozAction.RowId;
        }

        throw new Exception("https://tenor.com/view/8032213");
    }

    // https://github.com/Ottermandias/OtterGui/blob/03b6b17fee66488fff7f598e444fa99454098767/Util.cs#L263
    public static bool DisabledButtonWithTooltip(
        FontAwesomeIcon icon, string text, bool disabled, bool onlyShowWhenDisabled = false
    ) {
        if (disabled) ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.5f);
        var ret = ImGuiComponents.IconButton(icon);
        if (disabled) ImGui.PopStyleVar();

        var shouldShowTooltip = onlyShowWhenDisabled ? disabled : true;
        if (shouldShowTooltip && ImGui.IsItemHovered()) ImGui.SetTooltip(text);

        return ret && !disabled;
    }
}
