using UnityEngine;

public class EnemyShot : Shot
{
    protected override void WhenDestroyed()
    {
        base.WhenDestroyed();
        SpawnParticles(LevelState.I.bulletPopParticles, Color.white);
    }


    public void HitAWall()
    {
        WhenDestroyed();
        Destroy(gameObject);
    }
}
