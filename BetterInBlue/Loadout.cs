using System;
using System.Linq;
using System.Text.Json;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;

namespace BetterInBlue;

[Serializable]
public class Loadout {
    public string Name { get; set; }
    public uint[] Actions { get; set; } = new uint[24];

    public Loadout(string name = "Unnamed Loadout") {
        this.Name = name;
    }

    public static Loadout? FromPreset(string preset) {
        try {
            var bytes = Convert.FromBase64String(preset);
            var str = System.Text.Encoding.UTF8.GetString(bytes);
            return JsonSerializer.Deserialize<Loadout>(str);
        } catch (Exception) {
            return null;
        }
    }

    public string ToPreset() {
        var str = JsonSerializer.Serialize(this);
        var bytes = System.Text.Encoding.UTF8.GetBytes(str);
        return Convert.ToBase64String(bytes);
    }

    public int ActionCount(uint id) {
        return this.Actions.Count(x => x == id);
    }

    public unsafe bool ActionUnlocked(uint id) {
        var normalId = Plugin.AozToNormal(id);
        var link = Plugin.Action.GetRow(normalId)!.UnlockLink;
        return UIState.Instance()->IsUnlockLinkUnlocked(link);
    }

    public bool CanApply() {
        // Must be BLU to apply (id = 36)
        if (Services.ClientState.LocalPlayer?.ClassJob.Id != 36) return false;

        // Can't apply in combat
        if (Services.Condition[ConditionFlag.InCombat]) return false;

        foreach (var action in this.Actions) {
            // No out of bounds indexing
            if (action > Plugin.AozAction.RowCount) return false;

            if (action != 0) {
                // Can't have two actions in the same loadout
                if (this.ActionCount(action) > 1) return false;

                // Can't apply an action you don't have
                if (!this.ActionUnlocked(action)) return false;
            }
        }

        // aight we good
        return true;
    }

    public unsafe bool Apply() {
        var actionManager = ActionManager.Instance();

        var arr = new uint[24];
        for (var i = 0; i < 24; i++) {
            arr[i] = Plugin.AozToNormal(this.Actions[i]);
        }

        fixed (uint* ptr = arr) {
            var ret = actionManager->SetBlueMageActions(ptr);
            if (ret == false) return false;
        }

        if (Plugin.Configuration.ApplyToHotbars) {
            this.ApplyToHotbar(
                Plugin.Configuration.HotbarOne,
                this.Actions[..12]
            );

            this.ApplyToHotbar(
                Plugin.Configuration.HotbarTwo,
                this.Actions[12..]
            );
        }

        if(Plugin.Configuration.ApplyToCrossHotbars){
            this.ApplyToHotbar(
                Plugin.Configuration.CrossHotbarOne + 10,
                this.Actions[..16],
                true
            );

            this.ApplyToHotbar(
                Plugin.Configuration.CrossHotbarTwo + 10,
                this.Actions[16..],
                true
            );
        }

        return true;
    }

    private unsafe void ApplyToHotbar(int id, uint[] aozActions, bool crossbar = false) {
        var hotbarModule = RaptureHotbarModule.Instance();

        var maxSlots = !crossbar ? 12 : 16;
        
        for (var i = 0; i < maxSlots; i++) {
            var aozAction = aozActions[i];
            var normalAction = Plugin.AozToNormal(aozAction);
            var slot = !crossbar 
                            ? hotbarModule->GetSlotById((uint) (id - 1), (uint) i)
                            : hotbarModule->GetSlotById((uint) (id - 1), (uint) (15 - i));

            if (normalAction == 0) {
                // DO NOT SET ACTION 0 YOU WILL GET CURE'D
                slot->Set(
                    HotbarSlotType.Empty,
                    0
                );
            } else {
                slot->Set(
                    HotbarSlotType.Action,
                    normalAction
                );
            }
        }
    }
}
