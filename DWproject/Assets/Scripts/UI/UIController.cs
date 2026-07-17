using System;
using UnityEngine;
using UnityEngine.UI;

public class UIController: MonoBehaviour
{
    [SerializeField] private Button leftLocationButton;
    [SerializeField] private Button rightLocationButton;
    [SerializeField] private Image inventoryItem;
    [SerializeField] private Sprite keySprite;
    [SerializeField] private Sprite notebookSprite;
    
    private Func<Location> getLocation;
    private GameState lastState;
    private bool isHiddenByDialogue;
    
    public void Init(Action<string> onLocationButtonClicked, GameState state)
    {
        leftLocationButton.onClick.AddListener(()=> onLocationButtonClicked?.Invoke(Consts.LeftButton));
        rightLocationButton.onClick.AddListener(()=> onLocationButtonClicked?.Invoke(Consts.RightButton));
        Refresh(state);
    }
    
    public void Refresh(GameState state)
    {
        lastState = state;
        ChangeLocationButtonsVisibility();
        UpdateInventoryItemIcon(state.HasMinigameWinned, state.HasSafeOpened);
    }
    
    // for commands
    public void ChangeLocationButtonsDuringDialogue(bool hidden)
    {
        isHiddenByDialogue = hidden;
        ChangeLocationButtonsVisibility();
    }

    private void UpdateInventoryItemIcon(bool hasMinigameWinned, bool hasSafeOpened)
    {
        if(hasMinigameWinned)
        {
            inventoryItem.gameObject.SetActive(hasMinigameWinned);
        }
        inventoryItem.sprite = hasSafeOpened ? notebookSprite : keySprite;
    }

    private void ChangeLocationButtonsVisibility()
    {
        bool leftAllowed;
        bool rightAllowed;
 
        switch (lastState.CurrentLocation)
        {
            case Location.Location1:
                rightAllowed = lastState.HasTalkedToNpc;
                leftAllowed = lastState.HasCheckedSafe;
                break;
 
            case Location.Location2:
                leftAllowed = true;
                rightAllowed = false;
                break;
 
            case Location.Location3:
                leftAllowed = false;
                rightAllowed = true;
                break;
 
            default: // Location.Main (титульный экран) и т.п.
                leftAllowed = false;
                rightAllowed = false;
                break;
        }
 
        leftLocationButton.gameObject.SetActive(leftAllowed && !isHiddenByDialogue);
        rightLocationButton.gameObject.SetActive(rightAllowed && !isHiddenByDialogue);
    }
}