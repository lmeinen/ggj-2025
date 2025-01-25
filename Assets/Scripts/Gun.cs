using System;
using UnityEngine;
using UnityEngine.InputSystem;



[Serializable]
public struct GunParameters
{
    [Tooltip("How many projectiles to fire per second")]
    public float fireRate;

    [Tooltip("How fast the projectile moves")]
    public float projectileSpeed;

    [Tooltip("The projectile to fire. Must contain the Shot.cs script.")]
    public GameObject projectile;

    [Tooltip("How much the projectiles spreads, in radians")]
    public float projectileSpread;
}




public class Gun : MonoBehaviour
{
    public GunParameters gunParameters;

    public Transform projectileEmitTransform;

    //how many projectiles to fire next frame
    float fireAmount = 0f;

    //true if the owner is attempting to fire
    private bool _shouldFire = false;
    public bool ShouldFire
    {
        get => _shouldFire;
        set => _shouldFire = value;
    }
    
    // Update is called once per frame
    void Update()
    {
        //if we should fire, add to fire amount and fire any generated projectiles
        if (ShouldFire)
        {
            fireAmount += gunParameters.fireRate * Time.deltaTime;

            while (fireAmount > 0f)
            {
                fireAmount -= 1f;
                Fire();
            }
        }
    }


    void Fire()
    {
        //spawn a projectile and send it in the firing direction
        LevelState.CreateShot(gunParameters.projectile, projectileEmitTransform.position, projectileEmitTransform.rotation, transform.right * gunParameters.projectileSpeed, gunParameters.projectileSpread);

        //Vector3 fire_dir = transform.right;
        //var shot = Instantiate(gunParameters.projectile, transform.position + fire_dir * transform.localScale.x, transform.rotation, LevelState.I.transform);
        //shot.GetComponent<Shot>().Initialize(fire_dir * gunParameters.projectileSpeed, gunParameters.projectileSpread);
    }

    //Rotate the gun to point it at the provided position
    public void AimAt(Vector3 pos)
    {
        Vector3 dif = pos - transform.position;
        transform.rotation = Quaternion.Euler(0, -Mathf.Atan2(dif.z, dif.x) * Mathf.Rad2Deg, 0);
    }
}
