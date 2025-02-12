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

    [Tooltip("Audioclip to use (as base - pitch variations are randomized) when firing")]
    public AudioClip fireSound;

    [Tooltip("Volume to play audioclip with")]
    public float fireSoundVolume;
}




public class Gun : MonoBehaviour
{
    public GunParameters gunParameters;

    public Transform projectileEmitTransform;

    //how many projectiles to fire next frame
    float fireAmount = 0f;

    //true if the owner is attempting to fire
    //private bool _shouldFire = false;
    //public bool ShouldFire
    //{
    //    get => _shouldFire;
    //    set => _shouldFire = value;
    //}

    float _lastFireTime = 0f;
    public bool FiredRecently => Time.realtimeSinceStartup - _lastFireTime < 0.5f;

    // Update is called once per frame
    void Update()
    {
        //if we should fire, add to fire amount and fire any generated projectiles
    }

    public void AttemptToFire(Vector3 parent_velocity)
    {
        fireAmount += gunParameters.fireRate * Time.deltaTime;

        while (fireAmount > 0f)
        {
            fireAmount -= 1f;
            Fire(parent_velocity);
            _lastFireTime = Time.realtimeSinceStartup;
        }
    }


    void Fire(Vector3 parent_velocity)
    {
        //spawn a projectile and send it in the firing direction
        Game.CreateShot(gunParameters.projectile, projectileEmitTransform.position, projectileEmitTransform.rotation, transform.right * gunParameters.projectileSpeed, gunParameters.projectileSpread, parent_velocity);

        if (gunParameters.fireSound != null)
        {
            // TODO: Introduce pitch variation
            SoundManager.Instance.PlaySound(gunParameters.fireSound, gunParameters.fireSoundVolume, true);

        }
    }

    //Rotate the gun to point it at the provided position
    public void AimAt(Vector3 pos)
    {
        Vector3 dif = pos - transform.position;
        transform.rotation = Quaternion.Euler(0, -Mathf.Atan2(dif.z, dif.x) * Mathf.Rad2Deg, 0);
    }
}
