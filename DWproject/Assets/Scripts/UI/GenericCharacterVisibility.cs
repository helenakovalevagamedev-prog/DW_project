using UnityEngine;

public class GenericCharacterVisibility : MonoBehaviour
{
    [SerializeField] private GameObject visualRoot;

    private Renderer[] renderers;
 
    private void Reset()
    {
        visualRoot = gameObject;
    }
 
    private void Awake()
    {
        var root = visualRoot != null ? visualRoot : gameObject;
        renderers = root.GetComponentsInChildren<Renderer>(includeInactive: true);
    }
 
    public void SetVisible(bool visible)
    {
        foreach (var r in renderers)
            if (r != null) r.enabled = visible;
    }
}