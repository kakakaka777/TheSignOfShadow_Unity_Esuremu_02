using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathAfterSecond_Trap : MonoBehaviour
{
    [Header("基本設定")]
    [Tooltip("この秒数を超えると死亡")]
    [Min(0.0f)]
    public float killAfterSeconds = 1.0f;

    [Tooltip("トリガーから出たときに時間をリセットするか")]
    public bool resetOnExit = true;

    [Header("対象プレイヤーのタグ(最大4人を想定)")]
    public string[] playerTags = { "Player1", "Player2", "Player3", "Player4", "Player" };

    [Header("発動条件(任意)")]
    [Tooltip("指定した場合、この TrapActivator が発動中のみ有効")]
    public Activator_Trap activator;

    // 当たっているプレイヤーごとの経過時間
    private readonly Dictionary<GameObject, float> stayTimes = new Dictionary<GameObject, float>();

    private void Update()
    {
        if (activator != null && !activator.isActive) return;

        if (stayTimes.Count == 0) return;

        // Dictionary を回しながら削除できないので一旦バッファ
        List<GameObject> toKill = null;

        // コピーをforループ
        foreach (var kv in new List<KeyValuePair<GameObject, float>>(stayTimes))
        {
            GameObject player = kv.Key;
            float time = kv.Value + Time.deltaTime;

            stayTimes[player] = time;

            if (time >= killAfterSeconds)
            {
                if (toKill == null) toKill = new List<GameObject>();
                toKill.Add(player);
            }
        }

        if (toKill != null)
        {
            foreach (var p in toKill)
            {
                stayTimes.Remove(p);
                KillPlayer(p);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsPlayer(other.gameObject))
        {
            if (!stayTimes.ContainsKey(other.gameObject))
            {
                stayTimes.Add(other.gameObject, 0f);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!resetOnExit) return;

        if (stayTimes.ContainsKey(other.gameObject))
        {
            stayTimes.Remove(other.gameObject);
        }
    }

    private bool IsPlayer(GameObject obj)
    {
        if (playerTags == null) return false;
        foreach (var tag in playerTags)
        {
            if (string.IsNullOrEmpty(tag)) continue;
            if (obj.CompareTag(tag)) return true;
        }
        return false;
    }

    private void KillPlayer(GameObject player)
    {
        // ★ここで実際の死亡処理を呼ぶ★
        // - Player 側に OnTrapDeath() や Die() を用意しておく
        // - SendMessageOptions.DontRequireReceiver で未実装でもエラーにしない
        //player.SendMessage("OnTrapDeath", SendMessageOptions.DontRequireReceiver);
        //player.SendMessage("Die", SendMessageOptions.DontRequireReceiver);

        Destroy(player);

        // 必要ならここで Destroy(player); なども可
    }
}
