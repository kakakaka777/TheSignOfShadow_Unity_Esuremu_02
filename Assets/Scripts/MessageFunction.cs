using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MessageFunction : MonoBehaviour
{
    public bool canUse = false;

    // 呼び出し側は今まで通り Activate を呼ぶ


    public void Activate(Vector3 playerPosition)
    {
        if (canUse == true)
        {

            OnActivate(playerPosition); 
        }
        else return;
        

        
    }
    // 各メッセージで処理する
    public abstract void OnActivate(Vector3 playerPosition);


}
