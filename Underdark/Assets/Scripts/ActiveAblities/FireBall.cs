using System;
using System.Collections;
using UnityEngine;
using Random = System.Random;

public class FireBall : ProjectileAbility
{
    protected override IEnumerator InstantiateProjectiles()
    {
        var currShotInfo = shotInfo.GetValue(abilityLevel);
        var meanAngle = Mathf.Rad2Deg * Mathf.Acos(Vector2.Dot(Vector2.right, attackDir));
        if (attackDir.y < 0) meanAngle *= -1;
        Random rand = new Random();

        for (int i = 0; i < currShotInfo.Shots; i++)
        {
            for (int j = 0; j < currShotInfo.ProjCountInShot; j++)
            {
                var localAngle = meanAngle;
                if (j != 0)
                {
                    localAngle = NextTriangular(rand, meanAngle - currShotInfo.AngleBetweenProj,
                        meanAngle + currShotInfo.AngleBetweenProj, meanAngle);
                }

                var localDir = new Vector2(Mathf.Cos(localAngle * Mathf.Deg2Rad),
                    Mathf.Sin(localAngle * Mathf.Deg2Rad));
                var velocity = localDir * projSpeed;

                SpawnProjectile(velocity);
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
}
