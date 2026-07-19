using System;
using System.Collections;
using UnityEngine;

public class DisplayController : MonoBehaviour
{
    private const int AniIntLose = 4;
    private const int AniIntWin = 3;
    
    [SerializeField] private GameObject npc;
    [SerializeField] private Animator npcAnimator;
    [SerializeField] private GameObject key;
    [SerializeField] private BoxCollider2D keyCollider;
    [SerializeField] private GameObject safe;
    
    private Coroutine pendingAnimationUpdate;
    private Func<GameState> getCurrentGameState;
    private int AniIntHash = Animator.StringToHash("aniInt");
    private bool isLocationAllowsVisibleSafe;
    private bool isLocationAllowsVisibleKey;
    private bool isHiddenKey;
    private bool isLocationAllowsVisibleNPC;
    private bool isHiddenByDialogueNPC;
    private bool isQuestCompleted;

    public void Init(Func<GameState> getCurrentGameState)
    {
        this.getCurrentGameState = getCurrentGameState;
    }
    
    // for commands usage
    public void ChangeVisibility(bool isVisible)
    {
        isHiddenByDialogueNPC = !isVisible;
        Refresh(getCurrentGameState?.Invoke());
    }

    // for c#
    public void Refresh(GameState state)
    {
        isLocationAllowsVisibleNPC = state.CurrentLocation == Location.Location1;
        isLocationAllowsVisibleSafe = state.CurrentLocation == Location.Location2;
        isLocationAllowsVisibleKey = state.CurrentLocation == Location.Location3;
        isHiddenKey = state.HasMinigameWinned;
        isQuestCompleted = state.HasQuestCompleted;
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
            bool isVisible = isLocationAllowsVisibleNPC && !isHiddenByDialogueNPC;
            npc.SetActive(isVisible);
            if (isVisible)
            {
                if (pendingAnimationUpdate != null)
                {
                    StopCoroutine(pendingAnimationUpdate);
                }
                pendingAnimationUpdate = StartCoroutine(ApplyAnimationNextFrame());
            }
        }
        safe.SetActive(isLocationAllowsVisibleSafe);
        key.SetActive(isLocationAllowsVisibleKey && !isHiddenKey);
    }
    
    private IEnumerator ApplyAnimationNextFrame()
    {
        yield return null;
        npcAnimator.SetInteger(AniIntHash, isQuestCompleted ? AniIntWin : AniIntLose);
        pendingAnimationUpdate = null;
    }
}