using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningMarkMessage : MessageFunction
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnActivate(Vector3 playerPosition)
    {
        Debug.Log("WarningMark（メッセージを残す機能)を実行する");


        // メッセージUIなどから発動されたときの処理（例：周囲に印）
        RaycastHit hit;
        Vector3 rayOrigin = playerPosition + Vector3.up * 1f;
        Vector3 rayDir = transform.forward;

        
    }
}
