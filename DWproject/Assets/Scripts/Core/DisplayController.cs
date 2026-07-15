using UnityEngine;

public class DisplayController: MonoBehaviour
{
        [SerializeField] private GameObject npc;

        public void ChangeVisability(bool isVisible)
        {
                if (npc != null) npc.SetActive(isVisible);
        }
}