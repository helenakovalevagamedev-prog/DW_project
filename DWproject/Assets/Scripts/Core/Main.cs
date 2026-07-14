using UnityEngine;
using Naninovel;

public class Main : MonoBehaviour
{
    private const string VarTalkedToNpc = "TalkedToNpc"; // CustomVariablesManager Naninovel
    private const string VarSafeChecked = "SafeChecked"; // CustomVariablesManager Naninovel
    private const string NpcFirstMeet = "NpcFirstMeet";
    private const string NpcThanks = "NpcThanks";
    private const string NpcHintKeyLocation = "NpcHintKeyLocation";
    private const string NpcHowAreThings = "NpcHowAreThings";
    private const string Location1 = "Location1";
    private const string Location2 = "Location2";
    private const string Location3 = "Location3";
    private const string SafeAlreadyOpened = "SafeAlreadyOpened";
    private const string SafeLocked = "SafeLocked";
    private const string SafeOpened = "SafeOpened";
    private const string KeyMinigameEntry = "KeyMinigameEntry";
    private const string Key = "key";
    private const string Item = "item";

    private InventoryService Inventory => Engine.GetService<InventoryService>();
    private ICustomVariableManager Variables => Engine.GetService<ICustomVariableManager>();
    private IScriptPlayer Player => Engine.GetService<IScriptPlayer>();
    
    private bool HasTalkedToNpc => Variables.TryGetVariableValue(VarTalkedToNpc, out bool v) && v;
    private bool HasCheckedSafe => Variables.TryGetVariableValue(VarSafeChecked, out bool v) && v;
    private bool HasKey => Inventory.HasItem(Key);
    private bool HasQuestItem => Inventory.HasItem(Item);

    private void MarkTalkedToNpc() => Variables.SetVariableValue(VarTalkedToNpc, "true");
    private void MarkSafeChecked() => Variables.SetVariableValue(VarSafeChecked, "true");

    public async void OnNpcClicked()
    {
        string label = !HasTalkedToNpc ? NpcFirstMeet
            : HasQuestItem ? NpcThanks
            : HasCheckedSafe ? NpcHintKeyLocation
            : NpcHowAreThings;

        if (!HasTalkedToNpc)
        {
            MarkTalkedToNpc();
        }
        
        await Player.PreloadAndPlayAsync(Location1, label: label);
    }

    public async void OnSafeClicked()
    {
        if (HasQuestItem)
        {
            await Player.PreloadAndPlayAsync(Location2, label: SafeAlreadyOpened);
            return;
        }

        if (!HasKey)
        {
            MarkSafeChecked();
            await Player.PreloadAndPlayAsync(Location2, label: SafeLocked);
            return;
        }

        Inventory.RemoveItem(Key);
        Inventory.AddItem(Item);
        await Player.PreloadAndPlayAsync(Location2, label: SafeOpened);
    }

    public async void OnKeyClicked()
    {
        await Player.PreloadAndPlayAsync(Location3, label: KeyMinigameEntry);
    }

    public void OnLocationButtonClicked(string locationScriptName)
    {
        Player.PreloadAndPlayAsync(locationScriptName).Forget();
    }
}