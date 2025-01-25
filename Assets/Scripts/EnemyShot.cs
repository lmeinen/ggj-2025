using UnityEngine;

public class EnemyShot : Shot
{
    public void HitAWall()
    {
        WhenDestroyed();
        Destroy(gameObject);
    }
}
