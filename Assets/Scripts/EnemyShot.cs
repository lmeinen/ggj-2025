using UnityEngine;

public class EnemyShot : Shot
{
    public GameObject warningPlane;

    [Tooltip("Audioclip to use (as base - pitch variations are randomized) when shot hits sth")]
    public AudioClip hitSound;

    Material _warningMat;

    protected override void MyStart()
    {
        base.MyStart();
        warningPlane.transform.rotation = Quaternion.identity;
        _warningMat = warningPlane.GetComponent<MeshRenderer>().material;
    }

    protected override AudioClip HitSound() => hitSound;

    protected override void MyUpdate()
    {
        base.MyUpdate();
        warningPlane.transform.position = new Vector3(transform.position.x, 0.01f, transform.position.z);
        _warningMat.SetFloat("_ProjectileHeight", transform.position.y);
    }
}
