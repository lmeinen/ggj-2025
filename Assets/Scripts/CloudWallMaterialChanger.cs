using UnityEngine;

public class CloudWallMaterialChanger : MonoBehaviour
{
    [Tooltip("Array with colors the bubbles should have")]
    [SerializeField] private Color[] colors = new Color[] {  };

    
    protected Material _mat;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _mat = GetComponent<MeshRenderer>().material;
        int rand = Random.Range(0, colors.Length);
        _mat.color = colors[rand];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
