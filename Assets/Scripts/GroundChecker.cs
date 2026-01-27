using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    private HashSet<GameObject> touchingObjects = new HashSet<GameObject>();

    private bool unlocked = false;

    private void OnCollisionEnter(Collision collision)
    {
        touchingObjects.Add(collision.gameObject);
        CheckCondition();
    }

    private void OnCollisionExit(Collision collision)
    {
        touchingObjects.Remove(collision.gameObject);
        CheckCondition();
    }

    void CheckCondition()
    {
        if (unlocked) return;

        bool playerOnGround = false;
        bool enemyOnGround = false;

        foreach (GameObject obj in touchingObjects)
        {
            if (obj == null) continue;

            if (obj.CompareTag("Player"))
                playerOnGround = true;

            if (obj.CompareTag("Enemy") || obj.CompareTag("EnemyKing"))
                enemyOnGround = true;
        }

        // Åö PlayerÇæÇØÇ™GroundÇ…êGÇÍÇƒÇ¢ÇÈ
        if (playerOnGround && !enemyOnGround)
        {
            unlocked = true;

            // UIï\é¶
            GameManager.Instance.UnlockMessage("ååï∂éö");
        }
    }
}
