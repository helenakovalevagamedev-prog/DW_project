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
    private TicTacToeService ticTacToe;
    private bool hasMinigameWinned;
    private bool hasSafeOpened;
    
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
        string label = !HasTalkedToNpc ? Consts.NpcFirstMeet
            : HasQuestItem ? Consts.NpcThanks
            : HasCheckedSafe ? Consts.NpcHintKeyLocation
            : Consts.NpcHowAreThings;

        if (!HasTalkedToNpc)
        {
            MarkTalkedToNpc();
        }
        //displayController.ChangeVisibility(false);
        await Player.PreloadAndPlayAsync(Consts.Location1, label: label);
        UpdateGameState();
    }

    public async void OnSafeClicked()
    {
        Debug.Log($"HasQuestItem {HasQuestItem}, HasKey {HasKey}");
        if (HasQuestItem)
        {
            await Player.PreloadAndPlayAsync(Consts.Location2, label: Consts.SafeAlreadyOpened);
            Debug.Log($"HasQuestItem");
            return;
        }

        if (!HasKey)
        {
            MarkSafeChecked();
            await Player.PreloadAndPlayAsync(Consts.Location2, label: Consts.SafeLocked);
            Debug.Log($"!HasKey ");
            return;
        }

        Inventory.RemoveItem(Consts.Key);
        Inventory.AddItem(Consts.Item);
        Debug.Log($"await");
        await Player.PreloadAndPlayAsync(Consts.Location2, label: Consts.SafeOpened);
        Debug.Log($"update state");
        hasSafeOpened = true;
        UpdateGameState();
    }

    public async void OnKeyClicked()
    {
        await Player.PreloadAndPlayAsync(Consts.Location3, label: Consts.KeyMinigameEntry);
        UpdateGameState();
    }

    private void OnLocationButtonClicked(string buttonName)
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
        Player.PreloadAndPlayAsync(newLocationScriptName).Forget();
 
        if (Enum.TryParse(newLocationScriptName, out Location location))
        {
            SetCurrentLocation(location);
        }
    }
    
    public void SetCurrentLocation(Location location)
    {
        if (currentLocation == location)
        {
            return;
        }
        currentLocation = location;
        UpdateGameState();
    }

    private void UpdateGameState()
    {
        gameState.Update(HasTalkedToNpc, HasCheckedSafe, currentLocation, hasMinigameWinned, hasSafeOpened);
        OnGameStateChanged?.Invoke(gameState);
    }

    private void HandleMinigameWinned(GameResult gameResult)
    {
        if (gameResult == GameResult.PlayerWon)
        {
            hasMinigameWinned = true;
            UpdateGameState();
        }
    }
    
    private async void OnEnable()
    {
        gameState = new GameState(false, false, currentLocation, false, false);
        UIController.Init(OnLocationButtonClicked, gameState);
        OnGameStateChanged += UIController.Refresh;
        OnGameStateChanged += displayController.Refresh;
        OnGameStateChanged?.Invoke(gameState);
        
        while (ticTacToe == null)
        {
            ticTacToe = Engine.GetService<TicTacToeService>();
            if (ticTacToe == null)
            {
                await UniTask.Yield();
            }
        }
        
        ticTacToe.OnGameEnded += HandleMinigameWinned;
    }
 
    private void OnDisable()
    {
        OnGameStateChanged -= UIController.Refresh;
        OnGameStateChanged -= displayController.Refresh;
        if (ticTacToe != null)
        {
            ticTacToe.OnGameEnded -= HandleMinigameWinned;
        }
    }
}