using UnityEngine;

public class GenericCharacterVisibility : MonoBehaviour
{
    [SerializeField] private GameObject visualRoot;

    private void Reset()
    {
        visualRoot = gameObject;
    }

    public void SetVisible(bool visible)
    {
        (visualRoot != null ? visualRoot : gameObject).SetActive(visible);
    }
}