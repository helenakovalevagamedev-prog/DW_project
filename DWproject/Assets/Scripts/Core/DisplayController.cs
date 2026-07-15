using UnityEngine;

public class DisplayController : MonoBehaviour
{
    [SerializeField] private GameObject npc;

    public void ChangeVisibility(bool isVisible)
    {
        if (npc != null) npc.SetActive(isVisible);
    }

    public void Refresh(GameState state)
    {
        //ChangeVisibility(state.CurrentLocation == Location.Location1);
    }
}