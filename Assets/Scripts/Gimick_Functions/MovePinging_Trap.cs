using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePinging_Trap : MonoBehaviour
{
    [Header("移動設定")]
    [Tooltip("移動方向（左右なら (1,0,0)、上下なら (0,1,0) など）")]
    public Vector3 moveDirection = Vector3.right;

    [Tooltip("移動距離")]
    [Min(0f)]
    public float moveDistance = 3f;

    [Tooltip("片道の移動時間(秒)")]
    [Min(0.01f)]
    public float moveSpeed = 1f;

    [Tooltip("ローカル座標系で移動するか（親オブジェクト基準）")]
    public bool useLocalSpace = true;

    [Tooltip("ゲーム開始と同時に自動再生")]
    public bool playOnAwake = true;

    [Header("発動条件(任意)")]
    [Tooltip("指定した場合、この TrapActivator が発動中のみ動く")]
    public Activator_Trap activator;

    private Vector3 startPos;
    private float time;

    private void Start()
    {
        activator = GetComponent<Activator_Trap>();
        startPos = useLocalSpace ? transform.localPosition : transform.position;
        time = 0f;
    }

    private void Update()
    {
        //if (!playOnAwake) return;
        //if (activator != null && activator.isActive == false) return;


        

        if (activator.isActive == true)
        {
       
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);

            
            //// 0〜1を PingPong させる
            ////float t = Mathf.PingPong(time / moveDuration, 1f);

            //Vector3 offset = moveDirection.normalized * moveDistance * Time.deltaTime;
            //Vector3 targetPos = startPos + offset;

            //if (useLocalSpace)
            //    transform.localPosition = targetPos;
            //else
            //    transform.position = targetPos;
        }
    }

    public void ResetPosition()
    {
        if (useLocalSpace)
            transform.localPosition = startPos;
        else
            transform.position = startPos;

        time = 0f;
    }
}
