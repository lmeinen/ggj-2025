using UnityEngine;

public class VisualEffectManager : MonoBehaviour
{
    [SerializeField] private Material bluepillStyle;
    [SerializeField] private Material redpillStyle;

    private Renderer targetRenderer; // Drag the specific renderer in the Inspector

    private void Awake()
    {
        targetRenderer = GetComponent<Renderer>();
    }

    public void EnableBluepill()
    {
        targetRenderer.material = bluepillStyle;
    }

    public void EnableRedpill()
    {
        targetRenderer.material = redpillStyle;
    }
}
