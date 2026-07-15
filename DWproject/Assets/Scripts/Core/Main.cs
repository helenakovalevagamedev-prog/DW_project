using System;
using UnityEngine;
using Naninovel;

public class Main : MonoBehaviour
{
    [SerializeField] private UIController UIController;
    [SerializeField] private DisplayController displayController;
    
    public event Action<GameState> OnGameStateChanged;
    private Location currentLocation = Location.Main;
    private GameState gameState;
    
    private InventoryService Inventory => Engine.GetService<InventoryService>();
    private ICustomVariableManager Variables => Engine.GetService<ICustomVariableManager>();
    private IScriptPlayer Player => Engine.GetService<IScriptPlayer>();
    
    private bool HasTalkedToNpc => Variables.TryGetVariableValue(Consts.VarTalkedToNpc, out bool v) && v;
    private bool HasCheckedSafe => Variables.TryGetVariableValue(Consts.VarSafeChecked, out bool v) && v;
    private bool HasKey => Inventory.HasItem(Consts.Key);
    private bool HasQuestItem => Inventory.HasItem(Consts.Item);

    private void MarkTalkedToNpc() => Variables.SetVariableValue(Consts.VarTalkedToNpc, "true");
    private void MarkSafeChecked() => Variables.SetVariableValue(Consts.VarSafeChecked, "true");

    public async void OnNpcClicked()
    {
        Camera UiCamera = GameObject.Find("UICamera")?.GetComponent<Camera>();
        if (UiCamera != null) UiCamera.enabled = true;
        string label = !HasTalkedToNpc ? Consts.NpcFirstMeet
            : HasQuestItem ? Consts.NpcThanks
            : HasCheckedSafe ? Consts.NpcHintKeyLocation
            : Consts.NpcHowAreThings;

        if (!HasTalkedToNpc)
        {
            MarkTalkedToNpc();
        }
        displayController.ChangeVisibility(false);
        await Player.PreloadAndPlayAsync(Consts.Location1, label: label);
        //displayController.ChangeVisability(true);
        UpdateGameState();
    }

    public async void OnSafeClicked()
    {
        if (HasQuestItem)
        {
            await Player.PreloadAndPlayAsync(Consts.Location2, label: Consts.SafeAlreadyOpened);
            return;
        }

        if (!HasKey)
        {
            MarkSafeChecked();
            await Player.PreloadAndPlayAsync(Consts.Location2, label: Consts.SafeLocked);
            return;
        }

        Inventory.RemoveItem(Consts.Key);
        Inventory.AddItem(Consts.Item);
        await Player.PreloadAndPlayAsync(Consts.Location2, label: Consts.SafeOpened);
        UpdateGameState();
    }

    public async void OnKeyClicked()
    {
        await Player.PreloadAndPlayAsync(Consts.Location3, label: Consts.KeyMinigameEntry);
    }

    public void OnLocationButtonClicked(string buttonName)
    {
        var newLocationScriptName = Consts.Location1;
        if (buttonName == Consts.RightButton)
        {
            newLocationScriptName = currentLocation == Location.Location1 ? Consts.Location2 : Consts.Location1;
        }

        if (buttonName == Consts.LeftButton)
        {
            newLocationScriptName = currentLocation == Location.Location1 ? Consts.Location3 : Consts.Location1;
        }
        Debug.Log($"Current location {currentLocation}, button {buttonName}, newLocationScriptName {newLocationScriptName}");
        // Без указания label — скрипт проигрывается с самого начала (@back + проверка интро).
        Player.PreloadAndPlayAsync(newLocationScriptName).Forget();
 
        if (Enum.TryParse(newLocationScriptName, out Location location))
        {
            SetCurrentLocation(location);
            Debug.Log($"parsed location {location}");
        }
    }

    /// <summary>
    /// Обновляет только состояние текущей локации (для UI-контроллеров вроде
    /// LocationUIController), НЕ трогая воспроизведение скрипта. Безопасно
    /// вызывать из команды (например, @showTitleMenu), которая сама
    /// выполняется посреди другого проигрываемого скрипта — в отличие от
    /// OnLocationButtonClicked, которая ещё и запускает PreloadAndPlayAsync.
    /// </summary>
    ///
    
    public void SetCurrentLocation(Location location)
    {
        if (currentLocation == location) return;
        currentLocation = location;
        UpdateGameState();
    }

    private void UpdateGameState()
    {
        gameState.Update(HasTalkedToNpc, HasCheckedSafe, currentLocation);
        OnGameStateChanged?.Invoke(gameState);
    }
    
    private void OnEnable()
    {
        gameState = new GameState(false, false, currentLocation);
        UIController.Init(OnLocationButtonClicked, gameState);
        OnGameStateChanged += UIController.Refresh;
        OnGameStateChanged += displayController.Refresh;
    }
 
    private void OnDisable()
    {
        OnGameStateChanged -= UIController.Refresh;
        OnGameStateChanged -= displayController.Refresh;
    }
}