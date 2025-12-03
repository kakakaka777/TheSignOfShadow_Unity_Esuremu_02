using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyMessage : MessageFunction
{
    public GameObject flyPrefab;
    public int flyCount = 10;
    public float spawnRadius = 2.0f;

    public override void Activate(Vector3 playerPosition)
    {
        Debug.Log("蝿を放つ（メッセージを残す機能)を実行する");
        for (int i = 0; i < flyCount; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPos = playerPosition + new Vector3(randomCircle.x, 0.5f, randomCircle.y);
            GameObject fly = Instantiate(flyPrefab, spawnPos, Quaternion.identity);

            // 蝿にふわふわ浮くような演出を入れてもOK（例：AddForceとか）
        }
    }

}
