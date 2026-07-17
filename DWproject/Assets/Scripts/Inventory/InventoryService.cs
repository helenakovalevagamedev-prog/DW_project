using System;
using System.Collections.Generic;
using System.Linq;
using Naninovel;

[InitializeAtRuntime]
public class InventoryService : IStatefulService<GameStateMap>
{
    private readonly HashSet<string> items = new();

    public bool HasItem(string itemId) => items.Contains(itemId);

    public void AddItem(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
        {
            return;
        }
        items.Add(itemId);
    }

    public void RemoveItem(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
        {
            return;
        }
        items.Remove(itemId);
    }

    #region IEngineService

    public void SaveServiceState(GameStateMap stateMap)
    {
        var state = new InventoryState { Items = items.ToList() };
        stateMap.SetState(state);
    }
    
    public UniTask LoadServiceStateAsync(GameStateMap stateMap)
    {
        var state = stateMap.GetState<InventoryState>();
        ResetService();

        if (state is null) return UniTask.CompletedTask;

        foreach (var id in state.Items)
        {
            AddItem(id);
        }

        return UniTask.CompletedTask;
    }

    #endregion

    #region IStatefulService

    UniTask IEngineService.InitializeServiceAsync()
    {
        return UniTask.CompletedTask;
    }

    public void ResetService()
    {
        var old = items.ToList();
        items.Clear();
    }
    
    public void DestroyService() { }
    #endregion
}