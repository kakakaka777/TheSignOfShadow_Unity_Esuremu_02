using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activator_Trap : MonoBehaviour
{
    public enum ActivationType
    {
        None,
        Trigger,
        Distance
    }

    [Header("発動方法")]
    public ActivationType activationType = ActivationType.Trigger;

    [Tooltip("ゲーム開始時からすでに発動済みにするか")]
    public bool startActive = false;

    [Header("トリガー発動設定")]
    [Tooltip("このタグのオブジェクトがトリガーに入ったら発動")]
    public string triggerTag = "Player";

    [Header("距離発動設定")]
    [Tooltip("距離判定の対象になるプレイヤー(最大4人を想定)")]
    public Transform[] targetPlayers; // シーン上は1人だけでOK
    [Tooltip("この距離以内にプレイヤーが入ったら発動")]
    public float activationDistance = 5f;
    [Tooltip("距離が離れたら解除するか")]
    public bool deactivateWhenFar = true;

    [Header("発動時に自動でON/OFFするオブジェクト・コンポーネント")]
    [Tooltip("発動時に SetActive(true/false) するオブジェクト")]
    public GameObject[] objectsToToggle;
    [Tooltip("発動時に enabled を切り替えるコンポーネント")]
    public MonoBehaviour[] componentsToToggle;

    [HideInInspector]
    public bool isActive;

    

   

    private float checkDistance;

    private void Start()
    {
    }

    private void Update()
    {
        if (activationType == ActivationType.Distance)
        {
            CheckDistanceActivation();
        }

        Debug.Log("プレイヤーとの距離" + checkDistance) ;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activationType != ActivationType.Trigger) return;

        if (!string.IsNullOrEmpty(triggerTag) && other.CompareTag(triggerTag))
        {
            isActive = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 「トリガーから出たら解除」したいならここに処理を書く
        // 例: if (other.CompareTag(triggerTag)) SetActive(false);
    }

    private void CheckDistanceActivation()
    {
        if (targetPlayers == null || targetPlayers.Length == 0) return;

        checkDistance = float.MaxValue;
        foreach (var t in targetPlayers)
        {
            if (t == null) continue;
            checkDistance = Vector3.Distance(transform.position, t.position);
            
        }



        //if (checkDistance == float.MaxValue) return;



        if (checkDistance <= activationDistance)
        {
            isActive = true;
            Debug.Log("isActive : " + isActive);
        }
    }

    
}
