using System.Collections;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    Material _material;

    public float cooldown = .5f;
    public float maxAmount = 2f;
    public float distance = 4f;
    float _dashAmount = 0f;
    public float duration = .15f;

    Vector3 _vel;
    public Vector3 Velocity => _vel;


    float _dashStart = 0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _material = GetComponent<MeshRenderer>().material;
        _material.SetFloat("_Sections", maxAmount);
    }

    // Update is called once per frame
    void Update()
    {
        _dashAmount = Mathf.Min(maxAmount, _dashAmount + Time.deltaTime / cooldown);
        _material.SetFloat("_Charge", _dashAmount);
        if (Time.realtimeSinceStartup - _dashStart > duration)
        {
            _vel = Vector3.zero;
        }
    }

    public bool CanDash()
    {
        if (_dashAmount >= 1f)
        {
            _dashAmount -= 1f;
            return true;
        }
        return false;
    }

    public void Dash(Vector3 dash_dir)
    {
        _dashStart = Time.realtimeSinceStartup;
        _vel = dash_dir * distance / duration;
    }
}
