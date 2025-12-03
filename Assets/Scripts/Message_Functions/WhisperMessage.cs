using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhisperMessage : MessageFunction
{
    // Start is called before the first frame update
    public override void Activate(Vector3 playerPosition)
    {
        // UIから発動：プレイヤーの正面に印を残す
        Vector3 origin = playerPosition + Vector3.up * 1f;
        Vector3 direction = transform.forward;

        
    }
}
