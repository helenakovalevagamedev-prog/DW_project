using UnityEngine;

public class DisplayController : MonoBehaviour
{
    [SerializeField] private GameObject npc;
    [SerializeField] private GameObject key;
    [SerializeField] private BoxCollider2D keyCollider;
    [SerializeField] private GameObject safe;

    private bool isLocationAllowsVisibleSafe = false;
    private bool isLocationAllowsVisibleKey = false;
    private bool isHiddenKey = false;
    private bool isLocationAllowsVisibleNPC = false;
    private bool isHiddenByDialogueNPC = false;

    // for commands usage
    public void ChangeVisibility(bool isVisible)
    {
        isHiddenByDialogueNPC = !isVisible;
        Apply();
    }

    // for c#
    public void Refresh(GameState state)
    {
        isLocationAllowsVisibleNPC = state.CurrentLocation == Location.Location1;
        isLocationAllowsVisibleSafe = state.CurrentLocation == Location.Location2;
        isLocationAllowsVisibleKey = state.CurrentLocation == Location.Location3;
        Apply();
    }
    
    
    public void SetKeyInteractable(bool isKeyInteractable)
    {
        keyCollider.enabled = isKeyInteractable;
    }

    private void Apply()
    {
        if (npc != null)
        {
            npc.SetActive(isLocationAllowsVisibleNPC && !isHiddenByDialogueNPC);
        }
        safe.SetActive(isLocationAllowsVisibleSafe);
        key.SetActive(isLocationAllowsVisibleKey);
    }
}