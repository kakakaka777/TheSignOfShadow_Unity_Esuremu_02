using System.Collections.Generic;
using UnityEngine;

public class Death_Trap : MonoBehaviour
{
    public enum KillMode
    {
        AfterStaySeconds,  // 一定時間滞在で死亡（毒の雨など）
        InstantOnEnter     // 触れた瞬間に死亡（とらばさみ・穴・矢など）
    }

    [Header("基本設定")]
    public KillMode killMode = KillMode.AfterStaySeconds;

    [Tooltip("AfterStaySeconds のとき、この秒数を超えると死亡")]
    [Min(0.0f)]
    public float killAfterSeconds = 1.0f;

    [Tooltip("トリガーから出たときに時間をリセットするか（AfterStaySeconds 用）")]
    public bool resetOnExit = true;

    [Header("対象プレイヤーのタグ(最大4人を想定)")]
    public string[] playerTags = { "Player1", "Player2", "Player3", "Player4", "Player" };

    [Header("発動条件(任意)")]
    [Tooltip("指定した場合、この TrapActivator が発動中のみ有効")]
    public Activator_Trap activator;

    // 当たっているプレイヤーごとの経過時間（AfterStaySeconds 用）
    private readonly Dictionary<GameObject, float> stayTimes = new Dictionary<GameObject, float>();

    private void Update()
    {
        // 発動してないなら何もしない
        if (activator != null && !activator.isActive) return;

        // 即死モードのときは Update 不要
        if (killMode != KillMode.AfterStaySeconds) return;
        if (stayTimes.Count == 0) return;

        List<GameObject> toKill = null;

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
        if (!IsPlayer(other.gameObject)) return;
        if (activator != null && !activator.isActive) return;

        if (killMode == KillMode.InstantOnEnter)
        {
            // 触れた瞬間に即死
            KillPlayer(other.gameObject);
        }
        else // AfterStaySeconds
        {
            if (!stayTimes.ContainsKey(other.gameObject))
            {
                stayTimes.Add(other.gameObject, 0f);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (killMode != KillMode.AfterStaySeconds) return;
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
        // ★実際のゲーム用の死亡処理に差し替え推奨★
        // var hp = player.GetComponent<PlayerHealth>();
        // if (hp != null) hp.DieByTrap();

        Destroy(player);
    }
}
