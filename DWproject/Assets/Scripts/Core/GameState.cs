using System;
using System.Collections.Generic;

[Serializable]
public class GameState
{
    public bool HasTalkedToNpc { get; private set; }
    public bool HasCheckedSafe { get; private set; }
    public Location CurrentLocation { get; private set; }

    public GameState(bool hasTalkedToNpc, bool hasCheckedSafe, Location currentLocation)
    {
        Update(hasTalkedToNpc, hasCheckedSafe, currentLocation);
    }

    public void Update(bool hasTalkedToNpc, bool hasCheckedSafe, Location currentLocation)
    {
        HasTalkedToNpc = hasTalkedToNpc;
        HasCheckedSafe =  hasCheckedSafe;
        CurrentLocation = currentLocation;
    }
}