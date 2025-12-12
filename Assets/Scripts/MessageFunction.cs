using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MessageFunction : MonoBehaviour
{
    
    public bool canUse = false;

    public Transform player01;
    public Transform player02;

    

    // 呼び出し側は今まで通り Activate を呼ぶ


    public void Activate(Vector3 playerPosition)
    {
        if (canUse == true)
        {

            OnActivate(playerPosition); 
        }

        //if (PlayerID.playerID == 0)
        //{
        //    PlayerID.playTransform = player01;
        //}
        //else if (PlayerID.playerID == 1)
        //{
        //    PlayerID.playTransform = player02;
        //}

        else return;
        

        
    }
    // 各メッセージで処理する
    public abstract void OnActivate(Vector3 playerPosition);


}
