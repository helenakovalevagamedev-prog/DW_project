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
    
    public void Init(Action<string> onLocationButtonClicked, GameState state)
    {
        leftLocationButton.onClick.AddListener(()=> onLocationButtonClicked?.Invoke(Consts.LeftButton));
        rightLocationButton.onClick.AddListener(()=> onLocationButtonClicked?.Invoke(Consts.RightButton));
        Refresh(state);
    }
    
    public void Refresh(GameState state)
    {
        ChangeLocationButtonsVisibility(state);
        UpdateInventoryItemIcon(state.HasMinigameWinned, state.HasSafeOpened);
    }

    private void UpdateInventoryItemIcon(bool hasMinigameWinned, bool hasSafeOpened)
    {
        if(hasMinigameWinned)
        {
            inventoryItem.gameObject.SetActive(hasMinigameWinned);
        }
        inventoryItem.sprite = hasSafeOpened ? notebookSprite : keySprite;
    }

    private void ChangeLocationButtonsVisibility(GameState state)
    {
        switch (state.CurrentLocation)
        {
            case Location.Location1:
                leftLocationButton.gameObject.SetActive(state.HasCheckedSafe);
                rightLocationButton.gameObject.SetActive(state.HasTalkedToNpc);
                break;
            case Location.Location2:
                leftLocationButton.gameObject.SetActive(true);
                rightLocationButton.gameObject.SetActive(false);
                break;
            case Location.Location3:
                leftLocationButton.gameObject.SetActive(false);
                rightLocationButton.gameObject.SetActive(true);
                break;
            case Location.Main:
            default:
                leftLocationButton.gameObject.SetActive(false);
                rightLocationButton.gameObject.SetActive(false);
                break;
        }
    }
}