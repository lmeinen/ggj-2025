using UnityEngine;

public class EnemyShot : Shot
{
    public GameObject warningPlane;
    Material _warningMat;

    protected override void MyStart()
    {
        base.MyStart();
        warningPlane.transform.rotation = Quaternion.identity;
        _warningMat = warningPlane.GetComponent<MeshRenderer>().material;
    }

    protected override void MyUpdate()
    {
        base.MyUpdate();
        warningPlane.transform.position = new Vector3(transform.position.x, 0.01f, transform.position.z);
        _warningMat.SetFloat("_ProjectileHeight", transform.position.y);
    }
}
