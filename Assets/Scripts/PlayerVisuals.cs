using System.Collections.Generic;
using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{

    public AudioSource audioSource;
    public List<AudioClip> footstepSounds;

    public void AimAt(Vector3 pos)
    {
        Vector3 dif = pos - transform.position;
        transform.rotation = Quaternion.Euler(0, -Mathf.Atan2(dif.z, dif.x) * Mathf.Rad2Deg, 0);
    }

    public void PlayFootstepSound()
    {
        SoundManager.Instance.PlaySound(footstepSounds[Random.Range(0, footstepSounds.Count)], 0.8f, true);
    }
}
