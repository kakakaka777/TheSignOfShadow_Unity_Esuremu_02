using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MessageFunction : MonoBehaviour
{
    [HideInInspector]
    public bool canUse = false;
    public bool clickUsed = false;


    public Transform player01;
    public Transform player02;

    // 一人称カメラ

    [Header("プレイヤーの一人称カメラ")]
    public Camera player01_FPCamera;
    public Camera player02_FPCamera;

    // カーソルアイコン

    [Header("カーソルアイコン")]
    public Texture2D cursorIcon;


    // 呼び出し側は今まで通り Activate を呼ぶ


    protected void Activate(Vector3 playerPosition)
    {
        if (canUse == true)
        {
            PlayerManager.isRButtonUsed = false;
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

    protected void SetCurso()
    {
        Cursor.visible = true;
        Cursor.SetCursor(cursorIcon, Vector2.zero, CursorMode.Auto);
    }

}
