using System;
using System.Collections.Generic;

[Serializable]
public class GameState
{
    public bool HasTalkedToNpc { get; private set; }
    public bool HasCheckedSafe { get; private set; }
    public bool HasMinigameWinned { get; private set; }
    public bool HasSafeOpened { get; private set; }
    public Location CurrentLocation { get; private set; }

    public GameState(
        bool hasTalkedToNpc, 
        bool hasCheckedSafe,
        Location currentLocation,
        bool hasMinigameWinned,
        bool hasSafeOpened)
    {
        Update(hasTalkedToNpc, hasCheckedSafe, currentLocation, hasMinigameWinned, hasSafeOpened);
    }

    public void Update(
        bool hasTalkedToNpc, 
        bool hasCheckedSafe, 
        Location currentLocation, 
        bool hasMinigameWinned,
        bool hasSafeOpened)
    {
        HasTalkedToNpc = hasTalkedToNpc;
        HasCheckedSafe =  hasCheckedSafe;
        CurrentLocation = currentLocation;
        HasMinigameWinned = hasMinigameWinned;
        HasSafeOpened = hasSafeOpened;
    }
}