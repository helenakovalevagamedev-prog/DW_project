using Naninovel;
using UnityEngine;

[CommandAlias ("addItem")]
public class AddItemCommand : Command
{
    [ParameterAlias ("id"), RequiredParameter]
    public StringParameter ItemId;

    public override UniTask ExecuteAsync(AsyncToken asyncToken = default)
    {
        Engine.GetService<InventoryService>().AddItem(ItemId);
        return UniTask.CompletedTask;
    }
}

[CommandAlias ("removeItem")]
public class RemoveItemCommand : Command
{
    [ParameterAlias ("id"), RequiredParameter] 
    public StringParameter ItemId;

    public override UniTask ExecuteAsync(AsyncToken asyncToken = default)
    {
        Engine.GetService<InventoryService>().RemoveItem(ItemId);
        return UniTask.CompletedTask;
    }
}

[CommandAlias("startMinigame")]
public class StartMinigameCommand : Command
{
    [ParameterAlias("result"), RequiredParameter]
    public StringParameter ResultVariableName;
 
    public override async UniTask ExecuteAsync(AsyncToken asyncToken = default)
    {
        var minigame = Engine.GetService<TicTacToeService>();
        var ticTacToeUI = Engine.GetService<IUIManager>().GetUI<TicTacToeUI>();
        await ticTacToeUI.ChangeVisibilityAsync(true);
        minigame.StartNewGame();
        // проверка токен на случай сброса/уничтожения движка.
        while (minigame.GameActive)
        {
            asyncToken.ThrowIfCanceled();
            await UniTask.Yield();
        }
        await ticTacToeUI.ChangeVisibilityAsync(false);
        bool playerWon = minigame.Result == TicTacToeService.GameResult.PlayerWon;
        Engine.GetService<ICustomVariableManager>()
            .SetVariableValue(ResultVariableName, playerWon.ToString());
    }
}

[CommandAlias("startNewGame")]
public class StartNewGameCommand : Command
{
    public override UniTask ExecuteAsync(AsyncToken asyncToken = default)
    {
        var variables = Engine.GetService<ICustomVariableManager>();
        variables.SetVariableValue(Consts.VarTalkedToNpc, "false");
        variables.SetVariableValue(Consts.VarSafeChecked, "false");
        Engine.GetService<InventoryService>().ResetService();
        var main = Object.FindObjectOfType<Main>();
        if (main != null) main.SetCurrentLocation(Location.Location1);
        Engine.GetService<IScriptPlayer>().PreloadAndPlayAsync(Consts.Location1).Forget();
        return UniTask.CompletedTask;
    }
}

 [CommandAlias ("returnToTitle")]
 public class ReturnToTitleCommand : Command
 {
     public override UniTask ExecuteAsync(AsyncToken asyncToken = default)
     {
         var main = Object.FindObjectOfType<Main>();
         if (main != null) main.SetCurrentLocation(Location.Main);
 
         return UniTask.CompletedTask;
     }
 }
 
 [CommandAlias("hideWorldCharacter")]
 public class HideWorldCharacterCommand : Command
 {
     public override UniTask ExecuteAsync(AsyncToken asyncToken = default)
     {
         var display = Object.FindObjectOfType<DisplayController>();
         if (display != null) display.ChangeVisibility(false);
         return UniTask.CompletedTask;
     }
 }
 
 [CommandAlias("showWorldCharacter")]
 public class ShowWorldCharacterCommand : Command
 {
     public override UniTask ExecuteAsync(AsyncToken asyncToken = default)
     {
         var display = Object.FindObjectOfType<DisplayController>();
         if (display != null) display.ChangeVisibility(true);
         return UniTask.CompletedTask;
     }
 }