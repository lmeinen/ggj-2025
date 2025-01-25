using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    public void AimAt(Vector3 pos)
    {
        Vector3 dif = pos - transform.position;
        transform.rotation = Quaternion.Euler(0, -Mathf.Atan2(dif.z, dif.x) * Mathf.Rad2Deg, 0);
    }
}
